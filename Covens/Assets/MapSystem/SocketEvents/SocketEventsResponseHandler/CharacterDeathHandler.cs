using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterDeathHandler : IGameEventHandler
    {
        public string EventName => "character.dead";

        public struct DeathEventData
        {
            public string spirit;
            public string character;
            public string spell;
            public int degree;
        }

        public static event System.Action<DeathEventData> OnPlayerDeath;

        public void HandleResponse(string eventData)
        {
            DeathEventData data = JsonConvert.DeserializeObject<DeathEventData>(eventData);
            OnPlayerDeath?.Invoke(data);
        }
    }
}