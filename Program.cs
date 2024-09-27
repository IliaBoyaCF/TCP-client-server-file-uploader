using ClientServerApp;

namespace TCP_client_server_uploader;

public class Program
{
    public static void Start(string[] args)
    {
        IParser parser = new Parser();
        CommandLineArguments commandLineArguments = parser.ParseArguments(args);
        if (commandLineArguments.AskedForHelpInfo)
        {
            Console.WriteLine(CommandLineArguments.s_HelpInfo);
            return;
        }
        switch (commandLineArguments.Mode)
        {
            case CommandLineArguments.WorkingMode.CLIENT:
                Client.EntryPoint.Start(commandLineArguments);
                break;
            case CommandLineArguments.WorkingMode.SERVER:
                Server.EntryPoint.Start(commandLineArguments);
                break;
            default:
                throw new ArgumentException("Unknown working mode");
        }
    }
}

