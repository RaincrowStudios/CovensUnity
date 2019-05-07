using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenAdd
{
    public static event System.Action<string> OnTokenAdd;

    public static void HandleEvent(WSData data)
    {
        if (data.token.instance == PlayerDataManager.playerData.instance)
            return;

        if (data.token.position == 0)
        {
            var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
            IMarker marker = MovementManager.Instance.AddMarker(updatedData);

            if (marker != null)
            {
                if (marker.type == MarkerSpawner.MarkerType.portal && data.token.owner == PlayerDataManager.playerData.instance)
                    MapCameraUtils.FocusOnPosition(marker.gameObject.transform.position, true, 2f);
            }
        }
        else
        {
            LocationUIManager.Instance.AddToken(data.token);
        }

        OnTokenAdd?.Invoke(data.token.instance);
    }
}
