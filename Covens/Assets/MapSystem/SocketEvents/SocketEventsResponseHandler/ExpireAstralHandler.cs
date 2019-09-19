using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class ExpireAstralHandler : IGameEventHandler
{
    public string EventName => "expire.astral";
    public static event System.Action<string> OnExpireAstral;

    public struct ExpireAstralData
    {
        public string _id;
        public double timestamp;
    }

    public void HandleResponse(string eventData)
    {
        ExpireAstralData data = JsonConvert.DeserializeObject<ExpireAstralData>(eventData);
        OnExpireAstral?.Invoke(data._id);
    }
}