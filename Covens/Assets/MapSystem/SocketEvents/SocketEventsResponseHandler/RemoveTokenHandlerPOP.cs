using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class RemoveTokenHandlerPOP : IGameEventHandler
{
    public struct RemoveEventData
    {
        [JsonProperty("_id")]
        public string instance;
        public int island;
        public int position;
        public double timestamp;
    }
    public string EventName => "remove.token.pop";

    public static event System.Action<RemoveEventData> OnRemoveTokenPOP;

    public void HandleResponse(string eventData)
    {
        Debug.Log(eventData);
        RemoveEventData removeData = JsonConvert.DeserializeObject<RemoveEventData>(eventData);
        OnRemoveTokenPOP?.Invoke(removeData);
    }
}