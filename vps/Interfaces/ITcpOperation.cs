namespace vps.Interfaces
{
    public interface ITcpOperation
    {
        bool IsPortAvailable(int port);

        int FreePort();
    }
}
