namespace TCP_client_server_uploader.Server;

public class EntryPoint
{
    public static void Start(CommandLineArguments arguments)
    {
        Server server = new(arguments.Port);
        server.Run();
        return;
    }

}
