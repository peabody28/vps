using vps.Interfaces.Entities;

namespace vps.Interfaces.Operations
{
    public interface IDockerOperation
    {
        IDockerContainer CreateContainer(string username, string password, out bool isSuccess);
    }
}
