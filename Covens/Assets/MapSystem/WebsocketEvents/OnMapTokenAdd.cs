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
        if (data.token.instance == PlayerDataManager.playerData.instance)
            return;

        var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
        IMarker marker = MarkerSpawner.Instance.AddMarker(updatedData, true);
        marker.gameObject.SetActive(false);

        if (marker != null)
            OnMarkerAdd?.Invoke(marker);
        
        OnTokenAdd?.Invoke(data.token.instance);
    }
}
