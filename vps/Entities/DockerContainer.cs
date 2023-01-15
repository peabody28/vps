using vps.Interfaces.Entities;

namespace vps.Models
{
    public class DockerContainer : IDockerContainer
    {
        public string Name { get; set; }

        public int SshPort { get; set; }

        public int? HttpPort { set; get; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
