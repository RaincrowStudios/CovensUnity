using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterEnergyHandler : IGameEventHandler
    {
        public string EventName => "character.energy";

        private struct CharacterEnergyEventData
        {
            [JsonProperty("_id")]
            public string id;
            public int energy;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            CharacterEnergyEventData data = JsonConvert.DeserializeObject<CharacterEnergyEventData>(eventData);

            IMarker marker = MarkerManager.GetMarker(data.id);
            OnMapEnergyChange.ForceEvent(marker, data.energy, data.timestamp);
        }
    }
}