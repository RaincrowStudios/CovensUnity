using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;
using TMPro;

public class MarkerSpawner : MarkerManager
{
    public static Dictionary<string, HashSet<string>> ImmunityMap = new Dictionary<string, HashSet<string>>();



    public static MarkerSpawner Instance { get; set; }
    public static MarkerType selectedType;
    public static Transform SelectedMarker3DT = null;
    public static Vector2 SelectedMarkerPos;
    public static string instanceID = "";

    [Header("Witch")]
    public GameObject witchIcon;
    public GameObject witchDot;
    public GameObject physicalEnemy;
    public GameObject physicalFriend;
    public GameObject spiritForm;
    public GameObject spiritFormFriend;
    public GameObject spiritFormEnemy;

    public Sprite maleBlack;
    public Sprite maleWhite;
    public Sprite maleAsian;

    public Sprite femaleWhite;
    public Sprite femaleAsian;
    public Sprite femaleBlack;

    public Sprite brigid;


    public Sprite maleAcolyte;
    public Sprite femaleAcolyte;

    [Header("Portals")]
    public GameObject whiteLesserPortal;
    public GameObject shadowLesserPortal;
    public GameObject greyLesserPortal;
    public GameObject whiteGreaterPortal;
    public GameObject shadowGreaterPortal;
    public GameObject greyGreaterPortal;
    public GameObject summoningEventPortal;

    [Header("Spirits")]
    public GameObject spiritIcon;
    public GameObject dukeWhite;
    public GameObject dukeShadow;
    public GameObject dukeGrey;
    public GameObject spiritDot;

    public Sprite forbidden;
    public Sprite familiar;
    public Sprite guardian;
    public Sprite harvester;
    public Sprite healer;
    public Sprite protector;
    public Sprite trickster;
    public Sprite warrior;
    public Sprite unknownType;

    [Header("Place Of Power")]
    public GameObject level1Loc;
    public GameObject level2Loc;
    public GameObject level3Loc;

    [Header("Collectibles")]
    public GameObject herb;
    public GameObject tool;
    public GameObject gem;
    public GameObject silver;
    public GameObject energyIcon;
    public GameObject energyParticles;

    public Transform InventoryButton;
    public GameObject energyUIParticles;


    [Header("Marker Scales")]
    public float witchScale = 4;
    public float energyScale = 3;
    public float witchDotScale = 4;
    public float summonEventScale = 4;
    public float portalGreaterScale = 4;
    public float portalLesserScale = 4;
    public float spiritLesserScale = 4;
    public float spiritGreaterScale = 4;
    public float DukeScale = 4;
    public float spiritDotScale = 4;
    public float placeOfPowerScale = 4;
    public float botanicalScale = 4;
    public float familiarScale = 4;
    public float GemScale = 4;
    [Header("MarkerEnergyRing")]
    public Sprite[] EnergyRings;
    string lastEnergyInstance = "";
    public GameObject tokenFarAway;
    public Slider distanceSlider;

    public GameObject loadingObjectPrefab;
    [SerializeField] private GameObject m_LoadingScreen;

    bool curGender;
    float scaleVal = 1;
    public GameObject lore;
    //	public List<string> instanceIDS = 
    private Dictionary<string, Sprite> m_SpiritIcons;

    public enum MarkerType
    {
        none, portal, spirit, duke, location, witch, summoningEvent, gem, herb, tool, silver, lore, energy
    }


    void Awake()
    {
        Instance = this;
        m_SpiritIcons = new Dictionary<string, Sprite>
        {
            { "forbidden",  forbidden    },
            { "harvester",  harvester    },
            { "healer",     healer       },
            { "protector",  protector    },
            { "trickster",  trickster    },
            { "warrior",    warrior      },
            { "guardian",   guardian     },
            { "familiar",   familiar     },
            { "",           unknownType  }
        };

        LoginAPIManager.OnCharacterInitialized += LoginAPIManager_OnCharacterInitialized;
    }

    private void LoginAPIManager_OnCharacterInitialized()
    {
        //init the map/markers variables
        UpdateProperties();

        LoginAPIManager.OnCharacterInitialized -= LoginAPIManager_OnCharacterInitialized;
    }

    void Start()
    {
        MapsAPI.Instance.OnCameraUpdate += (a,b,c) => UpdateProperties();
        PlayerManager.onStartFlight += UpdateProperties;
        InventoryButton = UIStateManager.Instance.DisableButtons[2].transform;
    }

