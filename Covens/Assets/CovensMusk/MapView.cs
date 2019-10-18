using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    private static MapView m_Instance;
    private List<string> m_DiscoveredSpirits = new List<string>();
    private List<string> m_BanishedSpirits = new List<string>();

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        OnLeavePoP();

        LocationIslandController.OnEnterLocation += OnEnterPoP;
        LocationIslandController.OnExitLocation += OnLeavePoP;

        SpiritDicoveredHandler.OnSpiritDiscovered += _OnSpiritDiscovered;
        SpellCastHandler.OnSpiritBanished += _OnSpiritBanished;

        PlayerManager.Instance.CreatePlayerStart();
        
        LineRendererBasedDome.Instance.Setup(PlayerDataManager.DisplayRadius * MapsAPI.Instance.OneKmInWorldspace);

        WaitSocketConnection();
    }

    private void WaitSocketConnection()
    {
        if (SocketClient.Instance.IsConnected())
        {
            //get the markers at the current position
            MarkerManagerAPI.GetMarkers(
                PlayerDataManager.playerData.longitude,
                PlayerDataManager.playerData.latitude,
                null,
                true,
                false,
                true
            );
        }
        else
        {
            //Debug.LogError("Waiting for socket connection");
            LeanTween.value(0, 0, 0.5f).setOnComplete(WaitSocketConnection);
        }
    }

    private void _OnPlayerEnergyUpdated(int energy)
    {
        PlayerManagerUI.Instance.UpdateEnergy();
    }

    [ContextMenu("_OnPlayerDead")]
    private void _OnPlayerDead()
    {
        PlayerManager.witchMarker.AddDeathFX();
        DeathState.Instance.ShowDeath();

        MapFlightTransition.Instance.RecallHome(true);
    }

    [ContextMenu("_OnPlayerRevived")]
    private void _OnPlayerRevived()
    {
        PlayerManager.witchMarker.RemoveDeathFX();
        DeathState.Instance.Revived();
    }

    [ContextMenu("_OnPlayerVulnerable")]
    private void _OnPlayerVulnerable()
    {
        if (LowEnergyPopup.Instance == null)
            Utilities.InstantiateObject(Resources.Load<GameObject>("UILowEnergyPopUp"), null);//, DeathState.Instance.transform);
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
        
    private void _OnSpiritExpire(IMarker marker)
    {
        MarkerSpawner.DeleteMarker(marker.Token.instance);

        if (marker.Interactable)
            marker.Interactable = false;

        marker.SetAlpha(0, 1);
        LeanTween.scale(marker.GameObject, Vector3.zero, 2f).setEaseOutCubic();
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

    private void _OnSpiritBanished(string spirit)
    {
        if (FTFManager.InFTF)
            return;

        Debug.Log(spirit + " BANISHED");
        
        //discover.spirit was triggered before the banish
        if (m_DiscoveredSpirits.Contains(spirit))
        {
            m_DiscoveredSpirits.Remove(spirit);
            UISpiritDiscovered.Instance.Show(spirit);
        }

        //discover.spirit was not triggered yet
        else
        {
            bool discovered = PlayerDataManager.playerData.knownSpirits.Exists(spr => spr.spirit == spirit);

            //player already had this spirit
            if (discovered)
                UISpiritBanished.Instance.Show(spirit);

            //wait for the discover.spirit event
            else
                m_BanishedSpirits.Add(spirit);
        }
    }

    private void _OnSpiritDiscovered(string spirit)
    {
        Debug.Log(spirit + " DISCOVERED");

        //banished was already triggered
        if (m_BanishedSpirits.Contains(spirit))
        {
            UISpiritDiscovered.Instance.Show(spirit);
        }
        //banish was not triggered yet
        else
        {
            m_DiscoveredSpirits.Add(spirit);
        }
    }
    
    private void _OnSummonDeath(string spirit)
    {
        OnCharacterDeath.ShowSummonDeath();
    }

    private void _OnSpiritDeath(string spirit)
    {
        string spiritName = LocalizeLookUp.GetSpiritName(spirit);
        OnCharacterDeath.ShowSpiritDeath(spiritName);
    }

    private void _OnWitchDeath(string witch)
    {
        OnCharacterDeath.ShowWitchDeath(witch);
    }

    private void _OnSpellSuicide(string spell)
    {
        OnCharacterDeath.ShowSpellCastSuicide();
    }


    private void OnStartFlight()
    {
        MarkerSpawner.Instance.UpdateMarkers();
    }

    private void OnMapUpdate(bool position, bool zoom, bool twist)
    {
        MarkerSpawner.Instance.UpdateMarkers();
    }

    private void OnPressBackBtn(int stackedActions)
    {
        if (!FTFManager.InFTF && stackedActions > 0)
            return;

        if (!UIGlobalPopup.IsOpen)
            UIGlobalPopup.ShowPopUp(Application.Quit, () => { }, LocalizeLookUp.GetText("close_app_prompt"));
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
        OnMapEnergyChange.OnMarkerEnergyChange -= _OnMapTokenEnergyUpdated;
        ExpireSpiritHandler.OnSpiritMarkerExpire -= _OnSpiritExpire;

        MapsAPI.Instance.OnCameraUpdate -= OnMapUpdate;
        PlayerManager.onStartFlight -= OnStartFlight;

        OnCharacterDeath.OnCastSuicide -= _OnSpellSuicide;
        OnCharacterDeath.OnSpiritDeath -= _OnSpiritDeath;
        OnCharacterDeath.OnWitchDeath -= _OnWitchDeath;
        OnCharacterDeath.OnSummonDeath -= _OnSummonDeath;

        BackButtonListener.onPressBackBtn -= OnPressBackBtn;
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
        ExpireSpiritHandler.OnSpiritMarkerExpire += _OnSpiritExpire;

        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;
        PlayerManager.onStartFlight += OnStartFlight;

        OnCharacterDeath.OnCastSuicide += _OnSpellSuicide;
        OnCharacterDeath.OnSpiritDeath += _OnSpiritDeath;
        OnCharacterDeath.OnWitchDeath += _OnWitchDeath;
        OnCharacterDeath.OnSummonDeath += _OnSummonDeath;

        BackButtonListener.onPressBackBtn += OnPressBackBtn;
    }
}
