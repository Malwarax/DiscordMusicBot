using System.Text.Json.Serialization;

namespace DiscordMusicBot.Domain.Models;

public class YoutubeRequestModel
{
    [JsonPropertyName("videoId")]
    public string VideoId { get; set; }
    [JsonPropertyName("context")]
    public ContextModel Context { get; set; }

    public class ContextModel
    {
        [JsonPropertyName("client")]
        public ClientModel Client { get; set; }

        public class ClientModel
        {
            [JsonPropertyName("clientName")]
            public string ClientName { get; set; }
            [JsonPropertyName("clientVersion")]
            public string ClientVersion { get; set; }
        }
    }
}
