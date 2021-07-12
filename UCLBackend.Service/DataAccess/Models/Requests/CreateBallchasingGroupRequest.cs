using Newtonsoft.Json;

namespace UCLBackend.Discord.Requests
{
    public class CreateBallchasingGroupRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("player_identification")]
        public string PlayerIdentification { get; set; }
        [JsonProperty("team_identification")]
        public string TeamIdentification { get; set; }
    }
}
