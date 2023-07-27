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
        private readonly SongService _songService;
        private readonly BotOptions _botOptions;

        public MusicModule(MusicPlayerService musicPlayerService,
            SongService songService,
            IOptions<BotOptions> botOptions)
        {
            _musicPlayerService = musicPlayerService;
            _songService = songService;
            _botOptions = botOptions.Value;
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

            var song = await _songService.GetSongAsync(query);

            if (song == null)
            {
                await Context.Channel.SendMessageAsync($"Unable to find a song :sob:");

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
            var result = await _musicPlayerService.SkipAsync(Context.Guild);

            if (result)
            {
                await Context.Channel.SendMessageAsync("The previous song was skipped.");

                return;
            }

            await Context.Channel.SendMessageAsync("Nothing to skip.");
        }
    }
}
