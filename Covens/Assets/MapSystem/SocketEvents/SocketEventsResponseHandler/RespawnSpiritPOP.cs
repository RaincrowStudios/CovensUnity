using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class RespawnSpiritPOP : IGameEventHandler
{
    public string EventName => "respawn.spirit";
    public static event System.Action<SpiritToken> OnSpiritRewspawn;


    public void HandleResponse(string eventData)
    {
        SpiritToken data = JsonConvert.DeserializeObject<SpiritToken>(eventData);
        OnSpiritRewspawn?.Invoke(data);
    }
}