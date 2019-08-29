using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class ExpireSpiritHandler : IGameEventHandler
    {
        public string EventName => "expire.spirit";

        public static event System.Action<string> OnSpiritExpire;
        public static event System.Action<IMarker> OnSpiritMarkerExpire;

        public struct ExpireEventData
        {
            [JsonProperty("_id")]
            public string instance;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            ExpireEventData data = JsonConvert.DeserializeObject<ExpireEventData>(eventData);
            OnSpiritExpire?.Invoke(data.instance);

            IMarker marker = MarkerSpawner.GetMarker(data.instance);
            if (marker != null)
                OnSpiritMarkerExpire?.Invoke(marker);
        }
    }
}
