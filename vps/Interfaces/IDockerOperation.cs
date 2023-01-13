using vps.Models;

namespace vps.Interfaces
{
    public interface IDockerOperation
    {
        bool TryCreateContainer(string username, string password, out DockerContainerModel model);
    }
}
