using Newtonsoft.Json;

namespace UCLBackend.Service.Data.Discord
{
    public class Embed
    {
        #nullable enable
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("type")]
        public string? Type { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("url")]
        public string? Url { get; set; }
        [JsonProperty("timestamp")]
        public string? TimeStamp { get; set; }
        [JsonProperty("color")]
        public int? Color { get; set; }
        [JsonProperty("footer")]
        public EmbedFooter? Footer { get; set; }
        [JsonProperty("author")]
        public EmbedAuthor? Author { get; set; }
        #nullable disable
    }

    public class EmbedFooter
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        #nullable enable
        [JsonProperty("icon_url")]
        public string? IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string? ProxyIconUrl { get; set; }
    }

    public class EmbedAuthor
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("url")]
        public string? Url { get; set; }
        [JsonProperty("icon_url")]
        public string? IconUrl { get; set; }
        [JsonProperty("proxy_icon_url")]
        public string? ProxyIconUrl { get; set; }
        #nullable disable
    }
}
