using UnityEngine;
using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;
using TMPro;

namespace Raincrow.GameEventResponses
{
    public class AddStatusEffectHandler : IGameEventHandler
    {
        private struct AddEventData
        {
            [JsonProperty("_id")]
            public string id;
            public StatusEffect appliedEffect;
        }

        public string EventName => "add.effect";

        public void HandleResponse(string eventData)
        {
            AddEventData data = JsonConvert.DeserializeObject<AddEventData>(eventData);
            MarkerSpawner.ApplyStatusEffect(data.id, null, data.appliedEffect);
        }
    }
}