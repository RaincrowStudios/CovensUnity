using Newtonsoft.Json;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MoveHandler : IGameEventHandler
    {
        public struct MoveEventData
        {
            [JsonProperty("_id")]
            public string instance;

            public float longitude;
            public float latitude;
            public double timestamp;
        }

        public const string ResponseName = "move";

        public void HandleResponse(string eventData)
        {
            MoveEventData data = JsonConvert.DeserializeObject<MoveEventData>(eventData);
            OnMapTokenMove.HandleEvent(data);
        }       
    }
}