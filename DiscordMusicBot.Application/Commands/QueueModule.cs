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

        [Command("queue")]
        public async Task QueueAsync()
        {
            var queue = _musicPlayerService.GetQueue(Context.Guild);

            if (queue == null || queue.Items?.Any() != true)
            {
                await Context.Channel.SendMessageAsync($"The queue is empty or doesn't exist.");
                return;
            }

            var stringBuilder = new StringBuilder();

            if (queue.CurrentSong != null)
            {
                stringBuilder.AppendLine($"Current song: {queue.CurrentSong.Title}");
            }

            stringBuilder.AppendLine("Queue:");

            for (int i = 0; i < queue.Items.Count; i++)
            {
                stringBuilder.AppendLine($"{i + 1}.\t{queue.Items[i].Title}");
            }

            await Context.Channel.SendMessageAsync(stringBuilder.ToString());
        }

        [Command("queue-remove")]
        public async Task RemoveAsync(int? position = null)
        {
            if (position == null || position < 1)
            {
                await Context.Channel.SendMessageAsync($"The position not provided or invalid. Use **{_botOptions.CommandPrefix}queue-remove <position in a queue>**");
            }

            var result = _musicPlayerService.RemoveItem(Context.Guild, position.Value);

            if (result)
            {
                await Context.Channel.SendMessageAsync($"The song was removed.");
                return;
            }

            await Context.Channel.SendMessageAsync($"Something went wrong. Use **{_botOptions.CommandPrefix}queue-remove <position in a queue>**");
        }

        [Command("queue-shuffle")]
        public async Task ShuffleAsync()
        {
            var result = _musicPlayerService.ShuffleQueue(Context.Guild);

            if (result)
            {
                await Context.Channel.SendMessageAsync($"The queue was shuffled.");
                return;
            }

            await Context.Channel.SendMessageAsync($"The queue doesn't exist.");
        }
    }
}
