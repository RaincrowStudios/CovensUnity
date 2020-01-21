using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpiritDicoveredHandler : IGameEventHandler
    {
        public string EventName => "discover.spirit";
        public static System.Action<DiscoveredEventData> OnSpiritDiscovered;

        public struct DiscoveredEventData
        {
            [JsonProperty("actionId")]
            public string actionId;
            public string spirit;
            public double banishedOn;
            //public string dominion;
            public double timestamp;
            public int baseEnergy;
            public bool wild;
        }

        public void HandleResponse(string eventData)
        {
            DiscoveredEventData data = JsonConvert.DeserializeObject<DiscoveredEventData>(eventData);
            
            var k = new KnownSpirits();
            k.banishedOn = data.banishedOn;
            k.spirit = data.spirit;
            k.dominion = PlayerDataManager.playerData.dominion;

            PlayerDataManager.playerData.knownSpirits.Add(k);
            OnSpiritDiscovered?.Invoke(data);
        }
    }
}