using Discord;
using Discord.Commands;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Commands
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly MusicHandler _musicHandler;
        private readonly LinkHandler _linkHandler;
        private readonly BotOptions _botOptions;

        public MusicModule(MusicHandler musicHandler,
            LinkHandler linkHandler,
            IOptions<BotOptions> botOptions)
        {
            _musicHandler = musicHandler;
            _linkHandler = linkHandler;
            _botOptions = botOptions.Value;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync(string? query = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                await Context.Channel.SendMessageAsync($"The link is not provided. Use **{_botOptions.CommandPrefix}play <link>**");
                return;
            }

            var song = await _linkHandler.GetQueueItemAsync(query);

            if (song == null)
            {
                await Context.Channel.SendMessageAsync($"Unable to find a song :sob:.");
                return;
            }

            var channel = (Context.User as IGuildUser)?.VoiceChannel;

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel.");
                return;
            }

            await _musicHandler.PlayAsync(Context.Guild, channel, song);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task StopAsync()
        {
            await Context.Channel.SendMessageAsync(":ok_hand:");
            await _musicHandler.StopAsync(Context.Guild);
        }
    }
}
