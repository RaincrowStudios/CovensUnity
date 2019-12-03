using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Raincrow.Maps;
using Raincrow.Analytics.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; set; }
    public GameObject markerPrefab;

    public GameObject AtLocation_UI;

    private IMarker m_Marker;

    public static IMarker marker => LocationIslandController.isInBattle ? LocationPlayerAction.playerMarker : Instance.m_Marker;
    public static WitchMarker witchMarker => marker as WitchMarker;

    public static bool inSpiritForm { get => MarkerManagerAPI.IsSpiritForm; }

    public static bool isFlying { get => !MapsAPI.Instance.streetLevel; }

    public static string SystemLanguage { get => DictionaryManager.Languages[DictionaryManager.languageIndex]; }

    private Vector2 m_LastPosition;

    GameObject atLocationObject;

    public static event Action onStartFlight;
    public static event Action onFinishFlight;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        while (PlayerDataManager.playerData == null)
            yield return null;

        while (MapsAPI.Instance.IsInitialized == false)
            yield return null;

        CreatePlayerStart();

        yield return null;

        MapsAPI.Instance.OnEnterStreetLevel += OnFinishFlying;
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
    }

    private void CreatePlayerStart()
    {
        if (marker != null)
            return;

        var pos = new Vector2(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);
        SpawnPlayer(pos.x, pos.y);

        GardenMarkers.instance.SetupGardens();
        SoundManagerOneShot.Instance.PlayWelcome();

        if (PlayerDataManager.playerData.state == "dead")
            DeathState.Instance.ShowDeath();

        List<StatusEffect> conditions = new List<StatusEffect>();
        if (PlayerDataManager.playerData.effects != null)
            conditions = new List<StatusEffect>(PlayerDataManager.playerData.effects);

        foreach (var condition in conditions)
            MarkerSpawner.ApplyStatusEffect(PlayerDataManager.playerData.instance, null, condition);
    }

    void SpawnPlayer(float x, float y)
    {
        Vector2 pos = new Vector2(x, y);
        GameObject markerGo = GameObject.Instantiate(markerPrefab);
        m_Marker = MapsAPI.Instance.AddMarker(pos, markerGo);
        marker.GameObject.name += "_MyMarker";
        marker.inMapView = true;
        marker.Coords = pos;
        marker.Setup(new PlayerToken());
        marker.AvatarTransform.rotation = MapsAPI.Instance.camera.transform.rotation;

        OnUpdateEquips(() => witchMarker.EnableAvatar());

        AddAttackRing();

        //marker.OnClick += (m) => OnClickSelf();
    }

    [ContextMenu("Cancel flight")]
    public void CancelFlight()
    {
        if (isFlying)
        {
            MapCameraUtils.SetPosition(marker.Coords, 1f, false);
            LeanTween.value(0, 0, 1.1f).setOnComplete(() => MapCameraUtils.SetZoom(0.925f, 1.5f, false));
        }
        else //just recenter the map
        {
            MapCameraUtils.SetPosition(marker.GameObject.transform.position, 1f, false);
        }
    }

    public void FlyTo(double longitude, double latitude, float minDistance = 0.0003f, float maxDistance = 0.0006f)
    {

        Debug.Log("FLYING!");
        if (DeathState.IsDead || PlayerDataManager.playerData.energy == 0)
            return;

        if (BanishManager.isBind)
            return;



        float distance = UnityEngine.Random.Range(minDistance, maxDistance);
        float randAngle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 rand = new Vector2(distance * Mathf.Cos(randAngle), distance * Mathf.Sin(randAngle));


        Vector2 p = new Vector2((float)longitude + rand.x, (float)latitude + rand.y);
        Vector2 playerPos = PlayerManager.marker.Coords;

        if (MapsAPI.Instance.DistanceBetweenPointsD(p, playerPos) > 0.1f)
        {
            PlayerLandFX.PlayFlightAnim();
            MapsAPI.Instance.SetPosition(p.x, p.y);
            MarkerManagerAPI.GetMarkers(false, true, PlayerLandFX.PlayLandingAnim, true);
        }
        else
        {
            Vector3 worldPos = MapsAPI.Instance.GetWorldPosition(p.x, p.y);
            MapCameraUtils.SetPosition(worldPos, 1f, true);
        }

        PlayerManagerUI.Instance.CheckPhysicalForm();
        //onQuickFlight?.Invoke();
    }

    public void RecallHome()
    {
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.Coords, GetGPS.coordinates);

        if (dist < 0.1f)
        {
            if (isFlying)
                MapCameraUtils.FocusOnPosition(marker.Coords.x, marker.Coords.y, true, 2f);
            else
                MapCameraUtils.FocusOnPosition(PlayerManager.marker.GameObject.transform.position, true, 2f);

            PlayerManager.Instance.atLocationUIShow();
            return;
        }

        if (BanishManager.isBind || DeathState.IsDead)
            return;

        MapFlightTransition.Instance.RecallHome();
    }

    public void AddAttackRing()
    {
        GameObject selectionRing = marker.GameObject.transform.GetChild(0).GetChild(2).gameObject;

        if (PlayerDataManager.playerData.degree < 0)
        {
            selectionRing.transform.GetChild(0).gameObject.SetActive(true);
            selectionRing.transform.GetChild(1).gameObject.SetActive(false);
            selectionRing.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (PlayerDataManager.playerData.degree > 0)
        {
            selectionRing.transform.GetChild(0).gameObject.SetActive(false);
            selectionRing.transform.GetChild(1).gameObject.SetActive(false);
            selectionRing.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            selectionRing.transform.GetChild(0).gameObject.SetActive(false);
            selectionRing.transform.GetChild(1).gameObject.SetActive(true);
            selectionRing.transform.GetChild(2).gameObject.SetActive(false);
        }
        //AttackRing = Utilities.InstantiateObject(AttackRingPrefab, marker.instance.transform);
        //AttackRing.transform.position += Vector3.up * 0.15f;
    }

    public void OnUpdateEquips(System.Action callback = null)
    {
        if (witchMarker == null)
            return;

        witchMarker.DestroyGeneratedAvatar();
        witchMarker.GenerateAvatar((spr) => callback?.Invoke(), true);

        witchMarker.DestroyGeneratedPortrait();
        witchMarker.GeneratePortrait(null, true);
    }

    private void OnStartFlying()
    {
        m_LastPosition = marker.Coords;

        MainUITransition.Instance.EnableSummonButton(false);
        MainUITransition.Instance.EnableShoutButton(false);
        MainUITransition.Instance.EnableLocationButton(false);
        FlightVisuals.Instance.StartFlight();

        Debug.Log("TODO: ENABLE FLYSFX");
        //FlySFX.Instance.fly();

        onStartFlight?.Invoke();
    }

    private void OnFinishFlying()
    {
        FlightVisuals.Instance.EndFlight();

        System.Action finishFlight = () =>
        {
            Debug.Log("TODO: DISABLE FLY SFX");
            //FlySFX.Instance.EndFly();
            MainUITransition.Instance.EnableLocationButton(true);
            MainUITransition.Instance.EnableSummonButton(true);
            MainUITransition.Instance.EnableShoutButton(true);

            onFinishFlight?.Invoke();
        };

        if (MapsAPI.Instance.position != m_LastPosition)
        {
            MarkerManagerAPI.GetMarkers(false, true, finishFlight);
        }
        else
        {
            finishFlight.Invoke();
        }
    }

    public void atLocationUIShow()
    {
        //if (r != null)
        //{
        if (atLocationObject == null)
            atLocationObject = Utilities.Instantiate(AtLocation_UI, DeathState.Instance.turnOffInteraction[2].transform);
        //}
    }

    public void atLocationUIKill()
    {
        Utilities.Destroy(atLocationObject);
    }

    //private void OnClickSelf()
    //{
    //    MapCameraUtils.FocusOnMarker(witchMarker.transform.position);
    //    Vector3 previousPosition = MapsAPI.Instance.mapCenter.position;
    //    float previousZoom = Mathf.Min(0.98f, MapsAPI.Instance.normalizedZoom);

    //    List<SpellData> spells = new List<SpellData>(PlayerDataManager.playerData.UnlockedSpells);
    //    //spells.RemoveAll(spell => spell.target == SpellData.Target.OTHER);

    //    UISpellcastBook.Open(PlayerDataManager.playerData, marker, spells,
    //        (spell, ingredients) =>
    //        {
    //            //on click spell glyph
    //            Spellcasting.CastSpell(spell, marker, ingredients,
    //                (result) => { },
    //                () => { });
    //        },
    //        () =>
    //        { //on click return
    //            MapCameraUtils.FocusOnPosition(previousPosition, previousZoom, true);
    //        },
    //        () =>
    //        { //on click close
    //            MapCameraUtils.FocusOnPosition(previousPosition, previousZoom, true);
    //        });
    //}

    public static string GetQuickcastSpell(int index)
    {
        string[] defaultSpells = new string[] { "spell_hex", "spell_bless" };
        string defaultValue = index < defaultSpells.Length ? defaultSpells[index] : null;

        return PlayerPrefs.GetString($"{PlayerDataManager.playerData.instance}.quickcast.{index}", defaultValue);
    }

    public static void SetQuickcastSpell(int index, string spell) => PlayerPrefs.SetString($"{PlayerDataManager.playerData.instance}.quickcast.{index}", spell);
}