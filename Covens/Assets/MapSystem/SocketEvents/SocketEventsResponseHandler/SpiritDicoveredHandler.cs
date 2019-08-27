using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpiritDicoveredHandler : IGameEventHandler
    {
        public string EventName => "discover.spirit";
        public static event System.Action<string> OnSpiritDiscovered;

        private struct DiscoveredEventData
        {
            public string spirit;
            public double banishedOn;
            public string dominion;
        }

        public void HandleResponse(string eventData)
        {
            DiscoveredEventData data = JsonConvert.DeserializeObject<DiscoveredEventData>(eventData);

            //UISpiritDiscovered.Instance.Show(data.spirit);
            var k = new KnownSpirits();
            k.banishedOn = data.banishedOn;
            k.spirit = data.spirit;
            k.dominion = data.dominion;
            PlayerDataManager.playerData.knownSpirits.Add(k);

            OnSpiritDiscovered?.Invoke(data.spirit);
        }
    }
}