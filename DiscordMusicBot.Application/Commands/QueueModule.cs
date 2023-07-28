using Discord.Commands;
using DiscordMusicBot.Application.Services;
using DiscordMusicBot.Domain.Configuration;
using Microsoft.Extensions.Options;
using System.Text;

namespace DiscordMusicBot.Application.Commands
{
    public class QueueModule : ModuleBase<SocketCommandContext>
    {
        private readonly MusicPlayerService _musicPlayerService;
        private readonly BotOptions _botOptions;

        public QueueModule(MusicPlayerService musicPlayerService,
            IOptions<BotOptions> botOptions)
        {
            _musicPlayerService = musicPlayerService;
            _botOptions = botOptions.Value;
        }

        [RequireContext(ContextType.Guild)]
        [Command("queue", RunMode = RunMode.Async)]
        public async Task QueueAsync()
        {
            var queue = _musicPlayerService.GetQueue(Context.Guild);

            if (queue == null)
            {
                await Context.Channel.SendMessageAsync("The queue doesn't exist.");

                return;
            }

            if (queue.CurrentSong == null && queue.Items.Any() == false)
            {
                await Context.Channel.SendMessageAsync("The queue is empty.");

                return;
            }

            var stringBuilder = new StringBuilder();

            if (queue.CurrentSong != null)
            {
                stringBuilder.AppendLine($"Current song: {queue.CurrentSong.Title}");
            }

            if (queue.Items.Any())
            {
                stringBuilder.AppendLine("Queue:");

                for (int i = 0; i < queue.Items.Count; i++)
                {
                    stringBuilder.AppendLine($"{i + 1}.\t{queue.Items[i].Title}");
                }
            }

            await Context.Channel.SendMessageAsync(stringBuilder.ToString());
        }

        [RequireContext(ContextType.Guild)]
        [Command("queue-remove", RunMode = RunMode.Async)]
        public async Task RemoveAsync(int? position = null)
        {
            if (!position.HasValue || position < 1)
            {
                await Context.Channel.SendMessageAsync(
                    $"The position is not provided or invalid. Use **{_botOptions.CommandPrefix}queue-remove <position in a queue>**");
            }

            var result = _musicPlayerService.RemoveItem(Context.Guild, position.Value);

            if (result)
            {
                await Context.Channel.SendMessageAsync("The song was removed.");

                return;
            }

            await Context.Channel.SendMessageAsync(
                $"Something went wrong. Use **{_botOptions.CommandPrefix}queue-remove <position in a queue>**");
        }

        [RequireContext(ContextType.Guild)]
        [Command("queue-shuffle", RunMode = RunMode.Async)]
        public async Task ShuffleAsync()
        {
            var result = _musicPlayerService.ShuffleQueue(Context.Guild);

            if (result)
            {
                await Context.Channel.SendMessageAsync("The queue was shuffled.");

                return;
            }

            await Context.Channel.SendMessageAsync("The queue doesn't exist.");
        }
    }
}
