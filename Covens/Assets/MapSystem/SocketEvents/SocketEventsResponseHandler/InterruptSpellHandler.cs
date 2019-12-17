using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class InterruptSpellHandler : IGameEventHandler
    {
        public string EventName => "interrupt.spell";

        public static event System.Action<EventData> OnInterrupt;

        public struct EventData
        {
            public string targetId;
            public string spellId;
        }
        
        public void HandleResponse(string json)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(json);
            OnInterrupt?.Invoke(data);
        }
    }
}
