using System.Net;
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
            var address = IPAddress.Parse(host);
            IPEndPoint myEP = new IPEndPoint(address, port);
            try
            {
                using Socket listeningSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(myEP);
                return true;
            }
            catch
            {
                return false;
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
