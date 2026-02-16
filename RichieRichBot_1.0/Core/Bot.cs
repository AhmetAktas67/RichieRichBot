using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RichieRichBot.Services;
using RichieRichBot.SlashCommands;

namespace RichieRichBot.Core;

public class Bot
{
    // =========================
    // 🔹 Felder / Services
    // =========================
    private DiscordSocketClient _client;
    private InteractionService _interactions;
    private WelcomeService _welcomeService;
    private IServiceProvider _services;

    // =========================
    // 🔹 Bot Startpunkt
    // =========================
    public async Task StartAsync()
    {
        // -------------------------
        // Welcome-Service initialisieren
        // -------------------------
        _welcomeService = new WelcomeService();




        _services = new ServiceCollection()
       .AddSingleton(_welcomeService)
      .BuildServiceProvider();


        // -------------------------
        // Discord Client erstellen
        // -------------------------
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMembers
        });

        // -------------------------
        // Interaction Service (Slash Commands)
        // -------------------------
        _interactions = new InteractionService(_client.Rest);

        // -------------------------
        // Events registrieren
        // -------------------------
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.UserJoined += OnUserJoinedAsync;
        _client.UserLeft += OnUserLeftAsync;

        // Slash Command Handling
        _client.InteractionCreated += async interaction =>
        {
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _services);
        };

        // -------------------------
        // Ready Event (Slash Commands laden)
        // -------------------------
        _client.Ready += async () =>
        {
            var services = new ServiceCollection()
                .AddSingleton(_welcomeService)
                .BuildServiceProvider();

            await _interactions.AddModuleAsync<WelcomeCommands>(services);
            await _interactions.RegisterCommandsToGuildAsync
            (1225113759333224519);

            Console.WriteLine("✅ Slash Commands registriert");
        };

        // -------------------------
        // Config & Token laden
        // -------------------------
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var token = config["Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("❌ KEIN TOKEN in appsettings.json");
            return;
        }

        // -------------------------
        // Bot starten
        // -------------------------
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();


        await Task.Delay(-1);
    }

    // =========================
    // 🔹 Ready Event
    // =========================
    private Task ReadyAsync()
    {
        Console.WriteLine($"🤖 RichieRichBot ist online als {_client.CurrentUser}");
        return Task.CompletedTask;
    }

    // =========================
    // 🔹 Logging
    // =========================
    private Task LogAsync(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    // =========================
    // 👤 User joint Server
    // =========================
    private async Task OnUserJoinedAsync(SocketGuildUser user)
    {
        // Welcome-Channel aus Service holen
        var channelId = _welcomeService.GetChannel(user.Guild.Id);

        if (channelId == null)
        {
            Console.WriteLine("⚠️ Kein Welcome-Channel gesetzt");
            return;
        }

        var channel = user.Guild.GetTextChannel(channelId.Value);
        if (channel == null) return;

        await channel.SendMessageAsync(
            $"👋 Willkommen **{user.Mention}** auf **{user.Guild.Name}**!"
        );

        Console.WriteLine($"➕ {user.Username} ist {user.Guild.Name} beigetreten");
    }

    // =========================
    // 👤 User verlässt Server
    // =========================
    private async Task OnUserLeftAsync(SocketGuild guild, SocketUser user)
    {
        var channelId = _welcomeService.GetChannel(guild.Id);
        if (channelId == null) return;

        var channel = guild.GetTextChannel(channelId.Value);
        if (channel == null) return;

        await channel.SendMessageAsync(
            $"👋 **{user.Username}** hat den Server verlassen."
        );

        Console.WriteLine($"➖ {user.Username} hat {guild.Name} verlassen");
    }
}