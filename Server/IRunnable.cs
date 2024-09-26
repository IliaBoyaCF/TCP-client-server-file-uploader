namespace TCP_client_server_uploader.Server;

public interface IRunnable
{
    public void Run();
    public bool IsRunning();
    public void Stop();
}