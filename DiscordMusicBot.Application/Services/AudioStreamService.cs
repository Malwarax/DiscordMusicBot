using Discord.Audio;
using System.Diagnostics;

namespace DiscordMusicBot.Application.Services
{
    public class AudioStreamService
    {
        public async Task SendAsync(IAudioClient client, string link)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C yt-dlp -f bestaudio -o - \"{link}\" | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            using (var ffmpeg = Process.Start(psi))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Music))
            {
                try
                {
                    await output.CopyToAsync(discord);
                }
                finally
                {
                    await discord.FlushAsync();
                    ffmpeg.Close();
                }
            }
        }
    }
}
