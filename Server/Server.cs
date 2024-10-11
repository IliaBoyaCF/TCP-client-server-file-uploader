using System.Net;
using System.Net.Sockets;

namespace TCP_client_server_uploader.Server;

public class Server : IRunnable
{
    public static readonly int s_printInfoDelay = 3_000;
    private static int s_backLog = 10_000;

    private readonly Socket _serverSocket;
    private readonly int _port;
    private readonly List<IServerSession> _sessions = [];

    private readonly System.Timers.Timer _printerTimer = new(s_printInfoDelay);

    private readonly object _lock = new();

    private void PrintInfos()
    {
        if (_sessions.Count == 0)
        {
            return;
        }
        Console.WriteLine("----Download info----");
        lock (_lock)
        {
            _sessions.RemoveAll((session) =>
            {
                IServerSession.DownloadInfo info = session.GetDownloadInfo();
                Console.WriteLine("Downloading file from: {0}\n current speed: {1:N2}; average speed: {2:N2}; status: {3}",
                    session.GetRemoteAddress(), FormatSpeed(info.CurrentSpeed), FormatSpeed(info.AverageSessionSpeed), session.CurrentState);
                if (session.CurrentState == IServerSession.State.CLOSED)
                {
                    session.Dispose();
                    return true;
                }
                return false;
            });
        }
        Console.WriteLine();
    }

    public Server(int port)
    {
        _port = port;
        _serverSocket = new(SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
    }

    public bool IsRunning()
    {
        return _printerTimer.Enabled;
    }

    public void Run()
    {

        Console.WriteLine("Server started on port {0}, available interfaces to connect:", _port);

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress curAdd in host.AddressList)
        {
            Console.WriteLine(curAdd.ToString());
        }

        _printerTimer.Elapsed += (o, a) => PrintInfos();
        _printerTimer.AutoReset = true;
        _printerTimer.Enabled = true;
        _printerTimer.Start();

        _serverSocket.Listen(s_backLog);

        while (true)
        {
            Socket acceptedConnection = _serverSocket.Accept();
            ServerSession session = new(acceptedConnection);
            Thread thread = new(session.Start);
            thread.Start();
            lock (_lock)
            {
                _sessions.Add(session);
            }
        }
        Stop();
    }

    public void Stop()
    {
        foreach (var session in _sessions)
        {
            session.Dispose();
        }
        _serverSocket.Close();
    }

    private string FormatSpeed(double speed)
    {
        List<string> metrics = ["B/s", "KiB/s", "MiB/s", "GiB/s", "TiB/s"];
        string metric = metrics[0];
        int metricIndex = 0;
        while (speed > 1024 && metric != metrics[metrics.Count - 1])
        {
            speed /= 1024;
            metricIndex += 1;
            metric = metrics[metricIndex];
        }
        return speed + " " + metric;
    }
}
