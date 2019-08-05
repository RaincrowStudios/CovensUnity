using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class AddWitchHandlerPOP : IGameEventHandler
{
    public const string EventName = "add.pop.token.character";
    public static event System.Action<WitchToken> OnWitchAddPOP;
    public void HandleResponse(string eventData)
    {
        WitchToken witch = JsonConvert.DeserializeObject<WitchToken>(eventData);
        OnWitchAddPOP?.Invoke(witch);
    }
}