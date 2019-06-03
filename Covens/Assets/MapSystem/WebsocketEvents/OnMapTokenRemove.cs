using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenRemove
{
    public static event System.Action<string> OnTokenRemove;
    public static event System.Action<IMarker> OnMarkerRemove;

    public static void HandleEvent(WSData data)
    {
        IMarker marker = MarkerSpawner.GetMarker(data.instance);

        if (marker != null)
            OnMarkerRemove?.Invoke(marker);
        OnTokenRemove?.Invoke(data.instance);

    }

    public static void ForceEvent(string instance)
    {
        var remove_data = new WSData
        {
            instance = instance
        };
        OnMapTokenRemove.HandleEvent(remove_data);
    }
}
