using Newtonsoft.Json;

namespace vps.Models.Server
{
    public class UserModel
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
