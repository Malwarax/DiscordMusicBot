using Discord.Commands;

namespace DiscordMusicBot.Application.Commands
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        public async Task SayAsync(string echo)
            => await ReplyAsync(echo);
    }
}
