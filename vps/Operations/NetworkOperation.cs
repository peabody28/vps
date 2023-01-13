using System.Net;
using System.Net.Sockets;
using vps.Constants;
using vps.Interfaces;

namespace vps.Operations
{
    public class NetworkOperation : INetworkOperation
    {
        public IConfiguration Configuration { get; set; }

        public NetworkOperation(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public bool IsLocalPortAvailable(int port)
        {
            var address = IPAddress.Any;
            var myEP = new IPEndPoint(address, port);
            try
            {
                using var listeningSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(myEP);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int FreeLocalPort()
        {
            var startPort = Configuration.GetValue<int>("Server:StartPort");
            var endPort = Configuration.GetValue<int>("Server:EndPort");
            
            foreach(var portNumber in Enumerable.Range(startPort, endPort))
                if (IsLocalPortAvailable(portNumber))
                    return portNumber;

            return NetworkConstants.UnsupportedPort;
        }

    }
}
