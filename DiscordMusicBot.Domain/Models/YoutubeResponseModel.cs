using System.Text.Json.Serialization;

namespace DiscordMusicBot.Domain.Models
{
    public class YoutubeResponseModel
    {
        [JsonPropertyName("videoDetails")]
        public VideoDetailsModel VideoDetails { get; set; }

        public class VideoDetailsModel
        {
            [JsonPropertyName("title")]
            public string Title { get; set; }
            [JsonPropertyName("author")]
            public string Author { get; set; }
        }
    }
}
