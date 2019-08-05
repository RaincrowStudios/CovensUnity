using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class AddSpiritHandlerPOP : IGameEventHandler
{
    public const string EventName = "add.pop.token.spirit";
    public static event System.Action<Token> OnSpiritAddPOP;

    public void HandleResponse(string eventData)
    {
        var spiritToken = JsonConvert.DeserializeObject<SpiritToken>(eventData);
        OnSpiritAddPOP?.Invoke(spiritToken);
    }
}