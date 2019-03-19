using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenMove
{
    public static event System.Action<string, Vector3> OnTokenStartMove;
    public static event System.Action<string, Vector3> OnTokenFinishMove;
    public static event System.Action<string> OnTokenEscaped;

    public static void HandleEvent(WSData data)
    {
        if (data.token.position == 0)
        {
            if (MarkerManager.Markers.ContainsKey(data.token.instance))
            {
                double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.position, new Vector2(data.token.longitude, data.token.latitude));
                if (distance < PlayerDataManager.DisplayRadius)
                {
                    IMarker marker = MarkerSpawner.GetMarker(data.token.instance);

                    if (marker != null)
                    {
                        Transform transform = marker.gameObject.transform;
                        Vector3 startPos = transform.position;
                        Vector3 targetPos = MapController.Instance.CoordsToWorldPosition(data.token.longitude, data.token.latitude);

                        LeanTween.value(0, 1, 1f)
                            .setEaseOutCubic()
                            .setOnStart(() => { OnTokenStartMove?.Invoke(data.token.instance, targetPos); })
                            .setOnUpdate((float t) =>
                            {
                                transform.position = Vector3.Lerp(startPos, targetPos, t);
                                MarkerSpawner.Instance.UpdateMarker(marker);
                            })
                            .setOnComplete(() => { OnTokenFinishMove?.Invoke(data.token.instance, targetPos); });
                    }
                }
                else
                {
                    OnTokenEscaped?.Invoke(data.token.instance);
                    MovementManager.Instance.RemoveMarker(data.token.instance);
                }
            }
            else
            {
                var updatedData = MarkerManagerAPI.AddEnumValueSingle(data.token);
                MovementManager.Instance.AddMarker(updatedData);
            }
        }
    }
}
