using System.Net;

namespace Program;

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
        if (args.Length == 0)
        {
            return CommandLineArguments.NoInput;
        }
        if (args.Length == 1)
        {
            if (args[0] == "--help" || args[0] == "-h")
            {
                return CommandLineArguments.AskedForHelp;
            }
        }
        if (args.Length != 3)
        {
            throw new ArgumentException("Illegal number of arguments. Try '--help'");
        }
        try 
        {
            return CommandLineArguments.NewCommandLineArguments(args[0], args[1], Convert.ToInt32(args[2]), false); 
        }
        catch (Exception e) when (e is OverflowException || e is FormatException) 
        {
            throw new IParser.ParserException(e.Message, e);
        }
    }
}