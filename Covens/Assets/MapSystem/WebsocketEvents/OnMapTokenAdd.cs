using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenAdd
{
    public static event System.Action<string> OnTokenAdd;
    
    public static void HandleEvent(WSData data)
    {
        if (DeathState.IsDead)
            return;

        if (data.token.instance == PlayerDataManager.playerData.instance)
            return;

        //  Debug.Log(MarkerManagerAPI.mapReady);
        if (data.token.position == 0)
        {
            var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
            MovementManager.Instance.AddMarker(updatedData);
        }
        else
        {
            LocationUIManager.Instance.AddToken(data.token);
        }

        OnTokenAdd?.Invoke(data.token.instance);
    }
}
