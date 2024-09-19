using System.Net;

namespace Program;

public record CommandLineArguments(string filePath, string Address, int Port, bool AskedForHelpInfo)
{
    public static readonly string s_defaultAddress = "";
    public static readonly string s_defaultFilePath = "";
    public static readonly int DefaultPort = 20000;

    public static readonly CommandLineArguments AskedForHelp = new(s_defaultFilePath, s_defaultAddress, DefaultPort, true);
    public static readonly CommandLineArguments NoInput = AskedForHelp;
    public static CommandLineArguments NewCommandLineArguments(string filePath, string Address, int Port, bool AskedForHelpInfo)
    {
        return new CommandLineArguments(filePath, Address, Port, AskedForHelpInfo);
    }
}