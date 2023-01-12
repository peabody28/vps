using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http;
using vps.Interfaces;
using vps.Models;
using vps.Models.Server;

namespace vps.Controllers
{
    [ApiController]
    [System.Web.Http.Route("[controller]/[action]")]
    public class ServerController : ControllerBase
    {
        public IDockerOperation DockerOperation { get; set; }

        public ServerController(IDockerOperation dockerOperation) 
        {
            DockerOperation = dockerOperation;
        }

        [System.Web.Http.HttpPost]
        public DockerContainerModel Create(ServerCreateModel model)
        {
            var containerModel = DockerOperation.CreateContainer(model.Username, model.Password);
            if (containerModel == null)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            return containerModel;
        }
    }
}
