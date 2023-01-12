using System.Net.Sockets;
using vps.Interfaces;

namespace vps.Operations
{
    public class TcpOperation : ITcpOperation
    {
        private const int StartPort = 1024;
        private const int EndPort = 49151;

        public bool IsPortAvailable(int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.ConnectAsync("127.0.0.1", port).Wait(1000);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
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
