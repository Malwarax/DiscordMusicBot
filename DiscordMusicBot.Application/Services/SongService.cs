using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services
{
    public class SongService
    {
        private readonly YoutubeService _youtubeService;

        public SongService(YoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<SongModel> GetSongAsync(string url)
        {
            var cleanLink = GetCleanLink(url);

            if(string.IsNullOrEmpty(cleanLink))
            {
                return null;
            }

            try
            {
                return await _youtubeService.GetSongInfoAsync(cleanLink);
            }
            catch
            {
                return null;
            }
        }

        private string GetCleanLink(string url)
        {
            var isLink = Uri.TryCreate(url, UriKind.Absolute, out Uri uri);

            if (!isLink)
            {
                return string.Empty;
            }

            var cleanLink = uri.ToString()
                .Split('&')
                .FirstOrDefault(); 

            bool isValidLink = string.IsNullOrEmpty(cleanLink) == false
                               && (cleanLink.Contains("youtube") 
                                   || cleanLink.Contains("youtu.be"));

            if (!isValidLink)
            {
                return string.Empty;
            }

            return cleanLink;
        }
    }
}
