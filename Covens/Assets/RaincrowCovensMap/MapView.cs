using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    private static MapView m_Instance;

    public static bool InMapView { get; private set; }
    public static bool InBossArea { get; private set; }

    public static event System.Action<WorldBossMarker> OnEnterBossArea;
    public static event System.Action OnLeaveBossArea;

    private void Awake()
    {
        m_Instance = this;

        MapsAPI.Instance.InstantiateMap();
        MapsAPI.Instance.InitOrigin(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);
    }

    private IEnumerator Start()
    {
        while (MapsAPI.Instance.IsInitialized == false)
            yield return null;

        while (PlayerManager.marker == null)
            yield return null;
        
        OnLeavePoP();

        //LocationIslandController.OnEnterLocation += OnEnterPoP;
        //LocationIslandController.OnExitLocation += OnLeavePoP;

        SpiritDicoveredHandler.OnSpiritDiscovered += _OnSpiritDiscovered;
        SpiritBanishedHandler.OnSpiritBanished += _OnSpiritBanished;

        LineRendererBasedDome.Instance.Setup(PlayerDataManager.DisplayRadius * MapsAPI.Instance.OneKmInWorldspace);

        WaitSocketConnection();
    }

    private void WaitSocketConnection()
    {
        if (PlayerDataManager.IsFTF == false && SocketClient.Instance.IsConnected())
        {
            if (PlayerDataManager.playerData.insidePlaceOfPower)
                return;

            //get the markers at the current position
            MarkerManagerAPI.GetMarkers(
               PushManager.flyToPop ? (float)PushManager.longitude : PlayerDataManager.playerData.longitude,
                 PushManager.flyToPop ? (float)PushManager.latitude : PlayerDataManager.playerData.latitude,
                () =>
                {
                    if (PlayerDataManager.playerData.state == "vulnerable"/* || PlayerDataManager.playerData.state == "dead"*/)
                        _OnPlayerVulnerable();
                    if (PlayerDataManager.playerData.state == "dead")
                        _OnPlayerDead();
                },
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
        PlayerManager.witchMarker.OnDeath();
        DeathState.Instance.ShowDeath();

        MapFlightTransition.Instance.RecallHome(true);

        LowEnergyPopup.Show();
    }

    [ContextMenu("_OnPlayerRevived")]
    private void _OnPlayerRevived()
    {
        PlayerManager.witchMarker.OnRevive();
        DeathState.Instance.Revived();
    }

    [ContextMenu("_OnPlayerVulnerable")]
    private void _OnPlayerVulnerable()
    {
        LowEnergyPopup.Show();
    }

    private void _OnMapTokenAdd(IMarker marker)
    {
        Token token = marker.Token;

        marker.Interactable = true;
        if (marker.inMapView)
        {
            marker.GameObject.SetActive(true);
            //marker.SetAlpha(1, 1);
        }

        if (marker.Type == MarkerSpawner.MarkerType.BOSS && !InBossArea)
            _OnEnterBossArea(marker as WorldBossMarker);
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
        //marker.SetAlpha(0, 1);
        MarkerSpawner.DeleteMarker(marker.Token.instance);

        if (marker.Type == MarkerSpawner.MarkerType.BOSS && InBossArea)
            _OnLeaveBossArea();
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

        //marker.SetAlpha(0, 1);
        LeanTween.scale(marker.GameObject, Vector3.zero, 2f).setEaseOutCubic();
    }

    private void _OnMapTokenEnergyUpdated(IMarker marker, int energy)
    {
        marker.UpdateEnergy();

        CharacterToken token = marker.Token as CharacterToken;

        if (energy <= 0 || token.state == "dead")
        {
            if (token.Type == MarkerSpawner.MarkerType.WITCH)
                (marker as WitchMarker).OnDeath();
            else if (marker.Type == MarkerSpawner.MarkerType.SPIRIT)
                RemoveTokenHandler.ForceEvent(token.instance);
        }
        else
        {
            if (token.Type == MarkerSpawner.MarkerType.WITCH)
                (marker as WitchMarker).OnRevive();
        }
    }

    private void _OnSpiritBanished(SpiritBanishedHandler.SpiritBanishedEvent data)
    {
        if (FTFManager.InFTF)
            return;

        //if another UI is open, wait and try to show again
        if (UISpiritBanished.IsOpen || UISpiritDiscovered.IsOpen)
        {
            LeanTween.value(0, 0, 0.5f).setOnComplete(() => _OnSpiritBanished(data));
            return;
        }

        Debug.Log(data.spirit + " BANISHED");
        UISpiritBanished.Instance.Show(data);
    }

    private void _OnSpiritDiscovered(SpiritDicoveredHandler.DiscoveredEventData data)
    {
        //if another UI is open, wait half second and try to show again
        if (UISpiritBanished.IsOpen || UISpiritDiscovered.IsOpen)
        {
            LeanTween.value(0, 0, 0.1f).setOnComplete(() => _OnSpiritDiscovered(data));
            return;
        }

        Debug.Log(data.spirit + " DISCOVERED");
        UISpiritDiscovered.Instance.Show(data.spirit);
    }

    private void MarkerManagerAPI_OnSpawnMarkers(
        List<WitchMarker> wtiches,
        List<SpiritMarker> spirits, 
        List<CollectableMarker> collectables,
        List<EnergyMarker> energies,
        List<LocationMarker> pops,
        List<WorldBossMarker> bosses, 
        List<MuskMarker> loot)
    {
        if (InBossArea && bosses?.Count == 0)
            _OnLeaveBossArea();
        else if (!InBossArea && bosses?.Count > 0)
            _OnEnterBossArea(bosses[0]);
    }

    private void _OnEnterBossArea(WorldBossMarker boss)
    {
        Debug.Log("enter boss area");
        InBossArea = true;
        OnEnterBossArea?.Invoke(boss);
    }

    private void _OnLeaveBossArea()
    {
        Debug.Log("left boss area");
        InBossArea = false;
        OnLeaveBossArea?.Invoke();
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
        MarkerManagerAPI.OnSpawnMarkers -= MarkerManagerAPI_OnSpawnMarkers;

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

        BackButtonListener.onPressBackBtn -= OnPressBackBtn;

        InMapView = false;

        //force despawn markers
        MarkerManagerAPI.SpawnMarkers(new List<WitchToken>(), new List<SpiritToken>(), new List<CollectableToken>(), new List<EnergyToken>(), new List<PopToken>(), new List<BossToken>(), new List<LootToken>());
    }

    private void OnLeavePoP()
    {
        //make sure no event is subscribed
        OnEnterPoP();

        MarkerManagerAPI.OnSpawnMarkers += MarkerManagerAPI_OnSpawnMarkers;

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

        BackButtonListener.onPressBackBtn += OnPressBackBtn;

        InMapView = true;
    }
}
