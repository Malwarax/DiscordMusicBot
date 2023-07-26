using Discord;
using Discord.WebSocket;

namespace DiscordMusicBot.Domain.Models
{
    public class QueueModel
    {
        public IVoiceChannel VoiceChannel { get; set; }
        public ISocketMessageChannel TextChannel { get; set; }
        public SongModel CurrentSong { get; set; }
        public Queue<SongModel> Items { get; set; }
    }
}
