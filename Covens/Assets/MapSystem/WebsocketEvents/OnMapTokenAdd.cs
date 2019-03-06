using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenAdd
{
    public static event System.Action<string> OnTokenAdd;

    public static void HandleEvent(WSData data)
    {
        if (data.token.position == 0)
        {
            var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
            if (MapSelection.currentView == CurrentView.MapView)
                MovementManager.Instance.AddMarker(updatedData);
            else
                MovementManager.Instance.AddMarkerIso(updatedData);
        }
        else
        {
            LocationUIManager.Instance.AddToken(data.token);
        }

        OnTokenAdd?.Invoke(data.token.instance);
    }
}
