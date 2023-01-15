using Microsoft.AspNetCore.Mvc;
using System.Net;
using vps.Interfaces.Operations;
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
        public DockerContainerModel Create(UserModel model)
        {
            var dockerContainer = DockerOperation.CreateContainer(model.Username, model.Password, out var isSuccess);
            if (!isSuccess)
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.InternalServerError);

            return new DockerContainerModel { SshPort = dockerContainer.SshPort, HttpPort = dockerContainer.HttpPort };
        }
    }
}
