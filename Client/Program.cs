using Program;
using Server.Protocol;
using System.Net;

namespace Client;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        IParser parser = new Parser();
        CommandLineArguments commandLineArguments = parser.ParseArguments(args);
        if (commandLineArguments.AskedForHelpInfo)
        {
            Console.WriteLine("Asked for help");
            return;
        }
        if (!File.Exists(commandLineArguments.filePath))
        {
            Console.Error.WriteLine("File not found");
            
            return;
        }
        IPAddress serverAddress;
        try
        {
            serverAddress = IPAddress.Parse(commandLineArguments.Address);
        }
        catch (FormatException e)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(commandLineArguments.Address);
            if (addresses.Length == 0)
            {
                Console.Error.WriteLine("Invalid server address.");
                return;
            }
            serverAddress = addresses[0];
        }
        Session session = new Session(new IPEndPoint(serverAddress, commandLineArguments.Port));
        session.SelectFileToUpload(commandLineArguments.filePath);
        FileInfo fileInfo = new FileInfo(commandLineArguments.filePath);
        session.SendUploadRequest(new Server.Protocol.UploadRequest(fileInfo.Name, fileInfo.Length));
        UploadResponse uploadResponse = session.ReceiveUploadResponse();
        if (uploadResponse.Value != UploadResponse.UploadResponseValue.ACCEPTED)
        {
            Console.Error.WriteLine("Upload refused by server.");
            return;
        }
        session.UploadFile();
        FinishResponse finishResponse = session.ReceiveFinishResponse();
        Console.WriteLine("Uploading finished with {0} status.", finishResponse.Value);
        return;
    }
}
