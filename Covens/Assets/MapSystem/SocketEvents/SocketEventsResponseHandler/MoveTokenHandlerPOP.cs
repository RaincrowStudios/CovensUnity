using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MoveTokenHandlerPOP : IGameEventHandler
    {
        public struct MoveEventDataPOP
        {
            [JsonProperty("_id")]
            public string instance;
            public double timestamp;
            public int island;
            public int position;
        }

        public string EventName => "move.pop";
        public static event System.Action<MoveEventDataPOP> OnMarkerMovePOP;

        public void HandleResponse(string eventData)
        {
            Debug.Log(eventData);
            MoveEventDataPOP data = JsonConvert.DeserializeObject<MoveEventDataPOP>(eventData);
            OnMarkerMovePOP?.Invoke(data);
        }
    }
}