namespace TCP_client_server_uploader.Client;

public class EntryPoint
{
    public static void Start(CommandLineArguments commandLineArguments)
    {
        ClientSession session = new(commandLineArguments);
        session.Start();
        session.Dispose();
    }
}
