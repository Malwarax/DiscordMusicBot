using DiscordMusicBot.Domain.Models;
using VideoLibrary;

namespace DiscordMusicBot.Application.Services
{
    public class SongService
    {
        public async Task<SongModel> GetSongAsync(string url)
        {
            var cleanLink = GetCleanLink(url);

            if(string.IsNullOrEmpty(cleanLink))
            {
                return null;
            }

            try
            {
                var video = await YouTube.Default.GetVideoAsync(url);

                return new SongModel
                {
                    Title = video.Info.Title,
                    Url = cleanLink,
                    Author = video.Info.Author,
                };
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
                               && cleanLink.Contains("youtube");

            if (!isValidLink)
            {
                return string.Empty;
            }

            return cleanLink;
        }
    }
}
