using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class AddWitchHandlerPOP : IGameEventHandler
{
    public string EventName => "add.character.pop";
    public static event System.Action<WitchToken> OnWitchAddPOP;
    public void HandleResponse(string eventData)
    {
        Debug.Log("add witch");
        LocationWitchToken token = JsonConvert.DeserializeObject<LocationWitchToken>(eventData);
        token.character.position = token.position;
        token.character.island = token.island;
        OnWitchAddPOP?.Invoke(token.character);
    }
    public static void RaiseEvent(WitchToken witch)
    {
        OnWitchAddPOP?.Invoke(witch);
    }
}