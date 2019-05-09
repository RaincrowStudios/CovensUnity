using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    private static MapView m_Instance;

    public static void Initialize()
    {
        if (m_Instance != null)
            return;

        m_Instance = new GameObject("MapView").AddComponent<MapView>();
        m_Instance.OnLeavePoP();

        PlaceOfPower.OnEnterPlaceOfPower += m_Instance.OnEnterPoP;
        PlaceOfPower.OnLeavePlaceOfPower += m_Instance.OnLeavePoP;
    }

    private void _OnMapTokenAdd(IMarker marker)
    {
        Token token = marker.customData as Token;

        //token is in PoP
        if (token.position != 0)
            return;

        //TODO: ONLY FOCUS ON THE PORTAL IF IT WAS JUST SUMMONED (CURRENTLY IT WILL FOCUS ON THE PORTAL IF IT WAS RESULT OF /MOVE REQUEST)
        ////focus camera on marker if it is a portal summoned by me
        //if (marker.type == MarkerSpawner.MarkerType.portal && token.owner == PlayerDataManager.playerData.instance)
        //    MapCameraUtils.FocusOnPosition(marker.gameObject.transform.position, true, 2f);

        marker.interactable = true;
        marker.gameObject.SetActive(true);
        marker.SetAlpha(0);

        //animate the marker
        LeanTween.value(0, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                marker.SetAlpha(t);
            });
    }

    private void _OnMapTokenRemove(IMarker marker)
    {
        Token token = marker.customData as Token;

        if (token.position != 0)
            return;

        //disable interaction wit hit
        marker.interactable = false;

        //animate the marken, then actually despawn it 
        LeanTween.value(1, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                marker.SetAlpha(t);
            })
            .setOnComplete(() =>
            {
                MapsAPI.Instance.RemoveMarker(marker);
            });
    }

    private void _OnMapTokenMove(IMarker marker, Vector3 position)
    {
        marker.SetWorldPosition(position, 1f);
    }

    private void _OnMapTokenEscape(IMarker marker)
    {
        //animate the marken, then actually despawn it 
        LeanTween.value(1, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                marker.SetAlpha(t);
            })
            .setOnComplete(() =>
            {
                MapsAPI.Instance.RemoveMarker(marker);
            });
    }

    private void OnEnterPoP()
    {
        OnMapTokenAdd.OnMarkerAdd -= _OnMapTokenAdd;
        OnMapTokenRemove.OnMarkerRemove -= _OnMapTokenRemove;
        OnMapTokenMove.OnMarkerMove -= _OnMapTokenMove;
        OnMapTokenMove.OnMarkerEscaped -= _OnMapTokenEscape;
    }

    private void OnLeavePoP()
    {
        OnMapTokenAdd.OnMarkerAdd += _OnMapTokenAdd;
        OnMapTokenRemove.OnMarkerRemove += _OnMapTokenRemove;
        OnMapTokenMove.OnMarkerMove += _OnMapTokenMove;
        OnMapTokenMove.OnMarkerEscaped += _OnMapTokenEscape;
    }
}
