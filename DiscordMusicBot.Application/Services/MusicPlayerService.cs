using Discord;
using Discord.Audio;
using Discord.WebSocket;
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
                queue.Items.Enqueue(song);
                await queue.TextChannel.SendMessageAsync($"Added to the queue: {song.Title}");
                return;
            }

            var newQueue = new QueueModel
            {
                VoiceChannel = voiceChannel,
                TextChannel = textChannel,
                Items = new Queue<SongModel>(),
            };

            newQueue.Items.Enqueue(song);
            _activeServers.Add(server, newQueue);

            var audioClient = await voiceChannel.ConnectAsync();
            await HandleQueueAsync(audioClient, newQueue);
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

        private async Task HandleQueueAsync(IAudioClient audioClient, QueueModel queue)
        {
            while (queue.Items.Any())
            {
                queue.CurrentSong = queue.Items.Dequeue();
                string songDescription = $"Now playing: {queue.CurrentSong.Title} \n" +
                                          $"Author: {queue.CurrentSong.Author} \n" +
                                          $"URL: {queue.CurrentSong.Url}";
                await queue.TextChannel.SendMessageAsync(songDescription);
                await _audioStreamService.SendAsync(audioClient, queue.CurrentSong.Url);
            }
        }
    }
}
