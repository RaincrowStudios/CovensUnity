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


    private void _OnPlayerEnergyUpdated(int energy)
    {
        PlayerManagerUI.Instance.UpdateEnergy();
    }

    private void _OnPlayerDead()
    {
        PlayerManager.witchMarker.AddDeathFX();
        DeathState.Instance.ShowDeath();

        MapFlightTransition.Instance.RecallHome(true);
    }

    private void _OnPlayerRevived()
    {
        PlayerManager.witchMarker.RemoveDeathFX();
        DeathState.Instance.Revived();
    }

    private void _OnPlayerVulnerable()
    {
        if (LowEnergyPopup.Instance == null)
            Utilities.InstantiateObject(Resources.Load<GameObject>("UILowEnergyPopUp"), DeathState.Instance.transform);
    }

    private void _OnMapTokenAdd(IMarker marker)
    {
        Token token = marker.Token;
                
        marker.Interactable = true;
        if (marker.inMapView)
        {
            marker.GameObject.SetActive(true);
            marker.SetAlpha(1, 1);
        }
    }

    private void _OnMapTokenRemove(IMarker marker)
    {
        Token token = marker.Token;
        if (token.position != 0)
            return;

        //disable interaction wit hit
        if (marker.Interactable)
            marker.Interactable = false;

        //animate the marken
        marker.SetAlpha(0, 1);
        MarkerSpawner.DeleteMarker(marker.Token.instance);
    }

    private void _OnMapTokenMove(IMarker marker, Vector3 position)
    {
        marker.SetWorldPosition(position, 2f);
    }

    private void _OnMapTokenEnergyUpdated(IMarker marker, int energy)
    {
        marker.UpdateEnergy();

        CharacterToken token = marker.Token as CharacterToken;

        if (energy <= 0 || token.state == "dead")
        {
            if (token.Type == MarkerSpawner.MarkerType.WITCH)
                (marker as WitchMarker).AddDeathFX();
            else if (marker.Type == MarkerSpawner.MarkerType.SPIRIT)
                RemoveTokenHandler.ForceEvent(token.instance);
        }
        else
        {
            if (token.Type == MarkerSpawner.MarkerType.WITCH)
                (marker as WitchMarker).RemoveDeathFX();
        }
    }


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
        OnMapEnergyChange.OnPlayerEnergyChange -= _OnPlayerEnergyUpdated;
        OnMapEnergyChange.OnPlayerDead -= _OnPlayerDead;
        OnMapEnergyChange.OnPlayerRevived -= _OnPlayerRevived;
        OnMapEnergyChange.OnPlayerVulnerable -= _OnPlayerVulnerable;

        AddTokenHandler.OnMarkerAdd -= _OnMapTokenAdd;
        RemoveTokenHandler.OnMarkerRemove -= _OnMapTokenRemove;
        MoveTokenHandler.OnMarkerMove -= _OnMapTokenMove;

        MapsAPI.Instance.OnCameraUpdate -= OnMapUpdate;
        PlayerManager.onStartFlight -= OnStartFlight;
    }

    private void OnLeavePoP()
    {
        //make sure no event is subscribed
        OnEnterPoP();

        OnMapEnergyChange.OnPlayerEnergyChange += _OnPlayerEnergyUpdated;
        OnMapEnergyChange.OnPlayerDead += _OnPlayerDead;
        OnMapEnergyChange.OnPlayerRevived += _OnPlayerRevived;
        OnMapEnergyChange.OnPlayerVulnerable += _OnPlayerVulnerable;

        AddTokenHandler.OnMarkerAdd += _OnMapTokenAdd;
        RemoveTokenHandler.OnMarkerRemove += _OnMapTokenRemove;
        MoveTokenHandler.OnMarkerMove += _OnMapTokenMove;
        OnMapEnergyChange.OnMarkerEnergyChange += _OnMapTokenEnergyUpdated;

        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;
        PlayerManager.onStartFlight += OnStartFlight;
    }

}
