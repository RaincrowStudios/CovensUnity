using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;

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

    [Header("Place Of Power")]
    public GameObject level1Loc;
    public GameObject level2Loc;
    public GameObject level3Loc;

    [Header("Collectibles")]
    public GameObject herb;
    public GameObject tool;
    public GameObject gem;
    public GameObject silver;

    [Header("Marker Scales")]
    public float witchScale = 4;
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

    public GameObject tokenFarAway;
    public Slider distanceSlider;

    public GameObject loadingObjectPrefab;
    [SerializeField] private GameObject m_LoadingScreen;

    bool curGender;
    float scaleVal = 1;
    public GameObject lore;
    //	public List<string> instanceIDS = 
    private Dictionary<string, Sprite> m_SpiritIcons;
    private Transform centerPoint;

    public enum MarkerType
    {
        portal, spirit, duke, location, witch, summoningEvent, gem, herb, tool, silver, lore
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
            { "",   familiar     }
        };
    }

    void Start()
    {
        MapController.Instance.m_StreetMap.OnChangePosition += UpdateMarkers;
        centerPoint = MapController.Instance.m_StreetMap.cameraCenter;
    }

    public void CreateMarkers(List<Token> Data)
    {
        List<Token> tempData = new List<Token>();
        HashSet<string> existedMarkers = new HashSet<string>();
        //		DeleteAllMarkers ();

        foreach (var item in Data)
        {
            if (Markers.ContainsKey(item.instance))
            {
                foreach (var m in Markers[item.instance])
                {
                    ImmunityMap[item.instance] = item.immunityList;
                    m.SetPosition(item.longitude, item.latitude);
                    existedMarkers.Add(item.instance);
                }
            }
            else
            {
                tempData.Add(item);
            }
        }
        List<string> deleteList = new List<string>();
        foreach (var item in Markers)
        {
            if (!existedMarkers.Contains(item.Key))
            {
                deleteList.Add(item.Key);
            }
        }
        StartCoroutine(CreateMarkersHelper(tempData));
        StartCoroutine(DeleteMarkersHelper(deleteList));
    }

    IEnumerator DeleteMarkersHelper(List<string> deleteList)
    {
        foreach (var item in deleteList)
        {
            DeleteMarker(item);
        }
        yield return 0;
    }

    IEnumerator CreateMarkersHelper(List<Token> Data)
    {
        foreach (var item in Data)
        {
            AddMarker(item);
            yield return 0;
        }
        yield return 1;
        UpdateMarkers();
    }

    void callzoom()
    {
        EventManager.Instance.CallSmoothZoom();
    }

    public void AddMarker(Token Data)
    {
        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                item.SetPosition(Data.longitude, Data.latitude);
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

        float randomX = Random.Range(-100, 100.0f);
        float randomZ = Random.Range(-100, 100.0f);

        markers[0].gameObject.transform.Translate(randomX, 0, randomZ);
        if (Vector3.Distance(markers[0].gameObject.transform.position, centerPoint.position) > 150)
        {
            markers[0].gameObject.SetActive(false);
        }

        Markers.Add(Data.instance, markers);
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

            //setup icon
            var sp = marker.gameObject.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
            if (m_SpiritIcons.ContainsKey(data.spiritType))
                sp.sprite = m_SpiritIcons[data.spiritType];

            //setup spirit sprite
            sp = marker.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

            if (string.IsNullOrEmpty(data.spiritId))
                Debug.LogError("spritid not sent [" + data.instance + "]");
            else
            {
                DownloadedAssets.GetSprite(data.spiritId, (sprite) =>
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    sp.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * sp.transform.lossyScale.x, 0);
                    sp.sprite = sprite;
                });
            }
        }
        else if (data.Type == MarkerType.duke)
        {
            if (data.degree == 1)
                marker = SetupMarker(dukeWhite, pos, DukeScale, 13);
            else if (data.degree == -1)
                marker = SetupMarker(dukeShadow, pos, DukeScale, 13);
            else if (data.degree == 0)
                marker = SetupMarker(dukeGrey, pos, DukeScale, 13);
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
        }
        else if (data.Type == MarkerType.tool)
        {
            marker = SetupMarker(tool, pos, botanicalScale, 13);
        }
        else if (data.Type == MarkerType.silver)
        {
            marker = SetupMarker(silver, pos, botanicalScale, 13);
        }
        else if (data.Type == MarkerType.gem)
        {
            marker = SetupMarker(gem, pos, GemScale, 13);
        }
        else if (data.Type == MarkerType.lore)
        {
            marker = SetupMarker(lore, pos, 2.8f, 11);
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

        }

        else if (data.Type == MarkerType.silver)
        {
            marker = SetupMarker(tool, pos, botanicalScale, 13);
        }

        var mList = new List<IMarker>();
        mList.Add(marker);
        return mList;
    }

    private void SetupWitch(IMarker marker, Token data)
    {
        if (!FTFManager.isInFTF)
        {
            if (!data.bot)
            {
                if (data.race.Contains("m_"))
                    data.male = true;
                else
                    data.male = false;
            }
            marker.Setup(data);

            //setup the portrait and avatar sprites
            List<EquippedApparel> equipped = new List<EquippedApparel>(data.equipped.Values);
            marker.SetupAvatarAndPortrait(data.male, equipped);

            //set immunity icon
            if (IsPlayerImmune(data.instance))
                OnMapImmunityChange.AddImmunityFX(marker);

            SetupStance(marker.gameObject.transform, data);
        }
    }

    public void onClickMarker(IMarker m)
    {
        if (!PlayerManager.Instance.fly || PlayerDataManager.playerData.energy <= 0 || LocationUIManager.isLocation)
        {
            Debug.Log("DEAD!" + PlayerManager.Instance.fly);
            return;
        }

        var Data = m.customData as Token;

        SelectedMarkerPos = m.position;
        SelectedMarker3DT = Data.Object.transform;
        //		GetMarkerDetailAPI.GetData(Data.instance,Data.Type); 
        OnTokenSelect(Data);

        //show the basic available info, and waiting for the map/select response to fill the details
        if (Data.Type == MarkerType.witch)
        {
            UIPlayerInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.spirit)
        {
            UISpiritInfo.Instance.Show(m, Data);
        }
    }

    public void OnTokenSelect(Token Data, bool isLoc = false)
    {
        instanceID = Data.instance;
        selectedType = Data.Type;
        curGender = Data.male;
        TargetMarkerDetailData data = new TargetMarkerDetailData();
        data.target = instanceID;
        APIManager.Instance.PostData("map/select", JsonConvert.SerializeObject(data), GetResponse);
    }

    public void GetResponse(string response, int code)
    {
        if (code == 200)
        {
            var data = JsonConvert.DeserializeObject<MarkerDataDetail>(response);
            if (selectedType == MarkerType.lore)
            {
                QuestsController.instance.ExploreQuestDone(data.id);
                return;
            }
            if (data.conditions != null)
            {
                foreach (var item in data.conditions)
                {
                    data.conditionsDict[item.instance] = item;
                }
            }
            SelectedMarker = data;
            SelectedMarker.male = curGender;

            if (selectedType == MarkerType.witch)
            {
                //fill the details
                if (UIPlayerInfo.Instance.Witch.displayName == data.displayName)
                    UIPlayerInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.spirit)
            {
                //if (UISpiritInfo.Instance.Spirit.owner == data.owner)
                    UISpiritInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.portal || selectedType == MarkerType.location)
            {
                ShowSelectionCard.Instance.ShowCard(selectedType);
            }
            else if (selectedType == MarkerType.tool || selectedType == MarkerType.gem || selectedType == MarkerType.herb)
            {
                InventoryPickUpManager.Instance.OnDataReceived();
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
        marker.gameObject.transform.SetParent(this.transform);
        return marker;
    }

    public void SetupStance(Transform witchMarker, Token data)
    {
        Debug.Log("TODO-SetupStance");
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
        if (ImmunityMap.ContainsKey(spellTarget))
            ImmunityMap[spellTarget].Add(spellCaster);
        else
            MarkerSpawner.ImmunityMap[spellTarget] = new HashSet<string>() { spellCaster };
    }


    public static Sprite GetSpiritTierSprite(string spiritType)
    {
        return Instance.m_SpiritIcons[spiritType];

    }

    private float m_Distance;
    private void UpdateMarkers()
    {
        if (m_IsHighlighting)
            return;

        foreach (List<IMarker> _markers in Markers.Values)
        {
            UpdateMarker(_markers[0]);
        }
    }

    public void UpdateMarker(IMarker marker)
    {
        m_Distance = Vector2.Distance(
                   new Vector2(centerPoint.position.x, centerPoint.position.z), new Vector2(marker.characterTransform.position.x, marker.characterTransform.position.z));

        if (m_Distance > 150)
        {
            marker.inMapView = false;
            marker.gameObject.SetActive(false);
        }
        else if (m_Distance > 50)
        {
            marker.inMapView = true;
            marker.gameObject.SetActive(true);
            marker.EnablePortaitIcon();
        }
        else
        {
            marker.inMapView = true;
            marker.gameObject.SetActive(true);
            marker.EnableAvatar();
        }
    }

    private static int m_HighlightTweenId;
    private static float m_Aux = 1f;

    private static bool m_IsHighlighting = false;

    public static void HighlightMarker(List<IMarker> targets, bool highlight)
    {
        m_IsHighlighting = highlight;

        IMarker[] toHide = new IMarker[Markers.Count];

        int i = 0;
        foreach (List<IMarker> _markers in Markers.Values)
        {
            IMarker _marker = _markers[0];
            if (_marker.inMapView)
            {
                if (targets.Contains(_marker))
                    continue;

                toHide[i] = _marker;
                i++;
            }
        }

        LeanTween.cancel(m_HighlightTweenId);
        m_HighlightTweenId = LeanTween.value(m_Aux, highlight ? 0 : 1, .5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Aux = t;
                for (int j = 0; j < i; j++)
                {
                    toHide[j].gameObject.transform.localScale = new Vector3(t, t, t);
                }
            })
            .uniqueId;
    }

    public void SetMarkersScale(bool scaleDown)
    {
        foreach (var item in Markers)
        {
            if (item.Key != instanceID)
            {
                foreach (var m in item.Value)
                {
                    if (m.gameObject.activeInHierarchy)
                    {
                        if (scaleDown)
                            LeanTween.scale(m.gameObject, Vector3.zero, .5f);
                        else
                            LeanTween.scale(m.gameObject, Vector3.one, .5f);
                    }
                }
            }
        }
    }
}

