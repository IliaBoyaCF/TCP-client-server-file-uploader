using TCP_client_server_uploader;

namespace ClientServerApp;

public interface IParser
{
    public class ParserException : Exception
    {
        public ParserException() : base() { }

        public ParserException(string message) : base(message) { }

        public ParserException(string message, Exception innerException) : base(message, innerException) { }
    }

    public CommandLineArguments GetLastParsedArguments();

    public bool HasParsedArguments();

    public CommandLineArguments ParseArguments(string[] args);
}