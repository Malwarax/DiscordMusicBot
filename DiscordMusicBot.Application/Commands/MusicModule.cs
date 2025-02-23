using Discord;
using Discord.Commands;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Commands;

public class MusicModule(MusicPlayerService musicPlayerService,
    IOptions<BotOptions> botOptions,
    YoutubeService youtubeService) : ModuleBase<SocketCommandContext>
{
    [RequireContext(ContextType.Guild)]
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
            await Context.Channel.SendMessageAsync($"The link is not provided. Use **{botOptions.Value.CommandPrefix}play <link>**");

            return;
        }

        var song = await youtubeService.GetSongAsync(query);

        if (song == null)
        {
            await Context.Channel.SendMessageAsync("Unable to find a song :sob:");

            return;
        }

        await musicPlayerService.PlayAsync(Context.Guild, voiceChannel, Context.Channel, song);
    }

    [RequireContext(ContextType.Guild)]
    [Command("stop", RunMode = RunMode.Async)]
    public async Task StopAsync()
    {
        var result = await musicPlayerService.StopAsync(Context.Guild);

        if (result)
        {
            await Context.Channel.SendMessageAsync(":sleeping:");
        }
    }

    [RequireContext(ContextType.Guild)]
    [Command("skip", RunMode = RunMode.Async)]
    public async Task SkipAsync()
    {
        var result = musicPlayerService.Skip(Context.Guild);

        if (result)
        {
            await Context.Channel.SendMessageAsync("The previous song was skipped.");

            return;
        }

        await Context.Channel.SendMessageAsync("Nothing to skip.");
    }
}
