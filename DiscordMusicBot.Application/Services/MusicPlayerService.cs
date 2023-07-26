using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Application.Extensions;
using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services
{
    public class MusicPlayerService
    {
        private readonly Dictionary<SocketGuild, QueueModel> _activeServers = new();
        private readonly AudioStreamService _audioStreamService;

        public MusicPlayerService(AudioStreamService audioStreamService)
        {
            _audioStreamService = audioStreamService;
        }

        public async Task PlayAsync(SocketGuild server,
            IVoiceChannel voiceChannel,
            ISocketMessageChannel textChannel,
            SongModel song)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                queue.Items.Add(song);

                if (queue.IsBotActive)
                {
                    await queue.TextChannel.SendMessageAsync($"Added to the queue: {song.Title}");

                    return;
                }

                await HandleQueueAsync(queue);

                return;
            }

            var newQueue = new QueueModel
            {
                VoiceChannel = voiceChannel,
                TextChannel = textChannel,
                Items = new List<SongModel>(),
            };

            newQueue.Items.Add(song);
            _activeServers.Add(server, newQueue);

            var audioClient = await voiceChannel.ConnectAsync();
            newQueue.AudioClient = audioClient;

            audioClient.Disconnected += ex => HandleDisconnectedAsync(ex, server);

            await HandleQueueAsync(newQueue);
        }
        
        public async Task StopAsync(SocketGuild server)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                queue.Items.Clear();
                await queue.VoiceChannel.DisconnectAsync();
                _activeServers.Remove(server);
            }
        }

        public QueueModel GetQueue(SocketGuild server)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                return queue;
            }

            return null;
        }

        public bool ShuffleQueue(SocketGuild server)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                queue.Items.Shuffle();

                return true;
            }

            return false;
        }

        public bool RemoveItem(SocketGuild server, int position)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                if (position < 1 || position > queue.Items.Count)
                {
                    return false;
                }

                queue.Items.RemoveAt(position - 1);

                return true;
            }

            return false;
        }

        private async Task HandleQueueAsync(QueueModel queue)
        {
            queue.IsBotActive = true;

            while (queue.Items.Any())
            {
                queue.CurrentSong = queue.Items.First();
                queue.Items.RemoveAt(0);
                string songDescription = $"Now playing: {queue.CurrentSong.Title} \n"
                                         + $"Author: {queue.CurrentSong.Author} \n"
                                         + $"URL: {queue.CurrentSong.Url}";
                await queue.TextChannel.SendMessageAsync(songDescription);
                await _audioStreamService.SendAsync(queue.AudioClient, queue.CurrentSong.Url);
            }

            queue.CurrentSong = null;
            queue.IsBotActive = false;
        }

        private Task HandleDisconnectedAsync(Exception exception, SocketGuild server)
        {
            if (_activeServers.TryGetValue(server, out var queue))
            {
                _activeServers.Remove(server);
            }

            return Task.CompletedTask;
        }
    }
}
