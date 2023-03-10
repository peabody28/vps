using System.Net;
using System.Net.Sockets;
using vps.Constants;
using vps.Interfaces.Operations;

namespace vps.Operations
{
    public class NetworkOperation : INetworkOperation
    {
        public ILogger<NetworkOperation> Logger { get; set; }

        public IConfiguration Configuration { get; set; }

        public NetworkOperation(IConfiguration configuration, ILogger<NetworkOperation> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public bool IsLocalPortAvailable(int port)
        {
            var address = IPAddress.Any;
            var myEP = new IPEndPoint(address, port);
            try
            {
                Logger.LogInformation($"TRY BIND");

                using var listeningSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(myEP);
                Logger.LogInformation($"SUCCESS");

                return true;
            }
            catch(Exception ex)
            {
                Logger.LogInformation($" FAILD local port ({port}) binding exception "+ex.Message);
                return false;
            }
        }

        public int FreeLocalPort(IEnumerable<int> excludedPorts = null)
        {
            var startPort = Configuration.GetValue<int>("Server:StartPort");
            var endPort = Configuration.GetValue<int>("Server:EndPort");

            Logger.LogInformation($"RANGE FROM {startPort} to {endPort}");

            foreach (var portNumber in Enumerable.Range(startPort, endPort))
            {
                Logger.LogInformation($"PORT N {portNumber}");

                if (excludedPorts != null && excludedPorts.Contains(portNumber))
                    continue;

                if (IsLocalPortAvailable(portNumber))
                    return portNumber;
            }
               

            return NetworkConstants.UnsupportedPort;
        }

    }
}
