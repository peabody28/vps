namespace vps.Interfaces
{
    public interface INetworkOperation
    {
        bool IsLocalPortAvailable(int port);

        int FreeLocalPort();
    }
}
