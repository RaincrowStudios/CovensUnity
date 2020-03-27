using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpiritBanishedHandler : IGameEventHandler
    {
        public string EventName => "kill.spirit";
        public static System.Action<SpiritBanishedEvent> OnSpiritBanished;

        public struct SpiritBanishedEvent
        {
            [JsonProperty("actionId")]
            public string actionId;
            [JsonProperty("_id")]
            public string id;
            public string spirit;
            public long xp;
            public int silver;
            public int baseEnergy;
            public bool wild;
            public bool knownSpirit;
            public List<SpiritBanishedDropItem> drops;
        }
        public struct SpiritBanishedDropItem
        {
            public string collectible;
            public int amount;
        }

        public void HandleResponse(string eventData)
        {
            SpiritBanishedEvent data = JsonConvert.DeserializeObject<SpiritBanishedEvent>(eventData);

            PlayerDataManager.playerData.AddCurrency(data.silver, 0);

            OnSpiritBanished?.Invoke(data);
        }
    }
}