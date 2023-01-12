using System.Net.NetworkInformation;
using vps.Interfaces;

namespace vps.Operations
{
    public class TcpOperation : ITcpOperation
    {
        private const int StartPort = 1024;
        private const int EndPort = 49151;
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
            for (var portNumber = StartPort; portNumber <= EndPort; portNumber++)
                if (IsPortAvailable(portNumber))
                    return portNumber;

            return -1;
        }

    }
}
