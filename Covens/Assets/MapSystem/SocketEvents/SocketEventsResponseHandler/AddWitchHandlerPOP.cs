using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class AddWitchHandlerPOP : IGameEventHandler
{
    public string EventName => "add.character.pop";
    public static event System.Action<WitchToken> OnWitchAddPOP;
    public void HandleResponse(string eventData)
    {
        WitchToken witch = JsonConvert.DeserializeObject<WitchToken>(eventData);
        OnWitchAddPOP?.Invoke(witch);
    }

    public static void RaiseEvent(WitchToken witch)
    {
        OnWitchAddPOP?.Invoke(witch);
    }
}