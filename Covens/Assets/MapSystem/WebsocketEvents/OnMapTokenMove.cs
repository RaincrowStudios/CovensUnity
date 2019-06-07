using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapTokenMove
{
    public static event System.Action<string, Vector3> OnTokenMove;
    public static event System.Action<IMarker, Vector3> OnMarkerMove;
    //public static event System.Action<string> OnTokenEscaped;
    //public static event System.Action<IMarker> OnMarkerEscaped;

    public static void HandleEvent(WSData data)
    {
        double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, new Vector2(data.token.longitude, data.token.latitude));

        if (MarkerManager.Markers.ContainsKey(data.token.instance))
        {
            IMarker marker = MarkerSpawner.GetMarker(data.token.instance);
            Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(data.token.longitude, data.token.latitude);

            if (marker != null)
                marker.coords = new Vector2(data.token.longitude, data.token.latitude);

            if (distance < PlayerDataManager.DisplayRadius)
            {
                OnTokenMove?.Invoke(data.token.instance, targetPos);
                
                if (marker != null)
                    OnMarkerMove?.Invoke(marker, targetPos);
            }
            else
            {
                if (marker != null)
                {
                    //tempfix for banishing, currently the marker is being removed before the map_spell_cast arrives.
                    marker.interactable = false;
                    marker.SetAlpha(0, 2f);
                    LeanTween.value(0, 0, 2f).setOnComplete(() => OnMapTokenRemove.ForceEvent(data.token.instance));
                }
                else
                {
                    OnMapTokenRemove.ForceEvent(data.token.instance);
                }
            }
        }
        else //use the data as a AddTokenEvent instead
        {
            OnMapTokenAdd.HandleEvent(data);
        }
    }

    //public static void MoveMarker(IMarker marker, string instance, float lng, float lat)
    //{
    //    Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(lng, lat);

    //    OnTokenMove?.Invoke(instance, targetPos);

    //    if (marker != null)
    //    {
    //        OnMarkerMove?.Invoke(marker, targetPos);

    //        Transform transform = marker.gameObject.transform;
    //        Vector3 startPos = transform.position;
            
    //        LeanTween.value(0, 1, 1f)
    //            .setEaseOutCubic()
    //            .setOnUpdate((float t) =>
    //            {
    //                // if (transform != null)
    //                // {
    //                if (transform != null)
    //                {
    //                    transform.position = Vector3.Lerp(startPos, targetPos, t);
    //                    MarkerSpawner.Instance.UpdateMarker(marker);
    //                }// }}
    //                else
    //                {
    //                    //Debug.Log("<color=#FF0000>The transform issue that creates tons of errors");
    //                }
    //            });
    //    }
    //}
}
