using Discord;
using Discord.Audio;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services
{
    public class MusicHandler
    {
        private readonly Dictionary<SocketGuild, QueueModel> activeSockets = new();

        public async Task PlayAsync(SocketGuild server, IVoiceChannel channel, QueueItemModel song)
        {
            IAudioClient audioClient;

            if (activeSockets.TryGetValue(server, out var queue))
            {
                queue.Items.Enqueue(song);

                if (queue.ActiveVoiceChannel != channel)
                {
                    queue.ActiveVoiceChannel = channel;
                    audioClient = await channel.ConnectAsync();
                }

                return;
            }

            var newQueue = new QueueModel()
            {
                ActiveVoiceChannel = channel,
                Items = new Queue<QueueItemModel>(),
            };

            newQueue.Items.Enqueue(song);
            activeSockets.Add(server, newQueue);

            audioClient = await channel.ConnectAsync();
        }

        public async Task StopAsync(SocketGuild server)
        {
            if (activeSockets.TryGetValue(server, out var queue))
            {
                queue.Items.Clear();
                await queue.ActiveVoiceChannel.DisconnectAsync();
                activeSockets.Remove(server);
            }
        }
    }
}
