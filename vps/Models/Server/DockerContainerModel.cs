using Newtonsoft.Json;

namespace vps.Models.Server
{
    public class DockerContainerModel
    {
        [JsonProperty("sshPort")]
        public int SshPort { get; set; }

        [JsonProperty("httpPort")]
        public int? HttpPort { get; set; }
    }
}