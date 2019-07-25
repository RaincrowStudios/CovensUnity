using Raincrow.GameEventResponses;
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
    }

    private void Awake()
    {
        m_Instance = this;
        OnLeavePoP();

        //PlaceOfPower.OnEnterPlaceOfPower += OnEnterPoP;
        //PlaceOfPower.OnLeavePlaceOfPower += OnLeavePoP;

        //get the markers at the current position
        MarkerManagerAPI.GetMarkers(
            PlayerDataManager.playerData.longitude,
            PlayerDataManager.playerData.latitude,
            null,
            false,
            false,
            false
        );

        LineRendererBasedDome.Instance.Setup(PlayerDataManager.DisplayRadius * MapsAPI.Instance.OneKmInWorldspace);
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
        if (marker.inMapView)
        {
            marker.gameObject.SetActive(true);
            marker.SetAlpha(1, 1);
        }
    }

    private void _OnMapTokenRemove(IMarker marker)
    {
        Token token = marker.customData as Token;
        if (token.position != 0)
            return;

        //disable interaction wit hit
        if (marker.interactable)
            marker.interactable = false;

        //animate the marken
        marker.SetAlpha(0, 1);
        MarkerSpawner.DeleteMarker(marker.token.instance);
    }

    private void _OnMapTokenMove(IMarker marker, Vector3 position)
    {
        marker.SetWorldPosition(position, 2f);
    }

    //private void _OnMapTokenEscape(IMarker marker)
    //{
    //    //do nothing, OnMapTokenEscape will now trigger a OnMapTokenRemove at the end
    //}

    private void OnStartFlight()
    {
        MarkerSpawner.Instance.UpdateMarkers();
    }

    private void OnMapUpdate(bool position, bool zoom, bool twist)
    {
        MarkerSpawner.Instance.UpdateMarkers();
    }

    private void OnEnterPoP()
    {
        AddTokenHandler.OnMarkerAdd -= _OnMapTokenAdd;
        RemoveTokenHandler.OnMarkerRemove -= _OnMapTokenRemove;
        MoveTokenHandler.OnMarkerMove -= _OnMapTokenMove;
        //OnMapTokenMove.OnMarkerEscaped -= _OnMapTokenEscape;

        MapsAPI.Instance.OnCameraUpdate -= OnMapUpdate;
        PlayerManager.onStartFlight -= OnStartFlight;
    }

    private void OnLeavePoP()
    {
        //make sure no event is subscribed
        OnEnterPoP();

        AddTokenHandler.OnMarkerAdd += _OnMapTokenAdd;
        RemoveTokenHandler.OnMarkerRemove += _OnMapTokenRemove;
        MoveTokenHandler.OnMarkerMove += _OnMapTokenMove;
        //OnMapTokenMove.OnMarkerEscaped += _OnMapTokenEscape;

        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;
        PlayerManager.onStartFlight += OnStartFlight;
    }

}
