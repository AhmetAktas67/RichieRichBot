using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;

namespace RichieRichBot.Services
{
    public class WelcomeService
    {
        private const string FileName = "welcomeChannels.json";
        private Dictionary<ulong, ulong> _welcomeChannels = new();


        public WelcomeService()
        {
            Load();
        }


        public void SetChannel(ulong guildId, ulong channelId)
        {
            _welcomeChannels[guildId] = channelId;
            Save();
        }


        public ulong? GetChannel(ulong guildId)
        {
            return _welcomeChannels.TryGetValue(guildId, out var id) ? id : null;
        }


        private void Save()
        {
            File.WriteAllText(FileName, JsonSerializer.Serialize(_welcomeChannels));
        }




        private void Load()
        {
            if (!File.Exists(FileName)) return;

            var json = File.ReadAllText(FileName);
            _welcomeChannels =
                JsonSerializer.Deserialize<Dictionary<ulong, ulong>>(json)
                ?? new();
        }
    }
}
