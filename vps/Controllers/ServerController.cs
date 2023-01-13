using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        public DockerContainerModel Create(UserModel model)
        {
            var isContainerReady = DockerOperation.TryCreateContainer(model.Username, model.Password, out var dockerContainerModel);
            if (!isContainerReady)
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.InternalServerError);

            return dockerContainerModel;
        }
    }
}
