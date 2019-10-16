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
        
        public void HandleResponse(string eventData)
        {
            ExpireEventData data = JsonConvert.DeserializeObject<ExpireEventData>(eventData);
            ExpireEffect(data.id, data.effect);
        }

        public static void ExpireEffect(string character, StatusEffect effect)
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

                marker.OnExpireStatusEffect(effect);
            }

            SpellCastHandler.OnExpireEffect?.Invoke(character, effect);

            if (character == PlayerDataManager.playerData.instance)
                PlayerConditionManager.OnExpireEffect(effect);
        }
    }
}