using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenAdd
{
    public static event System.Action<string> OnTokenAdd;
    private static List<WSData> tokens = new List<WSData>();

    public static void mapReady()
    {
        foreach (var item in tokens)
        {
            MovementManager.Instance.AddMarker(item.token);
        }
        tokens.Clear();
    }

    public static void HandleEvent(WSData data)
    {
        if (DeathState.IsDead)
            return;

        Debug.Log(MarkerManagerAPI.mapReady);
        if (data.token.position == 0)
        {
            var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
            //if (MapSelection.currentView == CurrentView.MapView)
            if (MarkerManagerAPI.mapReady)
                MovementManager.Instance.AddMarker(updatedData);
            else
            {
                tokens.Add(data);
            }
            //else
            //    MovementManager.Instance.AddMarkerIso(updatedData);
        }
        else
        {
            LocationUIManager.Instance.AddToken(data.token);
        }

        OnTokenAdd?.Invoke(data.token.instance);
    }
}
