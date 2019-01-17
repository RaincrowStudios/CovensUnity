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
    private GameObject loadingObject;
    bool curGender;
    float scaleVal = 1;
    public GameObject lore;
    //	public List<string> instanceIDS = 



    public enum MarkerType
    {
        portal, spirit, duke, location, witch, summoningEvent, gem, herb, tool, silver, lore
    }

    void Awake()
    {
        Instance = this;
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

        Data.Object = markers[0].instance;
        Data.scale = markers[0].scale;
        markers[0].customData = Data;
        markers[0].OnClick += onClickMarker;

        if (Markers.ContainsKey(Data.instance))
        {
            DeleteMarker(Data.instance);
        }
        Markers.Add(Data.instance, markers);
    }

    public void CheckMarkerPos(string instance)
    {
        if (Markers.ContainsKey(instance))
        {
            if (MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.position, Markers[instance][0].position) < PlayerDataManager.attackRadius)
            {
                Markers[instance][0].instance.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                Markers[instance][0].instance.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, .65f);
            }
        }
    }

    List<IMarker> CreateWitch(Token data)
    {
        ImmunityMap[data.instance] = data.immunityList;

        var pos = new Vector2(data.longitude, data.latitude);
        IMarker marker;
        IMarker markerDot;
        marker = SetupMarker(witchIcon, pos, 15, 14);
        var sp = marker.instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (!FTFManager.isInFTF)
        {
            if (!data.bot)
            {
                if (data.race.Contains("m_"))
                {
                    if (data.race.Contains("A"))
                    {
                        sp.sprite = maleBlack;
                    }
                    else if (data.race.Contains("O"))
                    {
                        sp.sprite = maleAsian;
                    }
                    else
                    {
                        sp.sprite = maleWhite;
                    }
                }
                else
                {
                    if (data.race.Contains("A"))
                    {
                        sp.sprite = femaleBlack;
                    }
                    else if (data.race.Contains("O"))
                    {
                        sp.sprite = femaleAsian;
                    }
                    else
                    {
                        sp.sprite = femaleWhite;
                    }
                }
            }
            else
            {
                if (data.male)
                {
                    sp.sprite = maleAcolyte;
                }
                else
                {
                    sp.sprite = femaleAcolyte;
                }
            }
        }
        else
        {
            sp.sprite = brigid;
        }
        try
        {
            if (ImmunityMap[data.instance].Contains(PlayerDataManager.playerData.instance))
            {
                marker.instance.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, .3f);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        markerDot = SetupMarker(witchDot, pos, witchDotScale, 3, 13);
        marker.instance.GetComponent<MarkerScaleManager>().iniScale = witchScale;
        marker.instance.GetComponent<MarkerScaleManager>().m = marker;
        markerDot.instance.GetComponent<MarkerScaleManager>().iniScale = witchDotScale;
        markerDot.instance.GetComponent<MarkerScaleManager>().m = markerDot;
        marker.instance.GetComponentInChildren<UnityEngine.UI.Text>().text = data.displayName;
        var mList = new List<IMarker>();
        if (PlayerDataManager.playerData.covenName != "")
        {
            if (data.coven == PlayerDataManager.playerData.covenName)
            {
                marker.instance.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
        }
        mList.Add(marker);
        mList.Add(markerDot);
        SetupStance(marker.instance.transform, data);
        if (MapsAPI.Instance.zoom > 14)
        {
            markerDot.instance.gameObject.SetActive(false);
        }
        else
            marker.instance.gameObject.SetActive(false);
        return mList;
    }

    List<IMarker> CreateSpirit(Token data)
    {
        var pos = new Vector2(data.longitude, data.latitude);
        IMarker marker = null;
        IMarker markerDot = null;
        if (data.Type == MarkerType.spirit)
        {
            marker = SetupMarker(spiritIcon, pos, spiritLesserScale, 13);
            var sp = marker.instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (data.spiritType == "forbidden")
            {
                sp.sprite = forbidden;
            }
            else if (data.spiritType == "harvester")
            {
                sp.sprite = harvester;
            }
            else if (data.spiritType == "healer")
            {
                sp.sprite = healer;
            }
            else if (data.spiritType == "protector")
            {
                sp.sprite = protector;
            }
            else if (data.spiritType == "trickster")
            {
                sp.sprite = trickster;
            }
            else if (data.spiritType == "warrior")
            {
                sp.sprite = warrior;
            }
            else if (data.spiritType == "guardian")
            {
                sp.sprite = guardian;
            }
            else
            {
                sp.sprite = familiar;
            }
            sp.color = (data.owner == PlayerDataManager.playerData.instance ? Utilities.Blue : Color.white);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = spiritLesserScale;

        }
        else if (data.Type == MarkerType.duke)
        {
            if (data.degree == 1)
            {
                marker = SetupMarker(dukeWhite, pos, DukeScale, 13);
            }
            else if (data.degree == -1)
            {
                marker = SetupMarker(dukeShadow, pos, DukeScale, 13);
            }
            else if (data.degree == 0)
            {
                marker = SetupMarker(dukeGrey, pos, DukeScale, 13);
            }

            if (marker != null)
                marker.instance.GetComponent<MarkerScaleManager>().iniScale = DukeScale;
        }

        markerDot = SetupMarker(spiritDot, pos, witchDotScale, 3, 12);

        markerDot.instance.GetComponent<MarkerScaleManager>().iniScale = witchDotScale;
        marker.instance.GetComponent<MarkerScaleManager>().m = marker;
        markerDot.instance.GetComponent<MarkerScaleManager>().m = markerDot;
        var mList = new List<IMarker>();
        mList.Add(marker);
        mList.Add(markerDot);

        if (MapsAPI.Instance.zoom > 12)
        {
            markerDot.instance.gameObject.SetActive(false);
        }
        else
            marker.instance.gameObject.SetActive(false);

        return mList;
    }

    List<IMarker> CreateOther(Token data)
    {
        var pos = new Vector2(data.longitude, data.latitude);
        IMarker marker = null;
        //		print ("Adding Portal!");
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
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = portalLesserScale;
            //			print ("Adding Portal done");

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
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = summonEventScale;
        }
        else if (data.Type == MarkerType.herb)
        {
            marker = SetupMarker(herb, pos, botanicalScale, 13);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = botanicalScale;
        }
        else if (data.Type == MarkerType.tool)
        {
            marker = SetupMarker(tool, pos, botanicalScale, 13);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = botanicalScale;
        }
        else if (data.Type == MarkerType.silver)
        {
            marker = SetupMarker(silver, pos, botanicalScale, 13);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = botanicalScale;
        }
        else if (data.Type == MarkerType.gem)
        {
            marker = SetupMarker(gem, pos, GemScale, 13);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = GemScale;
        }
        else if (data.Type == MarkerType.lore)
        {
            marker = SetupMarker(lore, pos, 2.8f, 11);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = 2.8f;
        }

        //TODO ENABLE LOCATIONS

        else if (data.Type == MarkerType.location)
        {

            if (data.tier == 1)
            {
                marker = SetupMarker(level1Loc, pos, placeOfPowerScale, 13);
                marker.instance.GetComponent<MarkerScaleManager>().iniScale = placeOfPowerScale;
            }
            else if (data.tier == 2)
            {
                marker = SetupMarker(level2Loc, pos, placeOfPowerScale, 13);
                marker.instance.GetComponent<MarkerScaleManager>().iniScale = placeOfPowerScale;
            }
            else
            {
                marker = SetupMarker(level3Loc, pos, placeOfPowerScale, 13);
                marker.instance.GetComponent<MarkerScaleManager>().iniScale = placeOfPowerScale;
            }

        }

        else if (data.Type == MarkerType.silver)
        {
            marker = SetupMarker(tool, pos, botanicalScale, 13);
            marker.instance.GetComponent<MarkerScaleManager>().iniScale = botanicalScale;
        }
        marker.instance.GetComponent<MarkerScaleManager>().m = marker;

        var mList = new List<IMarker>();
        mList.Add(marker);
        return mList;
    }

    public void onClickMarker(IMarker m)
    {
        if (MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.position, m.position) > PlayerDataManager.attackRadius)
        {
            onClickMarkerFar(m);
            return;
        }

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
    }

    public void onClickMarkerFar(IMarker m)
    {
        if (!PlayerManager.Instance.fly || PlayerDataManager.playerData.energy <= 0 || LocationUIManager.isLocation)
            return;
        tokenFarAway.SetActive(false);
        tokenFarAway.SetActive(true);
        distanceSlider.maxValue = (float)MapsAPI.Instance.DistanceBetweenPointsD(m.position, PlayerManager.marker.position);
        distanceSlider.value = PlayerDataManager.attackRadius;
    }

    public void onClickMarkerFar(MarkerDataDetail m, bool physical)
    {
        if (!PlayerManager.Instance.fly || PlayerDataManager.playerData.energy <= 0 || LocationUIManager.isLocation)
            return;
        tokenFarAway.SetActive(false);
        tokenFarAway.SetActive(true);
        Vector2 playerPos = physical ? PlayerManager.physicalMarker.position : PlayerManager.marker.position;
        distanceSlider.maxValue = (float)MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(m.longitude, m.latitude), playerPos);
        distanceSlider.value = PlayerDataManager.attackRadius;
    }

    public void OnTokenSelect(Token Data, bool isLoc = false)
    {

        instanceID = Data.instance;
        selectedType = Data.Type;
        curGender = Data.male;
        TargetMarkerDetailData data = new TargetMarkerDetailData();
        data.target = instanceID;
        APIManager.Instance.PostData("map/select", JsonConvert.SerializeObject(data), GetResponse);
        if (!isLoc)
        {
            if (loadingObject != null)
                Destroy(loadingObject);
            if (selectedType == MarkerType.portal)
            {
                loadingObject = Utilities.InstantiateObject(loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT, .16f);
            }
            else if (selectedType == MarkerType.location)
            {
                LocationUIManager.locationID = Data.instance;
                loadingObject = Utilities.InstantiateObject(loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT, 2f);
            }
            else
            {
                loadingObject = Utilities.InstantiateObject(loadingObjectPrefab, MarkerSpawner.SelectedMarker3DT, 1f);
            }
        }
    }

    public void GetResponse(string response, int code)
    {
        Destroy(loadingObject);
        //		print (instanceID);
        //		print("Getting Data success " + response);
        //		print (code);
        if (code == 200)
        {
            var data = JsonConvert.DeserializeObject<MarkerDataDetail>(response);
            if (selectedType == MarkerType.lore)
            {
                QuestLogUI.Instance.ExploreQuestDone(data.id);
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

            if (selectedType == MarkerType.witch || selectedType == MarkerType.portal || selectedType == MarkerType.spirit || selectedType == MarkerType.location)
            {
                //				print ("Showing Card : " + selectedType );
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
        marker.scale = scale;
        marker.SetRange(rangeMin, rangeMax);
        return marker;
    }

    public void SetupStance(Transform witchMarker, Token data)
    {
        Dictionary<string, GameObject> names = new Dictionary<string, GameObject>();
        foreach (Transform item in witchMarker)
        {
            names[item.name] = item.gameObject;
        }

        if (StanceDict.ContainsKey(data.instance))
        {

            if (names.ContainsKey("spirit"))
                Destroy(names["spirit"]);

            if (data.physical)
            {
                if (StanceDict[data.instance])
                {
                    if (!names.ContainsKey("enemyP"))
                    {
                        var g = Utilities.InstantiateObject(physicalEnemy, witchMarker);
                        g.name = "enemyP";

                        if (names.ContainsKey("friendP"))
                            Destroy(names["friendP"]);
                        if (names.ContainsKey("enemyS"))
                            Destroy(names["enemyS"]);
                        if (names.ContainsKey("friendS"))
                            Destroy(names["friendS"]);

                    }
                }
                else
                {
                    if (!names.ContainsKey("friendP"))
                    {
                        var g = Utilities.InstantiateObject(physicalFriend, witchMarker);
                        g.name = "friendP";

                        if (names.ContainsKey("enemyP"))
                            Destroy(names["enemyP"]);
                        if (names.ContainsKey("enemyS"))
                            Destroy(names["enemyS"]);
                        if (names.ContainsKey("friendS"))
                            Destroy(names["friendS"]);

                    }
                }
            }
            else
            {
                if (StanceDict[data.instance])
                {
                    if (!names.ContainsKey("enemyS"))
                    {
                        var g = Utilities.InstantiateObject(spiritFormEnemy, witchMarker);
                        g.name = "enemyS";

                        if (names.ContainsKey("enemyP"))
                            Destroy(names["enemyP"]);
                        if (names.ContainsKey("friendP"))
                            Destroy(names["friendP"]);
                        if (names.ContainsKey("friendS"))
                            Destroy(names["friendS"]);

                    }
                }
                else
                {
                    if (!names.ContainsKey("friendS"))
                    {
                        var g = Utilities.InstantiateObject(spiritFormFriend, witchMarker);
                        g.name = "friendS";

                        if (names.ContainsKey("enemyP"))
                            Destroy(names["enemyP"]);
                        if (names.ContainsKey("friendP"))
                            Destroy(names["friendP"]);
                        if (names.ContainsKey("enemyS"))
                            Destroy(names["enemyS"]);

                    }
                }
            }
        }

        if (!data.physical && !names.ContainsKey("spirit"))
        {
            var g = Utilities.InstantiateObject(spiritForm, witchMarker);
            g.name = "spirit";
        }
    }
}

