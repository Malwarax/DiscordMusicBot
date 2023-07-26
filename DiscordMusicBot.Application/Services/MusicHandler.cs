using Discord;
using Discord.Audio;
using Discord.WebSocket;
using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services
{
    public class MusicHandler
    {
        private readonly Dictionary<SocketGuild, QueueModel> activeSockets = new();

        private readonly AudioStreamHandler _audioStreamHandler;

        public MusicHandler(AudioStreamHandler audioStreamHandler)
        {
            _audioStreamHandler = audioStreamHandler;
        }

        public async Task PlayAsync(SocketGuild server,
            IVoiceChannel voiceChannel,
            ISocketMessageChannel textChannel,
            QueueItemModel song)
        {
            if (activeSockets.TryGetValue(server, out var queue))
            {
                queue.Items.Enqueue(song);

                return;
            }

            var newQueue = new QueueModel
            {
                VoiceChannel = voiceChannel,
                TextChannel = textChannel,
                Items = new Queue<QueueItemModel>(),
            };

            newQueue.Items.Enqueue(song);
            activeSockets.Add(server, newQueue);

            var audioClient = await voiceChannel.ConnectAsync();
            await HandleQueueAsync(audioClient, newQueue);
        }

        public async Task StopAsync(SocketGuild server)
        {
            if (activeSockets.TryGetValue(server, out var queue))
            {
                queue.Items.Clear();
                await queue.VoiceChannel.DisconnectAsync();
                activeSockets.Remove(server);
            }
        }

        private async Task HandleQueueAsync(IAudioClient audioClient, QueueModel queue)
        {
            while (queue.Items.Any())
            {
                queue.CurrentSong = queue.Items.Dequeue();
                string musicDescription = $"Now Playing: {queue.CurrentSong.Title} \n" +
                                          $"Author: {queue.CurrentSong.Author} \n" +
                                          $"URL: {queue.CurrentSong.Url}";
                await queue.TextChannel.SendMessageAsync(musicDescription);
                await _audioStreamHandler.SendAsync(audioClient, queue.CurrentSong.Url);
            }
        }
    }
}
