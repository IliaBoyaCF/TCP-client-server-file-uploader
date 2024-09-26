using ClientServerApp;
using System.Net;

namespace TCP_client_server_uploader;

public class Parser : IParser
{
    private CommandLineArguments? _arguments;

    public CommandLineArguments GetLastParsedArguments()
    {
        if (!HasParsedArguments())
        {
            throw new InvalidOperationException("Nothing has been parsed yet.");
        }
        return _arguments;
    }

    public bool HasParsedArguments()
    {
        return _arguments != null;
    }

    public CommandLineArguments ParseArguments(string[] args)
    {
        if (args == null)
        {
            throw new NullReferenceException("Args must be not null.");
        }
        switch (args.Length)
        {
            case 0:
                return CommandLineArguments.NoInput;
            case 1:
                {
                    if (args[0] == "--help" || args[0] == "-h")
                    {
                        return CommandLineArguments.AskedForHelp;
                    }
                    try
                    {
                        return CommandLineArguments.NewServerArguments(Convert.ToInt32(args[0]));
                    }
                    catch (Exception e) when (e is OverflowException || e is FormatException)
                    {
                        throw new IParser.ParserException(e.Message, e);
                    }
                }
            case 2:
                {
                    if (args[0] == "-s")
                    {
                        try
                        {
                            return CommandLineArguments.NewServerArguments(Convert.ToInt32(args[1]));
                        }
                        catch (Exception e) when (e is OverflowException || e is FormatException)
                        {
                            throw new IParser.ParserException(e.Message, e);
                        }
                    }
                    throw new IParser.ParserException("Invalid arguments. Try -h");
                }
            case 3:
                try
                {
                    return CommandLineArguments.NewClientArguments(new FileInfo(args[0]), IPAddress.Parse(args[1]), Convert.ToInt32(args[2]));
                }
                catch (Exception e) when (e is OverflowException || e is FormatException)
                {
                    throw new IParser.ParserException(e.Message, e);
                }
            case 4:
                {
                    if (args[0] == "-c")
                    {
                        try
                        {
                            return CommandLineArguments.NewClientArguments(new FileInfo(args[1]), IPAddress.Parse(args[2]), Convert.ToInt32(args[3]));
                        }
                        catch (Exception e) when (e is OverflowException || e is FormatException)
                        {
                            throw new IParser.ParserException(e.Message, e);
                        }
                    }
                    throw new IParser.ParserException("Invalid arguments. Try -h");
                }
            default:
                throw new IParser.ParserException("Invalid arguments. Try -h");
        }
    }
}