using UnityEngine;
using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;
using TMPro;

namespace Raincrow.GameEventResponses
{
    public class ExpireStatusEffectHandler : IGameEventHandler
    {
        private struct ExpireEventData
        {
            [JsonProperty("_id")]
            public string id;
            public StatusEffect effect;
        }

        public string EventName => "expire.effect";

        public static event System.Action<string,string> OnStatusEffectExpire;
        public static event System.Action<string> OnPlayerStatusEffectExpire;

        public void HandleResponse(string eventData)
        {
            ExpireEventData data = JsonConvert.DeserializeObject<ExpireEventData>(eventData);
            if (data.id == PlayerDataManager.playerData.instance)// ? PlayerManager.marker : MarkerSpawner.GetMarker(data.id);
            {
                ConditionManager.ExpireStatusEffect(data.effect);
                OnPlayerStatusEffectExpire?.Invoke(data.effect.spell);
            }
        }
    }
}