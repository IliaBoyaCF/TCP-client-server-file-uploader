using System.Net;

namespace TCP_client_server_uploader;

public record CommandLineArguments(CommandLineArguments.WorkingMode Mode, FileInfo? FilePath, IPAddress Address, int Port, bool AskedForHelpInfo)
{
    public enum WorkingMode
    {
        CLIENT,
        SERVER,
    }
    public static readonly int DefaultPort = 30000;

    public static readonly CommandLineArguments AskedForHelp = new(WorkingMode.CLIENT, null, IPAddress.Any, DefaultPort, true);
    public static readonly CommandLineArguments NoInput = AskedForHelp;

    public static CommandLineArguments NewClientArguments(FileInfo FilePath, IPAddress serverAddress, int serverPort)
    {
        return new CommandLineArguments(WorkingMode.CLIENT, FilePath, serverAddress, serverPort, false);
    }

    public static CommandLineArguments NewServerArguments(int port)
    {
        return new CommandLineArguments(WorkingMode.SERVER, null, IPAddress.Any, port, false);
    }

}