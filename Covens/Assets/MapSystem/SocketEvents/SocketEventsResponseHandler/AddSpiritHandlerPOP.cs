using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class AddSpiritHandlerPOP : IGameEventHandler
{
    public string EventName => "add.spirit.pop";

    public static event System.Action<Token> OnSpiritAddPOP;

    public void HandleResponse(string eventData)
    {
        var spiritToken = JsonConvert.DeserializeObject<LocationSpiritToken>(eventData);
        spiritToken.spirit.position = spiritToken.position;
        spiritToken.spirit.island = spiritToken.island;
        OnSpiritAddPOP?.Invoke(spiritToken.spirit);
    }

    public static void RaiseEvent(SpiritToken spirit)
    {
        OnSpiritAddPOP?.Invoke(spirit);
    }
}