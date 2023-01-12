using System.Net;
using System.Net.NetworkInformation;
using vps.Interfaces;

namespace vps.Operations
{
    public class TcpOperation : ITcpOperation
    {
        public bool IsPortAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port.Equals(port))
                    return false;
            }
            return true;
        }

        public int FreePort()
        {
            for (var portNumber = IPEndPoint.MinPort; portNumber < IPEndPoint.MaxPort; portNumber++)
                if (IsPortAvailable(portNumber))
                    return portNumber;

            return -1;
        }

    }
}
