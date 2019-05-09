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

        IMarker marker = MarkerSpawner.GetMarker(data.token.instance);
        bool isNew = marker == null;

        var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
        marker = MarkerSpawner.Instance.AddMarker(updatedData, true);

        if (isNew)
        {
            marker.gameObject.SetActive(false);
            marker.SetAlpha(0);
        }
        
        OnMarkerAdd?.Invoke(marker);               
        OnTokenAdd?.Invoke(data.token.instance);
    }
}
