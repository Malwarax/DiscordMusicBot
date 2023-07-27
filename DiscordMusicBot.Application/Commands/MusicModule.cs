using Discord;
using Discord.Commands;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Commands
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        private readonly MusicPlayerService _musicPlayerService;
        private readonly BotOptions _botOptions;
        private readonly YoutubeService _youtubeService;

        public MusicModule(MusicPlayerService musicPlayerService,
            IOptions<BotOptions> botOptions,
            YoutubeService youtubeService)
        {
            _musicPlayerService = musicPlayerService;
            _botOptions = botOptions.Value;
            _youtubeService = youtubeService;
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync(string? query = null)
        {
            var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;

            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel.");

                return;
            }

            if (string.IsNullOrEmpty(query))
            {
                await Context.Channel.SendMessageAsync($"The link is not provided. Use **{_botOptions.CommandPrefix}play <link>**");

                return;
            }

            var song = await _youtubeService.GetSongAsync(query);

            if (song == null)
            {
                await Context.Channel.SendMessageAsync("Unable to find a song :sob:");

                return;
            }

            await _musicPlayerService.PlayAsync(Context.Guild, voiceChannel, Context.Channel, song);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task StopAsync()
        {
            var result = await _musicPlayerService.StopAsync(Context.Guild);

            if (result)
            {
                await Context.Channel.SendMessageAsync(":sleeping:");
            }
        }

        [Command("skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            var result = _musicPlayerService.Skip(Context.Guild);

            if (result)
            {
                await Context.Channel.SendMessageAsync("The previous song was skipped.");

                return;
            }

            await Context.Channel.SendMessageAsync("Nothing to skip.");
        }
    }
}
