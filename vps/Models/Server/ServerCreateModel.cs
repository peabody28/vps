using Newtonsoft.Json;

namespace vps.Models.Server
{
    public class ServerCreateModel
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
