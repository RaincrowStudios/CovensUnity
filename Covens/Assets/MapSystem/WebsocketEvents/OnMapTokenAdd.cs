using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenAdd
{
    public static event System.Action<string> OnTokenAdd;
    public static event System.Action<IMarker> OnMarkerAdd;

    public static void HandleEvent(WSData data)
    {
        if (data.token == null)
        {
            Debug.LogError("no token on map_token_add");
            return;
        }

        //ignore if somehow trying to add a token of the local player
        if (data.token.instance == PlayerDataManager.playerData.instance)
            return;

        //ignore if the token is already dead
        if (data.token.energy <= 0 || data.token.state == "dead")
            return;
        
        IMarker marker = MarkerSpawner.GetMarker(data.token.instance);
        bool isNew = marker == null;

        var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
        marker = MarkerSpawner.Instance.AddMarker(updatedData, true);

        if (marker == null)
            return;

        if (isNew)
        {
            marker.gameObject.SetActive(false);
            marker.SetAlpha(0);
        }
        
        OnMarkerAdd?.Invoke(marker);               
        OnTokenAdd?.Invoke(data.token.instance);
    }
}
