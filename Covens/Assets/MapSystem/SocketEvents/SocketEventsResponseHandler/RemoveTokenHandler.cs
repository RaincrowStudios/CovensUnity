using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class RemoveTokenHandler : IGameEventHandler
    {
        public const string EventName = "remove.token";
        public static event System.Action<string> OnTokenRemove;
        public static event System.Action<IMarker> OnMarkerRemove;

        public struct RemoveEventData
        {
            [JsonProperty("_id")]
            public string instance;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            RemoveEventData data = JsonConvert.DeserializeObject<RemoveEventData>(eventData);
            HandleEvent(data);
        }
        
        private static void HandleEvent(RemoveEventData data)
        {
            OnTokenRemove?.Invoke(data.instance);

            IMarker marker = MarkerSpawner.GetMarker(data.instance);

            if (marker != null)
                OnMarkerRemove?.Invoke(marker);
        }

        public static void ForceEvent(string instance)
        {
            var remove_data = new RemoveEventData
            {
                instance = instance
            };
            HandleEvent(remove_data);
        }
    }
}