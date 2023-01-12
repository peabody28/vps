using System.Net.Sockets;
using vps.Interfaces;

namespace vps.Operations
{
    public class TcpOperation : ITcpOperation
    {
        private const int StartPort = 1024;
        private const int EndPort = 49151;

        public bool IsPortAvailable(string host, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.ConnectAsync(host, port).Wait(1000);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int FreePort(string host)
        {
            for (var portNumber = StartPort; portNumber <= EndPort; portNumber++)
                if (IsPortAvailable(host, portNumber))
                    return portNumber;

            return -1;
        }

    }
}
