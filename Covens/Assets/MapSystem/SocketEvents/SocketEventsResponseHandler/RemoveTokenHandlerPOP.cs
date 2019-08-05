using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class RemoveTokenHandlerPOP : IGameEventHandler
{
    public struct RemoveEventData
    {
        [JsonProperty("_id")]
        public string instance;
        public double timestamp;
    }
    public string EventName => "remove.pop.token";

    public static event System.Action<string> OnRemoveTokenPOP;

    public void HandleResponse(string eventData)
    {
        RemoveEventData removeData = JsonConvert.DeserializeObject<RemoveEventData>(eventData);
        OnRemoveTokenPOP?.Invoke(removeData.instance);
    }
}