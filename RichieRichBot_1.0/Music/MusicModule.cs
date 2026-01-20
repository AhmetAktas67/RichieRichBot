using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace RichieRichBot.Music;

public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("play", "Joint deinen Voice-Channel (Audio folgt später)")]
    public async Task Play(string song)
    {
        // Prüfen ob User im Voice-Channel ist
        if (Context.User is not SocketGuildUser user || user.VoiceChannel == null)
        {
            await RespondAsync(
                "❌ Du musst in einem Voice-Channel sein.",
                ephemeral: true
            );
            return;
        }

        var voiceChannel = user.VoiceChannel;

        // Prüfen ob Bot schon verbunden ist
        if (Context.Guild.CurrentUser.VoiceChannel != null)
        {
            await RespondAsync("⚠️ Ich bin bereits in einem Voice-Channel.");
            return;
        }

        // Voice join
        await voiceChannel.ConnectAsync();

        await RespondAsync($"🎵 Joined **{voiceChannel.Name}**\nSong (später): `{song}`");
    }
}
