using ClientServerApp.Protocol;
using System.Net;
using System.Net.Sockets;

namespace TCP_client_server_uploader.Client;

public class ClientSession : Session, IClientSession
{
    public IClientSession.State CurrentState => _currentState;

    private IClientSession.State _currentState = IClientSession.State.WAITING_FILE_TO_BE_ATTACHED;

    private readonly FileInfo _file;

    public ClientSession(CommandLineArguments commandLineArguments) : base(new Socket(commandLineArguments.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp), new ClientFileSystemOperator())
    {
        IPEndPoint server = new(commandLineArguments.Address, commandLineArguments.Port);
        _socket.Connect(server);
        if (commandLineArguments.FilePath == null)
        {
            throw new NullReferenceException("File path shouldn't be null here.");
        }
        _file = commandLineArguments.FilePath;
    }

    public FinishResponse ReceiveFinishResponse()
    {
        if (_currentState != IClientSession.State.WAITING_SERVER_RESPONSE)
        {
            throw new Exception("Finish response is not expected.");
        }
        FinishResponse response = MessageParser.ParseFinishResponse(ReceiveNBytes(FinishResponse.s_ConstSize));
        _currentState = IClientSession.State.READY_TO_SEND_UPLOAD_REQUEST;
        return response;
    }

    public UploadResponse ReceiveUploadResponse()
    {
        if (_currentState != IClientSession.State.WAITING_SERVER_RESPONSE)
        {
            throw new Exception("Upload response is not expected");
        }
        UploadResponse response = MessageParser.ParseUploadResponse(ReceiveNBytes(UploadResponse.s_ConstSize));
        if (response.Value != UploadResponse.UploadResponseValue.ACCEPTED)
        {
            _currentState = IClientSession.State.READY_TO_SEND_UPLOAD_REQUEST;
        }
        else
        {
            _currentState = IClientSession.State.READY_TO_UPLOAD;
        }
        return response;
    }

    public void SelectFileToUpload(FileInfo path)
    {
        _fileSystemOperator.SelectFile(path);
        _currentState = IClientSession.State.READY_TO_SEND_UPLOAD_REQUEST;
    }

    public void SendUploadRequest(UploadRequest uploadRequest)
    {
        if (_currentState != IClientSession.State.READY_TO_SEND_UPLOAD_REQUEST)
        {
            throw new Exception("Not ready to set an upload request");
        }
        _currentState = IClientSession.State.WAITING_SERVER_RESPONSE;
        _socket.Send(uploadRequest.Serialize());
    }

    public void UploadFile()
    {
        if (_currentState != IClientSession.State.READY_TO_UPLOAD)
        {
            throw new Exception("Session is not ready to upload file.");
        }
        const int bufferSize = 8 * 1024;
        byte[] buffer = new byte[bufferSize];
        long sentData = 0;
        using Stream fileStream = _fileSystemOperator.GetStream(IFileSystemOperator.Mode.READ);
        while (sentData != _fileSystemOperator.GetFileSize())
        {
            int readBytes = fileStream.Read(buffer, 0, buffer.Length);
            int currentSentData = 0;
            while (currentSentData != readBytes)
            {
                currentSentData += _socket.Send(buffer, currentSentData, readBytes - currentSentData, SocketFlags.None);
            }
            sentData += currentSentData;
        }
        _currentState = IClientSession.State.WAITING_SERVER_RESPONSE;
    }

    public override void Start()
    {
        SelectFileToUpload(_file);
        SendUploadRequest(new UploadRequest(_file.Name, _file.Length));
        Console.WriteLine("Sending upload request to {0}", _socket.RemoteEndPoint);
        UploadResponse uploadResponse = ReceiveUploadResponse();
        if (uploadResponse.Value != UploadResponse.UploadResponseValue.ACCEPTED)
        {
            Console.WriteLine("Server refused request with code: {0}", uploadResponse.Value);
            return;
        }
        Console.WriteLine("Server accepted request\nUploading started.");
        UploadFile();
        FinishResponse finishResponse = ReceiveFinishResponse();
        Console.WriteLine("Uploading finished with {0} status.", finishResponse.Value);
    }
}