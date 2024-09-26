using System.Net.Sockets;

namespace TCP_client_server_uploader;

public abstract class Session : ISession
{
    protected readonly Socket _socket;
    protected readonly IFileSystemOperator _fileSystemOperator;

    protected Session(Socket socket, IFileSystemOperator fileSystemOperator)
    {
        _socket = socket;
        _fileSystemOperator = fileSystemOperator;
    }

    public virtual void Dispose()
    {
        _socket.Dispose();
        _fileSystemOperator.Dispose();
    }

    protected byte[] ReceiveNBytes(long N)
    {
        byte[] buffer = new byte[N];
        int received = 0;
        while (received < buffer.Length)
        {
            received += _socket.Receive(buffer, received, (int)(N - received), SocketFlags.None);
        }
        return buffer;
    }

    protected void ReceiveNBytes(byte[] destinationBuffer, int offset, long N)
    {
        int received = 0;
        while (received < N)
        {
            received += _socket.Receive(destinationBuffer, offset + received, (int)(N - received), SocketFlags.None);
        }
    }

    public abstract void Start();
}
