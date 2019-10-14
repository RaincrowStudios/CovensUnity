using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterXpHandler : IGameEventHandler
    {
        public string EventName => "character.xp";

        private struct CharacterXpData
        {
            [JsonProperty("xp")]
            public ulong xp;
            [JsonProperty("timestamp")]
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            CharacterXpData data = JsonConvert.DeserializeObject<CharacterXpData>(eventData);
            HandleEvent(data.xp, data.timestamp);
        }

        public static void HandleEvent(ulong xp, double timestamp)
        {
            PlayerDataManager.playerData.UpdateExp(xp, timestamp);
        }
    }
}
