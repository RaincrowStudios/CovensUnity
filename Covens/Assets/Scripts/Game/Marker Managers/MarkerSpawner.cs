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
        List<Token> newMarkers = new List<Token>();
        HashSet<string> existedMarkers = new HashSet<string>();

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
                newMarkers.Add(item);
            }
        }

        List<IMarker> deleteList = new List<IMarker>();
        foreach (var item in Markers)
        {
            if (!existedMarkers.Contains(item.Key))
            {
                deleteList.Add(item.Value[0]);
            }
        }

        DeleteAllMarkers(deleteList.ToArray());

        StartCoroutine(CreateMarkersHelper(newMarkers));
    }

    IEnumerator CreateMarkersHelper(List<Token> Data)
    {
        foreach (var item in Data)
        {
            AddMarker(item);
            //yield return 0;
        }
        yield return 1;

        UpdateMarkers();
    }

    void callzoom()
    {
        EventManager.Instance.CallSmoothZoom();
    }

    public void AddMarker(Token Data, bool updateVisuals = false)
    {
        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                //item.SetPosition(Data.longitude, Data.latitude);
                if(item.inMapView)
                {
                    OnMapTokenMove.MoveMarker(item, Data.instance, Data.longitude, Data.latitude);
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
            marker.gameObject.transform.GetChild(3).GetComponentInChildren<TextMeshPro>().text = data.amount.ToString();
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
        else if (Data.Type == MarkerType.portal)
        {
            UIPortalInfo.Instance.Show(m);
        }
        else if (Data.Type == MarkerType.location)
        {
            ShowSelectionCard.Instance.Show(m);
        }

    }

    public void OnTokenSelect(Token Data, bool isLoc = false)
    {
        instanceID = Data.instance;
        selectedType = Data.Type;
        curGender = Data.male;

        TargetMarkerDetailData data = new TargetMarkerDetailData();
        data.target = instanceID;
        SoundManagerOneShot.Instance.PlayItemAdded();
        if (selectedType == MarkerType.energy)
        {
            var g = Instantiate(energyParticles);
            g.transform.position = SelectedMarker3DT.GetChild(3).position;
            LeanTween.scale(SelectedMarker3DT.gameObject, Vector3.zero, .4f);
            var energyData = new { target = Data.instance };
            APIManager.Instance.PostData("map/pickup", JsonConvert.SerializeObject(energyData), (string s, int r) =>
            {
                print(s);

                if (r == 200)
                {
                    SoundManagerOneShot.Instance.PlayEnergyCollect();
                    PlayerDataManager.playerData.energy += Data.amount;
                    PlayerManagerUI.Instance.UpdateEnergy();
                }
                else
                {

                }
            });
        }
        else
        {
            APIManager.Instance.PostData("map/select", JsonConvert.SerializeObject(data), (response, result) => GetResponse(Data.instance, response, result));
        }
    }

    public void GetResponse(string instance, string response, int code)
    {
        if (code == 200)
        {
            var data = JsonConvert.DeserializeObject<MarkerDataDetail>(response);
            if (selectedType == MarkerType.lore)
            {
                QuestsController.instance.ExploreQuestDone(data.id);
                return;
            }

            SelectedMarker = data;
            SelectedMarker.male = curGender;

            if (selectedType == MarkerType.witch)
            {
                UpdateMarker(instance, data);

                //fill the details
                if (UIPlayerInfo.Instance.Witch.displayName == data.displayName)
                    UIPlayerInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.spirit)
            {
                //if (UISpiritInfo.Instance.Spirit.owner == data.owner)
                UISpiritInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.portal)
            {
                UIPortalInfo.Instance.SetupDetails(data);
            }
            else if (selectedType == MarkerType.location)
            {
                //  ShowSelectionCard.Instance.SetupDetails(MarkerType.location, data);
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
            marker.EnablePortait();
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

    //click controller

    private float m_MouseDownTime;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_MouseDownTime = Time.time;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //todo: also check delta
            float time = Time.time - m_MouseDownTime;
            if (time > 0.2f)
                return;

            if (Input.touchCount > 0)
            {
                // only click if touching with multiple fingers
                if (Input.touchCount == 1)
                {
                    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                        return;
                }
                else
                {
                    return;
                }
            }
            else  // in editor
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    return;
            }

            Camera cam;
            int layerMask;
            if (MapController.Instance.isStreet)
            {
                cam = MapController.Instance.m_StreetMap.camera;
            }
            else
            {
                cam = MapController.Instance.m_WorldMap.camera;
            }

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

            //Plane plane = new Plane(Vector3.up, Vector3.zero);
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            //float distance;

            //if (plane.Raycast(ray, out distance))
            //{
            //    Vector3 point = ray.GetPoint(distance);
            //    Debug.LogError(point);

            //    RaycastHit[] hits = Physics.SphereCastAll(point, 5, Vector3.forward, 10, 1 << 20);
            //    if (hits.Length > 0)
            //    {
            //        float minDist = Mathf.Infinity;
            //        int index = 0;

            //        for (int i = 0; i < hits.Length; i++)
            //        {
            //            float _dist = Vector3.Distance(hits[i].transform.position, point);
            //            if (_dist < minDist)
            //            {
            //                minDist = _dist;
            //                index = i;
            //            }
            //        }

            //        IMarker marker = hits[index].transform.GetComponentInParent<IMarker>();
            //        if (marker != null)
            //        {
            //            marker.OnClick?.Invoke(marker);
            //            return;
            //        }
            //    }
            //}
        }
    }
}

