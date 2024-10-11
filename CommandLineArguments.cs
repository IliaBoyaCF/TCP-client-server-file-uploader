using System.Net;
using System.Runtime.CompilerServices;

namespace TCP_client_server_uploader;

public record CommandLineArguments(CommandLineArguments.WorkingMode Mode, FileInfo? FilePath, IPAddress Address, int Port, bool AskedForHelpInfo)
{
    public const string s_HelpInfo = """"
        Usage:
            > upload <flag> <args>
        Flags can be skipped as following:
            > upload file_path server_address server_port
        is the same as:
            > upload -c file_path server_address server_port
        Flags:
            -c Start program as client for uploading.
                After specifying this flag you have to pass the following arguments: file_path server_address server_port
            -s Start program as server for downloading files from clients.
                After specifying this flag you have to pass the following arguments: server_port
        Arguments:
            file_path: Specifying for client mode. 
                        The path to the file client wants to upload.
            server_address: Specifying for client mode. 
                            The hostname, IPv4, IPv6 address of the server to which client wants to upload file.
            server_port: In client mode: port of the server to which client wants to upload file. 
                        In server mode: port for listening for incoming connections.
        
        """";
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