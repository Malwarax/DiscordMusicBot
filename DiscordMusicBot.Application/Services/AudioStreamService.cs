using Discord.Audio;
using System.Diagnostics;
using DiscordMusicBot.Domain.Models;

namespace DiscordMusicBot.Application.Services
{
    public class AudioStreamService
    {
        public async Task SendAsync(QueueModel queue)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C yt-dlp -f bestaudio -o - \"{queue.CurrentSong.Url}\" | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            using (var ffmpeg = Process.Start(startInfo))
            using (var outputStream = ffmpeg.StandardOutput.BaseStream)
            using (var discordStream = queue.AudioClient.CreatePCMStream(AudioApplication.Music))
            {
                queue.BaseAudioStream = outputStream;

                try
                {
                    await outputStream.CopyToAsync(discordStream);
                }
                catch
                {
                    Console.WriteLine($"[General/Info] {DateTime.Now:hh:mm:ss}\tThe ffmpeg stream was closed");
                }
                finally
                {
                    await discordStream.FlushAsync();
                    ffmpeg.Close();
                }
            }
        }
    }
}