    void callzoom()
    {
        EventManager.Instance.CallSmoothZoom();
    }

    public IMarker AddMarker(Token Data, bool updateVisuals = false)
    {
        if (LoginUIManager.isInFTF)
            return null;

        double distance = MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(Data.longitude, Data.latitude), PlayerManager.marker.coords);
        if (distance >= PlayerDataManager.DisplayRadius)
        {
#if UNITY_EDITOR
            // Debug.Log("distance (" + distance + "km) too far, skipping token " + Data.displayName);
#endif
            return null;
        }

        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                item.coords = new Vector2(Data.longitude, Data.latitude);
                item.customData = Data;
                UpdateMarker(item);
            }
            return Markers[Data.instance][0];
        }

        List<IMarker> markers = new List<IMarker>();
        if (Data.Type == MarkerType.witch)
        {
            markers = CreateWitch(Data);
        }
        else if (Data.Type == MarkerType.duke || Data.Type == MarkerType.spirit)
        {
            markers = CreateSpirit(Data);
        }
        else
        {
            markers = CreateOther(Data);
        }

        Data.Object = markers[0].gameObject;
        markers[0].customData = Data;
        markers[0].Setup(Data);
        markers[0].OnClick += onClickMarker;

        if (Data.Type == MarkerType.witch)
        {
            SetupWitch(markers[0], Data);
        }

        if (Markers.ContainsKey(Data.instance))
        {
            DeleteMarker(Data.instance);
        }

        Markers.Add(Data.instance, markers);

        if (updateVisuals)
            UpdateMarker(markers[0]);

        return markers[0];
    }

    public void CheckMarkerPos(string instance)
    {
        //if (Markers.ContainsKey(instance))
        //{
        //    if (MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.position, Markers[instance][0].position) < PlayerDataManager.attackRadius)
        //    {
        //        Markers[instance][0].instance.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        //    }
        //    else
        //    {
        //        Markers[instance][0].instance.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, .65f);
        //    }
        //}
    }

    List<IMarker> CreateWitch(Token data)
    {
        ImmunityMap[data.instance] = data.immunityList;

        var pos = new Vector2(data.longitude, data.latitude);

        IMarker marker = SetupMarker(witchIcon, pos, 15, 14);

        marker.gameObject.name = "[witch] " + data.displayName + " [" + data.instance + "]";

        var mList = new List<IMarker>();
        mList.Add(marker);
        return mList;
    }

    List<IMarker> CreateSpirit(Token data)
    {
        var pos = new Vector2(data.longitude, data.latitude);
        IMarker marker = null;
        if (data.Type == MarkerType.spirit)
        {
            marker = SetupMarker(spiritIcon, pos, spiritLesserScale, 13);
            marker.gameObject.name = "[spirit] " + data.spiritId + " [" + data.instance + "]";
        }
        else if (data.Type == MarkerType.duke)
        {
            if (data.degree == 1)
                marker = SetupMarker(dukeWhite, pos, DukeScale, 13);
            else if (data.degree == -1)
                marker = SetupMarker(dukeShadow, pos, DukeScale, 13);
            else if (data.degree == 0)
                marker = SetupMarker(dukeGrey, pos, DukeScale, 13);
            marker.gameObject.name = $"[duke] {data.spiritId}";
        }

        var mList = new List<IMarker>();
        mList.Add(marker);

        return mList;
    }

    List<IMarker> CreateOther(Token data)
    {
        var pos = new Vector2(data.longitude, data.latitude);
        IMarker marker = null;

        if (data.Type == MarkerType.portal)
        {
            if (data.degree == 1)
            {
                marker = SetupMarker(whiteLesserPortal, pos, portalLesserScale, 13);
            }
            else if (data.degree == -1)
            {
                marker = SetupMarker(shadowLesserPortal, pos, portalLesserScale, 13);
            }
            else if (data.degree == 0)
            {
                marker = SetupMarker(greyLesserPortal, pos, portalLesserScale, 13);
            }
            else
            {
                marker = SetupMarker(greyLesserPortal, pos, portalLesserScale, 13);
            }

            marker.gameObject.name = $"[portal] {data.instance}";
        }
        else if (data.Type == MarkerType.summoningEvent)
        {
            if (data.degree == 1)
            {
                marker = SetupMarker(whiteGreaterPortal, pos, summonEventScale, 13);
            }
            else if (data.degree == -1)
            {
                marker = SetupMarker(shadowGreaterPortal, pos, summonEventScale, 13);
            }
        }
        else if (data.Type == MarkerType.herb)
        {
            marker = SetupMarker(herb, pos, botanicalScale, 13);
            marker.gameObject.name = $"[herb] {data.instance}";
        }
        else if (data.Type == MarkerType.tool)
        {
            marker = SetupMarker(tool, pos, botanicalScale, 13);
            marker.gameObject.name = $"[tool] {data.instance}";
        }
        else if (data.Type == MarkerType.silver)
        {
            marker = SetupMarker(silver, pos, botanicalScale, 13);
            marker.gameObject.name = $"[silver] {data.instance}";
        }
        else if (data.Type == MarkerType.energy)
        {
            marker = SetupMarker(energyIcon, pos, botanicalScale, 13);
            marker.gameObject.GetComponentInChildren<TextMeshPro>().text = data.amount.ToString();
            marker.gameObject.name = $"[energy] {data.instance}";
        }
        else if (data.Type == MarkerType.gem)
        {
            marker = SetupMarker(gem, pos, GemScale, 13);
            marker.gameObject.name = $"[gem] {data.instance}";
        }
        else if (data.Type == MarkerType.lore)
        {
            marker = SetupMarker(lore, pos, 2.8f, 11);
            marker.gameObject.name = $"[lore] {data.instance}";
        }

        //TODO ENABLE LOCATIONS

        else if (data.Type == MarkerType.location)
        {

            if (data.tier == 1)
            {
                marker = SetupMarker(level1Loc, pos, placeOfPowerScale, 13);
            }
            else if (data.tier == 2)
            {
                marker = SetupMarker(level2Loc, pos, placeOfPowerScale, 13);
            }
            else
            {
                marker = SetupMarker(level3Loc, pos, placeOfPowerScale, 13);
            }

            marker.gameObject.name = $"[location] {data.instance}";
        }

        // else if (data.Type == MarkerType.silver)
        // {
        //     marker = SetupMarker(tool, pos, botanicalScale, 13);
        // }

        var mList = new List<IMarker>();
        mList.Add(marker);
        return mList;
    }

    private void SetupWitch(IMarker marker, Token data)
    {
        if (!LoginUIManager.isInFTF)
        {
            marker.Setup(data);

            //set immunity icon
            if (IsPlayerImmune(data.instance))
                OnMapImmunityChange.AddImmunityFX(marker);

            if (data.state == "dead" || data.energy <= 0)
                SpellcastingFX.SpawnDeathFX(data.instance, marker);

            SetupStance(marker.gameObject.transform, data);
        }
    }

    public void onClickMarker(IMarker m)
    {
        if (!UIStateManager.isMain)
            return;

        if (PlayerManager.isFlying || PlayerDataManager.playerData.energy <= 0)
        {
            return;
        }

        var Data = m.customData as Token;
        SelectedMarker3DT = m.gameObject.transform;

        //show the basic available info, and waut for the map/select response to fill the details

        if (Data.Type == MarkerType.witch)
        {
            UIPlayerInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.spirit)
        {
            UISpiritInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.portal)
        {
            UIPortalInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.location)
        {
            UIPOPinfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.herb || Data.Type == MarkerType.tool || Data.Type == MarkerType.gem)
        {
            PickUpCollectibleAPI.PickUpCollectable(Data.instance, Data.type);
            var g = Instantiate(energyParticles);
            g.transform.position = SelectedMarker3DT.GetChild(0).GetChild(0).position;
            Utilities.Instantiate(energyUIParticles, InventoryButton);
            MarkerManager.DeleteMarker(Data.instance);
            return;
        }

        OnTokenSelect(m);
    }

    public void OnTokenSelect(IMarker marker)
    {
        Token Data = marker.token;
        instanceID = Data.instance;
        selectedType = Data.Type;
        curGender = Data.male;

        TargetMarkerDetailData data = new TargetMarkerDetailData();
        data.target = instanceID;
        // Debug.Log(Data.instance + "ENERGY INSTANCE");

        //SoundManagerOneShot.Instance.PlayItemAdded();
        if (selectedType == MarkerType.energy && lastEnergyInstance != instanceID)
        {
            var g = Instantiate(energyParticles);
            g.transform.position = SelectedMarker3DT.GetChild(1).position;
            LeanTween.scale(SelectedMarker3DT.gameObject, Vector3.zero, .3f).setOnComplete(() =>
            {
                MarkerManager.DeleteMarker(instanceID);
            });
            var energyData = new { target = Data.instance };
            APIManager.Instance.PostData("map/pickup", JsonConvert.SerializeObject(energyData), (string s, int r) =>
                {
                    Debug.Log(s);

                    if (r == 200 && s != "")
                    {

                        UIEnergyBarGlow.Instance.Glow();
                        SoundManagerOneShot.Instance.PlayEnergyCollect();
                        PlayerDataManager.playerData.energy += Data.amount;
                        PlayerManagerUI.Instance.UpdateEnergy();
                        Debug.Log(instanceID);
                    }
                    else
                    {

                    }
                });

            lastEnergyInstance = instanceID;
        }
        else
        {
            SoundManagerOneShot.Instance.PlayWhisperFX();

            if (PlaceOfPower.IsInsideLocation)
                APIManager.Instance.PostData("location/select", JsonConvert.SerializeObject(data), (response, result) => GetResponse(marker, instanceID, response, result));
            else
                APIManager.Instance.PostData("map/select", JsonConvert.SerializeObject(data), (response, result) => GetResponse(marker, instanceID, response, result));
        }
    }

    public void GetResponse(IMarker marker, string instance, string response, int code)
    {
        if (code == 200)
        {
            switch(marker.type)
            {
                case MarkerType.witch:
                    WitchMarkerDetail witch = JsonConvert.DeserializeObject<WitchMarkerDetail>(response);
                    UpdateMarkerData(instance, witch);
                    //fill the details
                    if (UIPlayerInfo.isShowing && UIPlayerInfo.Instance.Witch.displayName == witch.displayName)
                        UIPlayerInfo.Instance.SetupDetails(witch);
                    break;

                case MarkerType.spirit:
                    SpiritMarkerDetail spirit = JsonConvert.DeserializeObject<SpiritMarkerDetail>(response);
                    UpdateMarkerData(instance, spirit);

                    if (UISpiritInfo.isOpen && UISpiritInfo.Instance.Spirit.instance == instance)
                        UISpiritInfo.Instance.SetupDetails(spirit);
                    if (spirit.state == "dead")
                        OnMapTokenRemove.ForceEvent(instance);
                    break;

                case MarkerType.location:
                    LocationMarkerDetail location = JsonConvert.DeserializeObject<LocationMarkerDetail>(response);

                    if (UIPOPinfo.isOpen && UIPOPinfo.Instance.tokenData.instance == instance)
                        UIPOPinfo.Instance.Setup(location);
                    break;
                case MarkerType.portal:
                    PortalMarkerDetail portal = JsonConvert.DeserializeObject<PortalMarkerDetail>(response);

                    if (UIPortalInfo.isOpen && UIPortalInfo.Instance.token.instance == instance)
                        UIPortalInfo.Instance.SetupDetails(portal);
                    break;
                //case MarkerType.herb:
                //case MarkerType.gem:
                //case MarkerType.tool:
                //    CollectableMarkerDetail collectable = JsonConvert.DeserializeObject<CollectableMarkerDetail>(response);

                //    if (UICollectableInfo.IsOpen && UICollectableInfo.Instance.token.instance == instance)
                //        UICollectableInfo.Instance.SetupDetails(collectable);
                //    break;


                default:
                    Debug.LogError("Token selection not implemented for " + marker.type);
                    //MarkerDetail data = JsonConvert.DeserializeObject<MarkerDetail>(response);
                    break;
            }
        }
        else
        {
            if (response == "4704")
            {
                UIGlobalErrorPopup.ShowPopUp(() => { }, "Move closer to the target.");
            }
            else
            {
                Debug.LogError("select marker error [" + code + "] " + response);
            }
        }
    }

    IMarker SetupMarker(GameObject prefab, Vector2 pos, float scale, int rangeMin = 3, int rangeMax = 20)
    {
        IMarker marker;
        marker = MapsAPI.Instance.AddMarker(pos, prefab);
        return marker;
    }

    public void SetupStance(Transform witchMarker, Token data)
    {
        //Debug.Log("TODO-SetupStance");
        //Dictionary<string, GameObject> names = new Dictionary<string, GameObject>();
        //foreach (Transform item in witchMarker)
        //{
        //    names[item.name] = item.gameObject;
        //}

        //if (StanceDict.ContainsKey(data.instance))
        //{

        //    if (names.ContainsKey("spirit"))
        //        Destroy(names["spirit"]);

        //    if (data.physical)
        //    {
        //        if (StanceDict[data.instance])
        //        {
        //            if (!names.ContainsKey("enemyP"))
        //            {
        //                var g = Utilities.InstantiateObject(physicalEnemy, witchMarker);
        //                g.name = "enemyP";

        //                if (names.ContainsKey("friendP"))
        //                    Destroy(names["friendP"]);
        //                if (names.ContainsKey("enemyS"))
        //                    Destroy(names["enemyS"]);
        //                if (names.ContainsKey("friendS"))
        //                    Destroy(names["friendS"]);

        //            }
        //        }
        //        else
        //        {
        //            if (!names.ContainsKey("friendP"))
        //            {
        //                var g = Utilities.InstantiateObject(physicalFriend, witchMarker);
        //                g.name = "friendP";

        //                if (names.ContainsKey("enemyP"))
        //                    Destroy(names["enemyP"]);
        //                if (names.ContainsKey("enemyS"))
        //                    Destroy(names["enemyS"]);
        //                if (names.ContainsKey("friendS"))
        //                    Destroy(names["friendS"]);

        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (StanceDict[data.instance])
        //        {
        //            if (!names.ContainsKey("enemyS"))
        //            {
        //                var g = Utilities.InstantiateObject(spiritFormEnemy, witchMarker);
        //                g.name = "enemyS";

        //                if (names.ContainsKey("enemyP"))
        //                    Destroy(names["enemyP"]);
        //                if (names.ContainsKey("friendP"))
        //                    Destroy(names["friendP"]);
        //                if (names.ContainsKey("friendS"))
        //                    Destroy(names["friendS"]);

        //            }
        //        }
        //        else
        //        {
        //            if (!names.ContainsKey("friendS"))
        //            {
        //                var g = Utilities.InstantiateObject(spiritFormFriend, witchMarker);
        //                g.name = "friendS";

        //                if (names.ContainsKey("enemyP"))
        //                    Destroy(names["enemyP"]);
        //                if (names.ContainsKey("friendP"))
        //                    Destroy(names["friendP"]);
        //                if (names.ContainsKey("enemyS"))
        //                    Destroy(names["enemyS"]);

        //            }
        //        }
        //    }
        //}

        //if (!data.physical && !names.ContainsKey("spirit"))
        //{
        //    var g = Utilities.InstantiateObject(spiritForm, witchMarker);
        //    g.name = "spirit";
        //}
    }

    /// <summary>
    /// Returns true if the target is immune to the player.
    /// </summary>
    public static bool IsPlayerImmune(string instance)
    {
        if (!ImmunityMap.ContainsKey(instance))
            return false;

        HashSet<string> immunityList = ImmunityMap[instance];

        if (immunityList == null)
            return false;

        if (!immunityList.Contains(PlayerDataManager.playerData.instance))
            return false;

        return true;
    }

    public static void AddImmunity(string spellCaster, string spellTarget)
    {
        if (ImmunityMap.ContainsKey(spellTarget) && ImmunityMap[spellTarget] != null)
            ImmunityMap[spellTarget].Add(spellCaster);
        else
            MarkerSpawner.ImmunityMap[spellTarget] = new HashSet<string>() { spellCaster };
    }

    public static void RemoveImmunity(string caster, string target)
    {
        if (ImmunityMap.ContainsKey(target) && ImmunityMap[target] != null)
            ImmunityMap[target].Remove(caster);
    }


    public static Sprite GetSpiritTierSprite(string spiritType)
    {
        if (spiritType != null && Instance.m_SpiritIcons.ContainsKey(spiritType))
            return Instance.m_SpiritIcons[spiritType];
        else
            return Instance.m_SpiritIcons[""];
    }

    private float m_Distance;
    public static float m_MarkerScale { get; private set; }
    public static bool m_PortaitMode { get; private set; }
    private static bool m_StreetLevel;
    private const float MARKER_SCALE_MIN = 1;
    private const float MARKER_SCALE_MAX = 2;


    public static void UpdateProperties()
    {
        m_PortaitMode = MapsAPI.Instance.streetLevelNormalizedZoom > 0.6f;
        m_StreetLevel = MapsAPI.Instance.streetLevel;
        m_MarkerScale = MARKER_SCALE_MAX * MapsAPI.Instance.normalizedZoom + (MARKER_SCALE_MIN - MapsAPI.Instance.normalizedZoom);
    }

    public void UpdateMarkers()
    {
        if (PlayerManager.marker != null)
        {
            PlayerManager.marker.gameObject.SetActive(m_StreetLevel);
            //  Debug.Log("setting playerMarker");
            PlayerManager.marker.gameObject.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
            PlayerManager.marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }

        if (m_Highlighting)
        {
            foreach (IMarker _marker in m_HighlightedMarkers)
            {
                if (_marker != null && _marker.gameObject != null && _marker != PlayerManager.marker)
                    UpdateMarker(_marker);
            }
        }
        else
        {
            foreach (List<IMarker> _marker in Markers.Values)
            {
                UpdateMarker(_marker[0]);
            }
        }
    }

    public void UpdateMarker(IMarker marker)
    {
        UpdateMarker(marker, m_PortaitMode, m_StreetLevel, m_MarkerScale);
    }

    public static void UpdateMarker(IMarker marker, bool portraitMode, bool streetLevel, float scale)
    {
        if (streetLevel && MapsAPI.Instance.IsPointInsideView(marker.gameObject.transform.position))
        {
            if (portraitMode)
                marker.EnablePortait();
            else
                marker.EnableAvatar();

            if (!marker.inMapView)
            {
                marker.SetAlpha(0);
                marker.SetAlpha(1, 1f);
                marker.gameObject.SetActive(true);
                marker.inMapView = true;
            }
            marker.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }
        else if (marker.inMapView)
        {
            marker.inMapView = false;
            marker.SetAlpha(0, 1f, () => marker.gameObject.SetActive(false));
        }
    }

    private static bool m_Highlighting = false;
    private static List<IMarker> m_HighlightedMarkers = new List<IMarker>();

    public static void HighlightMarker(List<IMarker> targets, bool highlight)
    {
        m_Highlighting = highlight;
        m_HighlightedMarkers = targets;
        MapsAPI.Instance.EnableBuildingIcons(!highlight);

        List<List<IMarker>> markers = new List<List<IMarker>>(Markers.Values);
        foreach (List<IMarker> _marker in markers)
        {
            if (_marker[0].inMapView && !targets.Contains(_marker[0]))
                _marker[0].SetAlpha(highlight ? 0 : 1, 1f);
        }

        foreach (IMarker _marker in targets)
            _marker?.SetAlpha(1, 1f);
    }

    public static void HideVisibleMarkers(float time, bool player)
    {
        List<List<IMarker>> markersList = new List<List<IMarker>>(Markers.Values);

        foreach (List<IMarker> _marker in markersList)
        {
            IMarker marker = _marker[0];
            if (marker.inMapView)
            {
                marker.inMapView = false; //so it wont be detected by MarkerSpawner.HighlightMarker
                marker.SetAlpha(0, time, () =>
                {
                    marker.gameObject.SetActive(false);
                });
            }
        }

        if (player)
            PlayerManager.marker.SetAlpha(0, time);
    }



    //click controller

    private Vector2 m_MouseDownPosition;
    private void Update()
    {
        bool inputUp = false;
        bool inputDown = false;

        if (Application.isEditor)
        {
            inputDown = Input.GetMouseButtonDown(0);
            inputUp = Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        else
        {
            inputDown = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId);
            inputUp = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
        }

        if (inputDown)
        {
            m_MouseDownPosition = Input.mousePosition;
            return;
        }
        else if (inputUp)
        {
            if (Vector2.Distance(m_MouseDownPosition, Input.mousePosition) > 15)
            {
                m_MouseDownPosition = new Vector2(-Screen.width, -Screen.height);
                return;
            }
            m_MouseDownPosition = new Vector2(-Screen.width, -Screen.height);

            Camera cam = MapsAPI.Instance.camera;

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << 20))
            {
                IMarker marker = hit.transform.GetComponentInParent<IMarker>();
                if (marker != null)
                {
                    marker.OnClick?.Invoke(marker);
                    return;
                }
            }
        }
    }
}
