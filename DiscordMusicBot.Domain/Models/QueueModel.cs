using Discord;

namespace DiscordMusicBot.Domain.Models
{
    public class QueueModel
    {
        public IVoiceChannel ActiveVoiceChannel { get; set; }
        public Queue<QueueItemModel> Items { get; set; }
    }
}
