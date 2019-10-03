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

        public static event System.Action<string,StatusEffect> OnEffectExpire;
        public static event System.Action<StatusEffect> OnPlayerStatusEffectExpire;

        public void HandleResponse(string eventData)
        {
            ExpireEventData data = JsonConvert.DeserializeObject<ExpireEventData>(eventData);
            ExpireEffect(data.id, data.effect);
        }

        public static void ExpireEffect(string character, StatusEffect effect)
        {
            if (character == PlayerDataManager.playerData.instance)
            {
                ConditionManager.ExpireStatusEffect(effect);
                OnPlayerStatusEffectExpire?.Invoke(effect);
            }
            else
            {
                IMarker marker = MarkerSpawner.GetMarker(character);
                if (marker != null)
                {
                    CharacterToken token = marker.Token as CharacterToken;
                    foreach (StatusEffect item in token.effects)
                    {
                        if (item.spell == effect.spell)
                        {
                            token.effects.Remove(item);
                            item.CancelExpiration();
                            break;
                        }
                    }
                }
                OnEffectExpire?.Invoke(character, effect);
            }
        }
    }
}