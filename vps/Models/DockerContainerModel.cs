using Newtonsoft.Json;

namespace vps.Models
{
    public class DockerContainerModel
    {
        [JsonProperty("sshPort")]
        public int? SshPort { get; set; }

        [JsonProperty("tcpPort")]
        public int? TcpPort { get; set; }
    }
}
