using Server.Protocol;
using System.Net;
using System.Net.Sockets;

namespace Client;

public class Session : ISession
{
    public ISession.State CurrentState => _currentState;

    private readonly Socket _socket;
    private readonly IFileSystemOperator _fileSystemOperator;

    private ISession.State _currentState = ISession.State.WAITING_FILE_TO_BE_ATTACHED;

    public Session(IPEndPoint server)
    {
        _socket = new(server.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _fileSystemOperator = new FileSystemOperator();
    }

    public void Close()
    {
        _socket.Close();
    }

    public FinishResponse ReceiveFinishResponse()
    {
        if (_currentState != ISession.State.WAITING_SERVER_RESPONSE)
        {
            throw new Exception("Finish response is not expected.");
        }
        FinishResponse response = MessageParser.ParseFinishResponse(ReceiveMessageFixedSize(FinishResponse.s_ConstSize));
        _currentState = ISession.State.READY_TO_SEND_UPLOAD_REQUEST;
        return response;
    }

    public UploadResponse ReceiveUploadResponse()
    {
        if (_currentState != ISession.State.WAITING_SERVER_RESPONSE)
        {
            throw new Exception("Upload response is not expected");
        }
        UploadResponse response = MessageParser.ParseUploadResponse(ReceiveMessageFixedSize(UploadResponse.s_ConstSize));
        if (response.Value != UploadResponse.UploadResponseValue.ACCEPTED) 
        {
            _currentState = ISession.State.READY_TO_SEND_UPLOAD_REQUEST;
        }
        else
        {
            _currentState = ISession.State.READY_TO_UPLOAD;
        }
        return response;
    }

    private byte[] ReceiveMessageFixedSize(int size)
    {
        byte[] buffer = new byte[size];
        int received = 0;
        while (received < buffer.Length)
        {
            received += _socket.Receive(buffer, size, SocketFlags.None);
        }
        return buffer;
    }

    public void SelectFileToUpload(string path)
    {
        _fileSystemOperator.OpenFile(path);
        _currentState = ISession.State.READY_TO_SEND_UPLOAD_REQUEST;
    }

    public void SendUploadRequest(UploadRequest uploadRequest)
    {
        if (_currentState != ISession.State.READY_TO_SEND_UPLOAD_REQUEST)
        {
            throw new Exception("Not ready to set an upload request");
        }
        _currentState = ISession.State.WAITING_SERVER_RESPONSE;
        _socket.Send(uploadRequest.Serialize());
    }

    public void UploadFile()
    {
        if (_currentState != ISession.State.READY_TO_UPLOAD)
        {
            throw new Exception("Session is not ready to upload file.");
        }
        const int bufferSize = 1024;
        byte[] buffer = new byte[bufferSize];
        long sentData = 0;
        Stream fileStream = _fileSystemOperator.GetInputStream();
        while (sentData != _fileSystemOperator.GetFileSize())
        {
            int readBytes = fileStream.Read(buffer);
            int currentSentData = 0;
            while (currentSentData != readBytes)
            {
                currentSentData += _socket.Send(buffer, currentSentData, readBytes - currentSentData, SocketFlags.None);
            }
            sentData += currentSentData;
        }
        _currentState = ISession.State.WAITING_SERVER_RESPONSE;
    }
}