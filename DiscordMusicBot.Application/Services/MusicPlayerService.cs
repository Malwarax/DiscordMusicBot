﻿using Discord;
using Discord.WebSocket;
using DiscordMusicBot.Application.Extensions;
using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services;

public class MusicPlayerService(AudioStreamService audioStreamService)
{
    private readonly Dictionary<SocketGuild, QueueModel> _activeServers = new();

    public async Task PlayAsync(SocketGuild server,
        IVoiceChannel voiceChannel,
        ISocketMessageChannel textChannel,
        SongModel song)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            await AddNewQueueAsync(server, voiceChannel, textChannel, song);

            return;
        }

        queue.Items.Add(song);

        if (queue.IsBotActive)
        {
            await queue.TextChannel.SendMessageAsync($"Added to the queue: {song.Title}");

            return;
        }

        await HandleQueueAsync(queue);
    }
    
    public async Task<bool> StopAsync(SocketGuild server)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            return false;
        }

        await queue.VoiceChannel.DisconnectAsync();
        _activeServers.Remove(server);

        return true;
    }

    public bool Skip(SocketGuild server)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            return false;
        }

        if (queue.IsBotActive == false || queue.BaseAudioStream == null)
        {
            return false;
        }

        queue.BaseAudioStream.Close();

        return true;
    }

    public QueueModel GetQueue(SocketGuild server)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            return null;
        }

        return queue;
    }

    public bool ShuffleQueue(SocketGuild server)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            return false;
        }

        queue.Items.Shuffle();

        return true;
    }

    public bool RemoveItem(SocketGuild server, int position)
    {
        if (!_activeServers.TryGetValue(server, out var queue))
        {
            return false;
        }

        if (position < 1 || position > queue.Items.Count)
        {
            return false;
        }

        queue.Items.RemoveAt(position - 1);

        return true;
    }

    private async Task AddNewQueueAsync(SocketGuild server,
        IVoiceChannel voiceChannel,
        ISocketMessageChannel textChannel,
        SongModel song)
    {
        var newQueue = new QueueModel
        {
            VoiceChannel = voiceChannel,
            TextChannel = textChannel,
            Items = new List<SongModel>(),
        };

        newQueue.Items.Add(song);

        try
        {
            var audioClient = await voiceChannel.ConnectAsync(selfDeaf: true);
            newQueue.AudioClient = audioClient;
            audioClient.Disconnected += ex => HandleDisconnectedAsync(ex, server);
        }
        catch
        {
            await textChannel.SendMessageAsync($"Can't connect to the voice channel.");

            return;
        }

        _activeServers.Add(server, newQueue);

        await HandleQueueAsync(newQueue);
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

            await SendNextSongDescriptionAsync(queue, songDescription);
            await audioStreamService.SendAsync(queue);
        }

        queue.CurrentSong = null;
        queue.IsBotActive = false;
    }

    private static async Task SendNextSongDescriptionAsync(QueueModel queue, string songDescription)
    {
        try
        {
            await queue.TextChannel.SendMessageAsync(songDescription);
        }
        catch
        {
            try
            {
                await queue.VoiceChannel.SendMessageAsync(songDescription);
                queue.TextChannel = (ISocketMessageChannel)queue.VoiceChannel;
            }
            catch
            {
                Console.WriteLine(
                    $"[General/Error] {DateTime.Now:hh:mm:ss}\tThe bot can't send the next song description");
            }
        }
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
