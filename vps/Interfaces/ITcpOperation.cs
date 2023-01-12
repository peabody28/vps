namespace vps.Interfaces
{
    public interface ITcpOperation
    {
        bool IsPortAvailable(string host, int port);

        int FreePort(string host);
    }
}
