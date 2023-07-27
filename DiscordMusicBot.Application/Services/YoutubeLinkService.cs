using System.Text.RegularExpressions;

namespace DiscordMusicBot.Application.Services
{
    public class YoutubeLinkService
    {
        private const string VideoIdPattern = @".+\/(?>watch\?v=|.{0})(.+)";

        public string GetVideoId(string url)
        {
            Match idMath = Regex.Match(url, VideoIdPattern);

            if (idMath.Success)
            {
                return idMath.Groups[1].Value;
            }

            return string.Empty;
        }

        public string GetCleanLink(string url)
        {
            var isLink = Uri.TryCreate(url, UriKind.Absolute, out var uri);

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
