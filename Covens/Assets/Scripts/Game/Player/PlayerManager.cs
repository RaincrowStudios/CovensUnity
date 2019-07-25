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
    public GameObject physicalMarkerPrefab;

    public Sprite maleBlack;
    public Sprite maleWhite;
    public Sprite maleAsian;

    public Sprite femaleWhite;
    public Sprite femaleAsian;
    public Sprite femaleBlack;

    public static float reinitTime = 50;

    public GameObject AtLocation_UI;

    public Image playerFlyIcon;

    public static IMarker marker { get; private set; }                //actual marker
    public static IMarker physicalMarker { get; set; }       // gyro marker
    public static WitchMarker witchMarker { get; private set; }
    
    [SerializeField] private GameObject selectionRing;

    public static bool inSpiritForm { get => MarkerManagerAPI.IsSpiritForm; }

    public static bool isFlying { get => !MapsAPI.Instance.streetLevel; }

    public static string SystemLanguage { get => DictionaryManager.Languages[DictionaryManager.languageIndex]; }

    public float playerScale = 15;
    public float playerPhysicalScale = 15;
    public GameObject transFormPrefab;
    public GameObject AttackRingPrefab;
    public static GameObject AttackRing;
    bool connectionFailed = false;
    Vector2 currentPos;

    public bool SnapMapToPosition = true;
        
    GameObject atLocationObject;

    public static event Action onStartFlight;
    public static event Action onFinishFlight;
    //public static event Action onQuickFlight; // quick flight usually happens when you click on a 'go to location' chat message

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        MapsAPI.Instance.OnEnterStreetLevel += OnFinishFlying;
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;

        CreatePlayerStart();
    }           

    private void CreatePlayerStart()
    {
        if (marker != null)
            return;

        var pos = new Vector2(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude);
        SpawnPlayer(pos.x, pos.y);

        Debug.LogError("TODO: SETUP HEATMAPS");
        //HeatMapManager.instance.createHeatMap(PlayerDataManager.config.heatmap);
        GardenMarkers.instance.SetupGardens();
        SoundManagerOneShot.Instance.PlayWelcome();
    }


    void onMapChangePos()
    {
        SnapMapToPosition = false;
    }

    public void ReSnapMap()
    {
        SnapMapToPosition = true;
    }

    void SpawnPlayer(float x, float y)
    {
        Vector2 pos = new Vector2(x, y);
        GameObject markerGo = GameObject.Instantiate(markerPrefab);
        marker = MapsAPI.Instance.AddMarker(pos, markerGo);
        marker.gameObject.name += "_MyMarker";
        marker.inMapView = true;
        marker.coords = pos;
        marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        witchMarker = marker as WitchMarker;

        OnUpdateEquips(() => witchMarker.EnableAvatar());

        AddAttackRing();

        marker.OnClick += (m) => OnClickSelf();
    }


    void SpawnPhysicalPlayer()
    {
        #region compare coordinates
        double x1, y1, x2, y2;
        //marker.GetPosition(out x1, out y1);
        Vector2 aux = marker.coords;
        x1 = aux.x;
        y1 = aux.y;

        var pos = GetGPS.coordinates;
        x2 = pos.x;
        y2 = pos.y;
        x2 = System.Math.Round(x2, 6);
        y2 = System.Math.Round(y2, 6);
        x1 = System.Math.Round(x1, 6);
        y1 = System.Math.Round(y1, 6);
        #endregion

        if (x2 != x1 && y2 != y1)
        {
            //physicalMarker = MapsAPI.Instance.AddMarker(pos, physicalMarkerPrefab);
            //physicalMarker.scale = playerPhysicalScale;
            //physicalMarker.SetRange(3, 20);
            //physicalMarker.instance.name = "_PhysicalMarker";
            //physicalMarker.instance.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4;
            //inSpiritForm = true;
            //var ms = physicalMarker.instance.GetComponent<MarkerScaleManager>();
            //ms.iniScale = physicalMarker.scale;
            //ms.m = physicalMarker;
        }
        else
        {
            //if (physicalMarker != null)
            //{
            //    MapsAPI.Instance.RemoveMarker(PlayerManager.physicalMarker);
            //}
        }
    }

    void fadePlayerMarker()
    {
        //var g = Utilities.InstantiateObject(transFormPrefab, marker.gameObject.transform);
        //marker.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, .25f);
    }

    [ContextMenu("Cancel flight")]
    public void CancelFlight()
    {
        if (isFlying)
        {
            MapCameraUtils.SetPosition(marker.coords, 1f, false);
            LeanTween.value(0, 0, 1.1f).setOnComplete(() => MapCameraUtils.SetZoom(0.925f, 1.5f, false));
        }
        else //just recenter the map
        {
            MapCameraUtils.SetPosition(marker.gameObject.transform.position, 1f, false);
        }
    }

    public void FlyTo(double longitude, double latitude, float minDistance = 0.0003f, float maxDistance = 0.0006f)
    {
        if (DeathState.IsDead || PlayerDataManager.playerData.energy == 0)
            return;

        if (BanishManager.isBind)
            return;

        float distance = UnityEngine.Random.Range(minDistance, maxDistance);
        float randAngle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 rand = new Vector2(distance * Mathf.Cos(randAngle), distance * Mathf.Sin(randAngle));


        Vector2 p = new Vector2((float)longitude + rand.x, (float)latitude + rand.y);
        Vector2 playerPos = PlayerManager.marker.coords;

        if (MapsAPI.Instance.DistanceBetweenPointsD(p, playerPos) > 0.1f)
        {
            MapsAPI.Instance.SetPosition(p.x, p.y);
            MarkerManagerAPI.GetMarkers(false, true, null, true);
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
        double dist = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, GetGPS.coordinates);

        if (dist < 0.1f)
        {
            PlayerManager.Instance.atLocationUIShow();
            return;
        }

        if (BanishManager.isBind || DeathState.IsDead)
            return;

        MapFlightTransition.Instance.RecallHome();
    }

    public static void CenterMapOnPlayer()
    {
        double x, y;
        //marker.GetPosition(out x, out y);
        Vector2 aux = marker.coords;
        x = aux.x;
        y = aux.y;

        MapsAPI.Instance.SetPosition(x, y);
    }

    public void AddAttackRing()
    {
        selectionRing = marker.gameObject.transform.GetChild(0).GetChild(2).gameObject;

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
        witchMarker.SetupAvatar(PlayerDataManager.playerData.male, PlayerDataManager.playerData.equipped, (spr) => callback?.Invoke());
        witchMarker.SetupPortrait(PlayerDataManager.playerData.male, PlayerDataManager.playerData.equipped);
    }

    private void OnStartFlying()
    {
        currentPos = marker.coords;

        MainUITransition.Instance.EnableSummonButton(false);
        MainUITransition.Instance.EnableShoutButton(false);
        MainUITransition.Instance.EnableLocationButton(false);
        FlightVisuals.Instance.StartFlight();
        FlySFX.Instance.fly();

        onStartFlight?.Invoke();
    }

    private void OnFinishFlying()
    {
        FlightVisuals.Instance.EndFlight();

        System.Action finishFlight = () =>
        {
            FlySFX.Instance.EndFly();
            MainUITransition.Instance.EnableLocationButton(true);
            MainUITransition.Instance.EnableSummonButton(true);
            MainUITransition.Instance.EnableShoutButton(true);

            onFinishFlight?.Invoke();
        };

        if (MapsAPI.Instance.position != currentPos)
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

    private void OnClickSelf()
    {
        Debug.LogError("TODO: FILTER SPELLS");

        MapCameraUtils.FocusOnMarker(witchMarker.transform.position);
        Vector3 previousPosition = MapsAPI.Instance.mapCenter.position;
        float previousZoom = MapsAPI.Instance.normalizedZoom;

        List<SpellData> spells = PlayerDataManager.playerData.Spells;
        UISpellcasting.Instance.Show(null, marker, spells,
            () => { //on closed the cast results

            },
            () => { //on click return (X)
                UISpellcasting.Instance.Close();
                MapCameraUtils.FocusOnPosition(previousPosition, previousZoom, true);
            },
            () => { //on click close (outside the book)
                UISpellcasting.Instance.Close();
                MapCameraUtils.FocusOnPosition(previousPosition, previousZoom, true);
            });
    }
}