using DiscordMusicBot.Application.Services;
using DiscordMusicBot.InfrastructureIoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordMusicBot.Console;

internal class Program
{
    public static async Task Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        var app = host.Services.GetService<BotService>();
        await app.StartAsync();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                DependencyContainer.RegisterServices(services);
            })
            .ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Error));

        return host;
    }
}
