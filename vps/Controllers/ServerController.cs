using Microsoft.AspNetCore.Mvc;
using vps.Interfaces;
using vps.Models;
using vps.Models.Server;

namespace vps.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ServerController : ControllerBase
    {
        public IDockerOperation DockerOperation { get; set; }

        public ServerController(IDockerOperation dockerOperation) 
        {
            DockerOperation = dockerOperation;
        }

        [HttpPost]
        public DockerContainerModel Create(ServerCreateModel model)
        {
            var containerModel = DockerOperation.CreateContainer(model.Username, model.Password);

            return containerModel;
        }
    }
}
