using Discord.Commands;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordMusicBot.Application.Commands
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly BotOptions _botOptions;

        public InfoModule(IOptions<BotOptions> botOptions)
        {
            _botOptions = botOptions.Value;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            string helpMessage = "Available commands: \n\n"
                                 + $"Play a song / add a song to a queue: **{_botOptions.CommandPrefix}play <link>**\n"
                                 + $"Skip a song: **{_botOptions.CommandPrefix}skip**\n"
                                 + $"Stop the bot: **{_botOptions.CommandPrefix}stop**\n"
                                 + $"Get a queue: **{_botOptions.CommandPrefix}queue**\n"
                                 + $"Remove a song from a queue: **{_botOptions.CommandPrefix}queue-remove <position>**\n"
                                 + $"Shuffle a queue: **{_botOptions.CommandPrefix}queue-shuffle**\n"
                                 + $"Help: **{_botOptions.CommandPrefix}help**\n";

            await Context.Channel.SendMessageAsync(helpMessage);
        }
    }
}
