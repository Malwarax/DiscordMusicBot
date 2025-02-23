using Discord.Commands;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Commands;

public class InfoModule(IOptions<BotOptions> botOptions) : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    public async Task HelpAsync()
    {
        string helpMessage = "Available commands: \n\n"
                             + $"Play a song / add a song to a queue: **{botOptions.Value.CommandPrefix}play <link>**\n"
                             + $"Skip a song: **{botOptions.Value.CommandPrefix}skip**\n"
                             + $"Stop the bot: **{botOptions.Value.CommandPrefix}stop**\n"
                             + $"Get a queue: **{botOptions.Value.CommandPrefix}queue**\n"
                             + $"Remove a song from a queue: **{botOptions.Value.CommandPrefix}queue-remove <position>**\n"
                             + $"Shuffle a queue: **{botOptions.Value.CommandPrefix}queue-shuffle**\n"
                             + $"Help: **{botOptions.Value.CommandPrefix}help**\n";

        await Context.Channel.SendMessageAsync(helpMessage);
    }
}
