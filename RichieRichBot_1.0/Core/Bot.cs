using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace RichieRichBot.Core;

public class Bot
{
    private DiscordSocketClient _client;

    public async Task StartAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
        });

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var token = config["Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("❌ KEIN TOKEN in appsettings.json");
            return;
        }

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    private Task ReadyAsync()
    {
        Console.WriteLine($"🤖 RichieRichBot ist online als {_client.CurrentUser}");
        return Task.CompletedTask;
    }

    private Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
