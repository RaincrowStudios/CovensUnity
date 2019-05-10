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

    public static bool inSpiritForm
    {
        get { return MarkerManagerAPI.inSpiritForm; }
    }

    public static bool isFlying
    {
        get
        {
            return !MapsAPI.Instance.streetLevel;
        }
    }

    public float playerScale = 15;
    public float playerPhysicalScale = 15;
    public GameObject transFormPrefab;
    public GameObject AttackRingPrefab;
    public static GameObject AttackRing;
    bool connectionFailed = false;
    Vector2 currentPos;

    public bool SnapMapToPosition = true;

    DateTime applicationBG;

    public GameObject reinitObject;
    public Image spririt;
    public Text spiritName;
    public Text syncingServer;
    bool CheckFocus = false;

    GameObject atLocationObject;

    public static event Action onStartFlight;
    public static event Action onFinishFlight;

    void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        StartCoroutine(CheckInternetConnection());

        MapsAPI.Instance.OnEnterStreetLevel += OnFinishFlying;
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
    }

    float deltaTime = 0.0f;
    public Color m_Color;

    public void initStart()
    {
        Debug.Log("init start");
        LoginAPIManager.GetCharacterReInit();

        if (IsoPortalUI.isPortal)
            IsoPortalUI.instance.DisablePortalCasting();
        if (SummoningManager.isOpen)
        {
            SummoningController.Instance.Close();
        }
        if (SpellManager.isInSpellView)
        {
            SpellManager.Instance.Exit();
        }
        reinitObject.SetActive(true);
        try
        {
            var d = DownloadedAssets.spiritDictData.ElementAt(UnityEngine.Random.Range(0, DownloadedAssets.spiritDictData.Count));
            spiritName.text = d.Value.spiritName;

            DownloadedAssets.GetSprite(d.Key, spririt);

        }
        catch
        {

        }
        syncingServer.text = "Syncing with server . . .";

    }

    public void InitFinished()
    {
        reinitObject.SetActive(false);
        //		Debug.Log ("Reinit Done");
    }

    void OnApplicationFocus(bool pause)
    {
#if UNITY_EDITOR
        return;
#endif
        if (!pause)
        {
            applicationBG = DateTime.Now;
            CheckFocus = true;
        }
        else
        {

            if (CheckFocus && !LoginUIManager.isInFTF)
            {
                TimeSpan ts = DateTime.Now.Subtract(applicationBG);
                if (ts.TotalSeconds > reinitTime && LoginAPIManager.loggedIn)
                {
                    initStart();
                    CheckFocus = false;
                }
            }
        }
    }

    public void CreatePlayerStart()
    {
        //  GardenMarkers.Instance.CreateGardens();
        // SoundManagerOneShot.Instance.LandingSound();
        if (marker != null)
        {
            MapsAPI.Instance.RemoveMarker(marker);
        }
        var pos = PlayerDataManager.playerPos;
        SpawnPlayer(pos.x, pos.y);
        HeatMapManager.instance.createHeatMap(PlayerDataManager.config.heatmap);
        GardenMarkers.instance.SetupGardens();
        SoundManagerOneShot.Instance.PlayWelcome();
        //MapsAPI.Instance.SetPosition(pos.x, pos.y);
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
        var data = PlayerDataManager.playerData;

        marker = MapsAPI.Instance.AddMarker(pos, markerPrefab);
        marker.gameObject.name = "_MyMarker";
        witchMarker = marker as WitchMarker;
        OnUpdateEquips(() => witchMarker.EnableAvatar());



        // update ring 
        //PDM.playerData.degree

        //		StartCoroutine()
        AddAttackRing();
    }


    void SpawnPhysicalPlayer()
    {
        #region compare coordinates
        double x1, y1, x2, y2;
        //marker.GetPosition(out x1, out y1);
        Vector2 aux = marker.coords;
        x1 = aux.x;
        y1 = aux.y;

        var pos = MapsAPI.Instance.physicalPosition;
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

    public void CancelFlight()
    {
        if (isFlying)
        {
            MapsAPI.Instance.InitMap(marker.coords.x, marker.coords.y, 0.925f, null, true);
        }
    }

    public void FlyTo(double longitude, double latitude, float minDistance = 0.001f, float maxDistance = 0.002f)
    {
        if (PlayerDataManager.playerData.energy == 0)
            return;

        float distance = UnityEngine.Random.Range(minDistance, maxDistance);
        float randAngle = UnityEngine.Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 rand = new Vector2(distance * Mathf.Cos(randAngle), distance * Mathf.Sin(randAngle));

        //Fly();
        MapsAPI.Instance.SetPosition(longitude + rand.x, latitude + rand.y);
        //Fly();
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

    IEnumerator CheckInternetConnection()
    {
        while (true)
        {

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                reinitObject.SetActive(true);

                var d = DownloadedAssets.spiritDictData.ElementAt(UnityEngine.Random.Range(0, DownloadedAssets.spiritDictData.Count));
                spiritName.text = d.Value.spiritName;

                DownloadedAssets.GetSprite(d.Key, spririt);

                syncingServer.text = "Trying to connect . . .";
                connectionFailed = true;
            }
            else if (connectionFailed)
            {
                initStart();
                connectionFailed = false;
            }
            PlayerManagerUI.Instance.checkTime();
            yield return new WaitForSeconds(5);
        }
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

}