﻿using ClientServerApp.Protocol;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace TCP_client_server_uploader.Server;

public class ServerSession : Session, IServerSession
{

    private class DownloadSpeedometer
    {
        private readonly Stopwatch stopwatch = new();

        private double _downloadStartTime;
        private double _downloadEndTime;

        private double _lastReceivedBytesTime;

        private double _totalReceivedBytes;
        private double _lastReceivedBytes;

        public void Start()
        {
            stopwatch.Start();
            _downloadStartTime = stopwatch.Elapsed.TotalMicroseconds;
            _lastReceivedBytesTime = _downloadStartTime;
        }

        public void GotBytes(long bytes)
        {
            _lastReceivedBytesTime = stopwatch.Elapsed.TotalMicroseconds;
            _lastReceivedBytes = bytes;
            _totalReceivedBytes += bytes;
        }

        public IServerSession.DownloadInfo GetDownloadInfo()
        {
            double currentTime;
            if (_downloadEndTime != 0)
            {
                currentTime = _downloadEndTime;
            }
            else
            {
                currentTime = stopwatch.Elapsed.TotalMicroseconds;
            }
            double currentInterval = currentTime - _lastReceivedBytesTime;
            double totalInterval = currentTime - _downloadStartTime;

            const double microsecToSecFactor = 1_000.0 * 1_000.0;

            double currentSpeed = currentInterval == 0 ? double.PositiveInfinity : microsecToSecFactor * _lastReceivedBytes / currentInterval;
            double averageSpeed = totalInterval == 0 ? double.PositiveInfinity : microsecToSecFactor * _totalReceivedBytes / totalInterval;
            return new IServerSession.DownloadInfo(currentSpeed,
                averageSpeed);
        }

        public bool IsFinished()
        {
            return _downloadEndTime != 0.0;
        }

        public void Stop()
        {
            _downloadEndTime = stopwatch.Elapsed.TotalMicroseconds; ;
            stopwatch.Stop();
        }

    }
    public IServerSession.State CurrentState => _currentState;

    private readonly EndPoint _remoteEndPoint;

    private IServerSession.State _currentState = IServerSession.State.WAITING_UPLOAD_REQUEST;

    private DownloadSpeedometer? _downloadSpeedometer;

    public ServerSession(Socket socket) : base(socket, new ServerFileSystemOperator())
    {
        EndPoint? remoteEndPoint = _socket.RemoteEndPoint;
        if (remoteEndPoint == null)
        {
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        }
        _remoteEndPoint = remoteEndPoint;
        Console.WriteLine("Connected with {0}.", socket.RemoteEndPoint);
    }

    public override void Start()
    {
        UploadRequest uploadRequest = ReceiveUploadRequest();
        _fileSystemOperator.SelectFile(new FileInfo(uploadRequest.FileName));
        if (!((ServerFileSystemOperator)_fileSystemOperator).CanSaveFile(uploadRequest.FileSize))
        {
            SendUploadResponse(new UploadResponse(UploadResponse.UploadResponseValue.NO_SPACE));
            Dispose();
            return;
        }
        SendUploadResponse(new UploadResponse(UploadResponse.UploadResponseValue.ACCEPTED));
        _currentState = IServerSession.State.DOWNLOADING_FILE;
        FinishResponse finishResponse = DownloadFile(uploadRequest);
        SendFinishResponse(finishResponse);
        Dispose();
    }

    public override void Dispose()
    {
        base.Dispose();
        _currentState = IServerSession.State.CLOSED;
    }

    public UploadRequest ReceiveUploadRequest()
    {
        ArrayBufferWriter<byte> arrayBufferWriter = new();
        byte[] fileNameLengthBytes = ReceiveMessageFixedSize(sizeof(short));
        int fileNameLength = BitConverter.ToInt16(fileNameLengthBytes);
        arrayBufferWriter.Write(fileNameLengthBytes);

        arrayBufferWriter.Write(ReceiveMessageFixedSize(fileNameLength + sizeof(long)));

        return MessageParser.ParseUploadRequest(arrayBufferWriter.WrittenSpan.ToArray());
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

    public void SendUploadResponse(UploadResponse response)
    {
        SendBytes(response.Serialize());
    }

    public void SendFinishResponse(FinishResponse finishResponse)
    {
        SendBytes(finishResponse.Serialize());
        Dispose();
    }

    private void SendBytes(byte[] bytes)
    {
        int sentBytes = 0;
        while (sentBytes < bytes.Length)
        {
            sentBytes += _socket.Send(bytes, sentBytes, bytes.Length - sentBytes, SocketFlags.None);
        }
    }

    public IServerSession.DownloadInfo GetDownloadInfo()
    {
        if (_downloadSpeedometer == null)
        {
            throw new InvalidOperationException("Downloading is not started.");
        }
        return _downloadSpeedometer.GetDownloadInfo();
    }

    public FinishResponse DownloadFile(UploadRequest request)
    {
        if (_currentState != IServerSession.State.DOWNLOADING_FILE)
        {
            throw new Exception("Session is not ready to download file.");
        }
        const int bufferSize = 1024;
        byte[] buffer = new byte[bufferSize];
        long receivedData = 0;
        using Stream fileStream = _fileSystemOperator.GetStream(IFileSystemOperator.Mode.WRITE);
        DownloadSpeedometer speedometer = new();
        _downloadSpeedometer = speedometer;
        speedometer.Start();
        while (receivedData != request.FileSize)
        {
            int currentReceivedData = _socket.Receive(buffer);
            speedometer.GotBytes(currentReceivedData);
            receivedData += currentReceivedData;
            fileStream.Write(buffer, 0, currentReceivedData);
        }
        _currentState = IServerSession.State.WAITING_UPLOAD_REQUEST;
        FinishResponse finishResponse = new(_fileSystemOperator.GetFileSize() == request.FileSize ? FinishResponse.FinishResponseValue.SUCCESS : FinishResponse.FinishResponseValue.FAILURE);
        return finishResponse;
    }

    public EndPoint GetRemoteAddress() => _remoteEndPoint;

}