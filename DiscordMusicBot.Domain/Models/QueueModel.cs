using Discord;
using Discord.WebSocket;

namespace DiscordMusicBot.Domain.Models
{
    public class QueueModel
    {
        public IVoiceChannel VoiceChannel { get; set; }
        public ISocketMessageChannel TextChannel { get; set; }
        public QueueItemModel CurrentSong { get; set; }
        public Queue<QueueItemModel> Items { get; set; }
    }
}
