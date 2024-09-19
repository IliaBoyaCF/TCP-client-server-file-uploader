using System.Net;

namespace Client;

public class EntryPoint
{
    public static void Main(string[] args)
    {
        IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);

        ISession session = new Session(server);
    }
}
