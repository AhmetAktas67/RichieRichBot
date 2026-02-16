using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using RichieRichBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RichieRichBot.SlashCommands
{
    public class WelcomeCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly WelcomeService _welcomeService;

        public WelcomeCommands(WelcomeService welcomeService)
        {
            _welcomeService = welcomeService;
        }

        // =========================
        // /setwelcome
        // =========================
        [SlashCommand("setwelcome", "Setzt den Welcome-Channel")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task SetWelcomeAsync(ITextChannel channel)
        {
            await DeferAsync(ephemeral: true);

            _welcomeService.SetChannel(Context.Guild.Id, channel.Id);

            await FollowupAsync(
                $"✅ Welcome-Channel gesetzt auf {channel.Mention}",
                ephemeral: true
            );
        }

        // =========================
        // /testwelcome
        // =========================
        [SlashCommand("testwelcome", "Testet die Welcome-Nachricht")]
        [DefaultMemberPermissions(GuildPermission.Administrator)]
        public async Task TestWelcomeAsync()
        {
            await DeferAsync(ephemeral: true);

            var channelId = _welcomeService.GetChannel(Context.Guild.Id);
            if (channelId == null)
            {
                await FollowupAsync(
                    "❌ Kein Welcome-Channel gesetzt. Nutze zuerst `/setwelcome`.",
                    ephemeral: true
                );
                return;
            }

            var channel = Context.Guild.GetTextChannel(channelId.Value);
            if (channel == null)
            {
                await FollowupAsync(
                    "❌ Welcome-Channel nicht gefunden.",
                    ephemeral: true
                );
                return;
            }

            // Fake-Welcome-Nachricht
            await channel.SendMessageAsync(
                $"👋 Willkommen **{Context.User.Mention}** auf **{Context.Guild.Name}**! (Test)"
            );

            await FollowupAsync(
                "✅ Welcome-Nachricht wurde gesendet.",
                ephemeral: true
            );
        }
    }
}
