using vps.Models;

namespace vps.Interfaces
{
    public interface IDockerOperation
    {
        DockerContainerModel CreateContainer(string username, string password);
    }
}
