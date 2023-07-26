using DiscordMusicBot.Domain.Models;
using VideoLibrary;

namespace DiscordMusicBot.Application.Services
{
    public class LinkHandler
    {
        public async Task<QueueItemModel> GetQueueItemAsync(string link)
        {
            var cleanLink = GetCleanLink(link);

            if(string.IsNullOrEmpty(cleanLink))
            {
                return null;
            }

            try
            {
                var video = await YouTube.Default.GetVideoAsync(link);

                return new QueueItemModel
                {
                    SongName = video.Title,
                    SongUrl = cleanLink,
                };
            }
            catch
            {
                return null;
            }
        }

        private string GetCleanLink(string link)
        {
            var isLink = Uri.TryCreate(link, UriKind.Absolute, out Uri uri);

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
