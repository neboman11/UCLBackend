using System.Collections.Generic;
using Newtonsoft.Json;

namespace UCLBackend.Service.Data.Discord
{
    public class Message
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("embeds")]
        public List<Embed> Embeds { get; set; }
    }
}
