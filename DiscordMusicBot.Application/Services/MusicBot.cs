using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Services
{
    public class MusicBot
    {
        private readonly BotOptions _botOptions;

        public MusicBot(IOptions<BotOptions> botOptions)
        {
            _botOptions = botOptions.Value;
        }

        public async Task StartAsync()
        {

        }
    }
}
