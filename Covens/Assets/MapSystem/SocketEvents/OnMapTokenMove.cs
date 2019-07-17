using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Raincrow.GameEventResponses.MapMoveResponseHandler;

public static class OnMapTokenMove
{
    public static event System.Action<string, Vector3> OnTokenMove;
    public static event System.Action<IMarker, Vector3> OnMarkerMove;

    public static void HandleEvent(MoveEventData data)
    {
        if (MarkerManager.Markers.ContainsKey(data.instance))
        {
            IMarker marker = MarkerSpawner.GetMarker(data.instance);

            if (marker == null)
                return;

            marker.coords = new Vector2(data.longitude, data.latitude);
            Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(data.longitude, data.latitude);
            double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, new Vector2(data.longitude, data.latitude));

            if (distance < PlayerDataManager.DisplayRadius)
            {
                OnTokenMove?.Invoke(data.instance, targetPos);
                OnMarkerMove?.Invoke(marker, targetPos);
            }
            else
            {
                marker.interactable = false;
                marker.SetAlpha(0, 2f);
                marker.SetWorldPosition(targetPos, 2f);
                LeanTween.value(0, 0, 2f).setOnComplete(() => OnMapTokenRemove.ForceEvent(data.instance));
            }
        }
    }
}
