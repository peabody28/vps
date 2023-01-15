namespace vps.Interfaces.Operations
{
    public interface INetworkOperation
    {
        bool IsLocalPortAvailable(int port);

        int FreeLocalPort(IEnumerable<int> excludedPorts = null);
    }
}
