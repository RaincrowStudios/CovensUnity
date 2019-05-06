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
    public static MarkerDataDetail SelectedMarker = null;
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
        portal, spirit, duke, location, witch, summoningEvent, gem, herb, tool, silver, lore, energy
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
        UpdateMarkers();

        LoginAPIManager.OnCharacterInitialized -= LoginAPIManager_OnCharacterInitialized;
    }

    void Start()
    {
        MapsAPI.Instance.OnCameraUpdate += (position, zoom, twist) => UpdateMarkers();
        MapsAPI.Instance.OnExitStreetLevel += UpdateMarkers;
        InventoryButton = UIStateManager.Instance.DisableButtons[2].transform;
    }

    //public void CreateMarkers(List<Token> Data)
    //{
    //    if (LoginUIManager.isInFTF)
    //        return;
    //    List<Token> newMarkers = new List<Token>();
    //    HashSet<string> existedMarkers = new HashSet<string>();

    //    foreach (var item in Data)
    //    {
    //        if (Markers.ContainsKey(item.instance))
    //        {
    //            foreach (var m in Markers[item.instance])
    //            {
    //                ImmunityMap[item.instance] = item.immunityList;
    //                m.SetPosition(item.longitude, item.latitude);
    //                existedMarkers.Add(item.instance);
    //            }
    //        }
    //        else
    //        {
    //            newMarkers.Add(item);
    //        }
    //    }

    //    List<IMarker> deleteList = new List<IMarker>();
    //    foreach (var item in Markers)
    //    {
    //        if (!existedMarkers.Contains(item.Key))
    //        {
    //            deleteList.Add(item.Value[0]);
    //        }
    //    }

    //    DeleteAllMarkers(deleteList.ToArray());

    //    StartCoroutine(CreateMarkersHelper(newMarkers));
    //}

    //IEnumerator CreateMarkersHelper(List<Token> Data)
    //{
    //    foreach (var item in Data)
    //    {
    //        AddMarker(item);
    //        //yield return 0;
    //    }
    //    yield return 1;

    //    UpdateMarkers();
    //}

    void callzoom()
    {
        EventManager.Instance.CallSmoothZoom();
    }

    public void AddMarker(Token Data, bool updateVisuals = false)
    {
        if (LoginUIManager.isInFTF)
            return;

        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                if (item.inMapView)
                {
                    OnMapTokenMove.MoveMarker(item, Data.instance, Data.longitude, Data.latitude);
                }
                else
                {
                    item.SetPosition(Data.longitude, Data.latitude);
                    UpdateMarker(item);
                }
            }
            return;
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

        marker.gameObject.name = $"[witch] {data.displayName}";

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
            marker.gameObject.name = $"[spirit] {data.spiritId}";
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
            if (!data.bot)
            {
                if (data.race.Contains("m_"))
                    data.male = true;
                else
                    data.male = false;
            }
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

        if (PlayerManager.isFlying || PlayerDataManager.playerData.energy <= 0 || LocationUIManager.isLocation)
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
            SoundManagerOneShot.Instance.PlaySpiritSelectedSpellbook();
        }
        else if (Data.Type == MarkerType.portal)
        {
            UIPortalInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.herb || Data.Type == MarkerType.tool || Data.Type == MarkerType.gem)
        {
            UICollectableInfo.Instance.CollectItem(Data, null);
            var g = Instantiate(energyParticles);
            g.transform.position = SelectedMarker3DT.GetChild(0).GetChild(0).position;
            Utilities.Instantiate(energyUIParticles, InventoryButton);
            MarkerManager.DeleteMarker(Data.instance);
            return;
        }

        OnTokenSelect(Data);
    }

    public void OnTokenSelect(Token Data, bool isLoc = false)
    {

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
            APIManager.Instance.PostData("map/select", JsonConvert.SerializeObject(data), (response, result) => GetResponse(Data.instance, response, result));
        }
    }

    public void GetResponse(string instance, string response, int code)
    {
        if (code == 200)
        {
            var data = JsonConvert.DeserializeObject<MarkerDataDetail>(response);
            // if (selectedType == MarkerType.lore)
            // {
            //     QuestsController.instance.ExploreQuestDone(data.id);
            //     return;
            // }

            SelectedMarker = data;
            SelectedMarker.male = curGender;

            if (selectedType == MarkerType.witch)
            {
                UpdateMarker(instance, data);

                //fill the details
                if (UIPlayerInfo.isShowing && UIPlayerInfo.Instance.Witch.displayName == data.displayName)
                    UIPlayerInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.spirit)
            {
                if (UISpiritInfo.isOpen && UISpiritInfo.Instance.Spirit.instance == instance)
                    UISpiritInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.portal)
            {
                if (UIPortalInfo.isOpen && UIPortalInfo.Instance.token.instance == instance)
                    UIPortalInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.location)
            {
                //  ShowSelectionCard.Instance.SetupDetails(MarkerType.location, data);
            }
            else if (selectedType == MarkerType.tool || selectedType == MarkerType.gem || selectedType == MarkerType.herb)
            {
                if (UICollectableInfo.IsOpen && UICollectableInfo.Instance.token.instance == instance)
                    UICollectableInfo.Instance.SetupDetails(data);
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
    private float m_MarkerScale;
    private bool m_PortaitMode;
    private bool m_StreetLevel;
    private const float MARKER_SCALE_MIN = 1;
    private const float MARKER_SCALE_MAX = 2;

    private void UpdateMarkers()
    {
        m_PortaitMode = MapsAPI.Instance.normalizedZoom < 0.95f;
        m_StreetLevel = MapsAPI.Instance.streetLevel;
        m_MarkerScale = MARKER_SCALE_MAX * MapsAPI.Instance.normalizedZoom + (MARKER_SCALE_MIN - MapsAPI.Instance.normalizedZoom);

        Camera cam = MapsAPI.Instance.camera;

        if (PlayerManager.marker != null)
        {
            PlayerManager.marker.gameObject.SetActive(m_StreetLevel);
            //  Debug.Log("setting playerMarker");
            PlayerManager.marker.gameObject.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
            PlayerManager.marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }

        foreach (List<IMarker> _marker in Markers.Values)
            UpdateMarker(_marker[0]);
    }

    public void UpdateMarker(IMarker marker)
    {
        //Vector3 centerPosition = MapsAPI.Instance.GetWorldPosition();
        //m_Distance = Vector2.Distance(
        //           new Vector2(centerPosition.x, centerPosition.z), new Vector2(marker.characterTransform.position.x, marker.characterTransform.position.z));

        if (m_StreetLevel && MapsAPI.Instance.IsPointInsideView(marker.gameObject.transform.position))
        {
            if (m_PortaitMode)// || m_Distance > CircleRangeTileProvider.minViewDistance / 5f)
                marker.EnablePortait();
            else
                marker.EnableAvatar();

            marker.inMapView = true;
            marker.gameObject.SetActive(true);
            marker.gameObject.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
            marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }
        else
        {
            marker.inMapView = false;
            marker.gameObject.SetActive(false);
        }
    }

    private static bool m_IsHighlighting = false;
    private static IMarker[] m_VisibleMarkers = new IMarker[] { };
    private static IMarker[] m_HiddenMarkers = new IMarker[] { };
    private static int m_VisibleTweenId;
    private static int m_HiddenTweenId;
    private static float m_VisibletTween = 1;
    private static float m_HiddenTween = 1;

    public static void HighlightMarker(List<IMarker> targets, bool highlight)
    {
        m_IsHighlighting = highlight;

        LeanTween.cancel(m_VisibleTweenId);
        LeanTween.cancel(m_HiddenTweenId);

        MapsAPI.Instance.EnableBuildingIcons(!highlight);

        if (highlight)
        {
            List<IMarker> toHide = new List<IMarker>();
            foreach (List<IMarker> _markers in Markers.Values)
            {
                if (targets.Contains(_markers[0]))
                    continue;
                else
                    toHide.Add(_markers[0]);
            }

            m_VisibleMarkers = targets.ToArray();
            m_HiddenMarkers = toHide.ToArray();

            m_HiddenTweenId = LeanTween.value(m_HiddenTween, 0f, 0.5f).setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_HiddenTween = t;
                    for (int i = 0; i < m_HiddenMarkers.Length; i++)
                        m_HiddenMarkers[i].MultiplyAlpha(t);

                }).uniqueId;

            m_VisibleTweenId = LeanTween.value(m_VisibletTween, 1f, 0.5f).setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_VisibletTween = t;
                    for (int i = 0; i < m_VisibleMarkers.Length; i++)
                        m_VisibleMarkers[i].MultiplyAlpha(t);

                }).uniqueId;
        }
        else
        {
            m_HiddenTweenId = LeanTween.value(m_HiddenTween, 1f, 0.5f).setEaseOutCubic()
                 .setOnUpdate((float t) =>
                 {
                     m_HiddenTween = t;
                     for (int i = 0; i < m_HiddenMarkers.Length; i++)
                         m_HiddenMarkers[i].MultiplyAlpha(t);

                 }).uniqueId;

            m_VisibleTweenId = LeanTween.value(m_VisibletTween, 1f, 0.5f).setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_VisibletTween = t;
                    for (int i = 0; i < m_VisibleMarkers.Length; i++)
                        m_VisibleMarkers[i].MultiplyAlpha(t);

                }).uniqueId;
        }
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
