using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;
using TMPro;
using Raincrow.GameEventResponses;

public class MarkerSpawner : MarkerManager
{
    public static event System.Action<string, string, bool> OnImmunityChange;

    public static MarkerSpawner Instance { get; set; }
    public static MarkerType selectedType;
    public static Transform SelectedMarker3DT = null;
    public static string instanceID = "";

    [Header("Witch")]
    public GameObject witchIcon;
    
    [Header("Spirits")]
    public GameObject spiritIcon;

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
    public GameObject unclaimedLoc;
    public GameObject greyLoc;
    public GameObject shadowLoc;
    public GameObject whiteLoc;

    [Header("Collectibles")]
    public GameObject herb;
    public GameObject tool;
    public GameObject gem;
    public GameObject energyIcon;
    
    [Header("MarkerEnergyRing")]
    public Sprite[] EnergyRings;
    private string lastEnergyInstance = "";
    
    private Dictionary<string, Sprite> m_SpiritIcons;
    private List<(SimplePool<Transform>, IMarker)> m_ToDespawn = new List<(SimplePool<Transform>, IMarker)>();
    private float m_DespawnTimer;
    private Coroutine m_DespawnCoroutine;

    private static SimplePool<Transform> m_WitchPool;
    private static SimplePool<Transform> m_SpiritPool;
    private static SimplePool<Transform> m_HerbPool;
    private static SimplePool<Transform> m_ToolPool;
    private static SimplePool<Transform> m_GemPool;
    private static SimplePool<Transform> m_EnergyPool;

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

        m_WitchPool = new SimplePool<Transform>(witchIcon.transform, 10);
        m_SpiritPool = new SimplePool<Transform>(spiritIcon.transform, 10);
        m_HerbPool = new SimplePool<Transform>(herb.transform, 10);
        m_GemPool = new SimplePool<Transform>(gem.transform, 10);
        m_ToolPool = new SimplePool<Transform>(tool.transform, 10);
        m_EnergyPool = new SimplePool<Transform>(energyIcon.transform, 10);

        //init the map/markers variables
        UpdateProperties();
        MapsAPI.Instance.OnCameraUpdate += (a, b, c) => UpdateProperties();
        PlayerManager.onStartFlight += UpdateProperties;
    }

    private IEnumerator DespawnCoroutine()
    {
        while (m_DespawnTimer > 0)
        {
            m_DespawnTimer -= Time.deltaTime;
            yield return null;
        }

        foreach (var entry in m_ToDespawn)
        {
            //Debug.Log("<color=magenta>despawning " + entry.Item2.gameObject.name + "</color>");
            entry.Item2.OnDespawn();
            entry.Item1.Despawn(entry.Item2.gameObject.transform);
        }
        m_ToDespawn.Clear();

        m_DespawnCoroutine = null;
    }

    public IMarker AddMarker(Token Data)
    {
        if (LoginUIManager.isInFTF)
            return null;

        //double distance = MapsAPI.Instance.DistanceBetweenPointsD(new Vector2(Data.longitude, Data.latitude), PlayerManager.marker.coords);
        //if (distance >= PlayerDataManager.DisplayRadius)
        //{
        //    return null;
        //}

        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                item.coords = new Vector2(Data.longitude, Data.latitude);
                item.customData = Data;
                item.Setup(Data);
                UpdateMarker(item);
            }
            return Markers[Data.instance][0];
        }

        GameObject go = null;

        if (Data.Type == MarkerType.WITCH)
        {
            go = m_WitchPool.Spawn().gameObject;
            go.name = "[witch] " + (Data as WitchToken).displayName + " [" + Data.instance + "]";
        }
        else if (Data.Type == MarkerType.SPIRIT)
        {
            go = m_SpiritPool.Spawn().gameObject;
            go.name = "[spirit] " + (Data as SpiritToken).spiritId + " [" + Data.instance + "]";
        }
        else if (Data.Type == MarkerType.HERB)
        {
            go = m_HerbPool.Spawn().gameObject;
            go.name = $"[herb] {Data.instance}";
        }
        else if (Data.Type == MarkerType.TOOL)
        {
            go = m_ToolPool.Spawn().gameObject;
            go.name = $"[tool] {Data.instance}";
        }
        else if (Data.Type == MarkerType.GEM)
        {
            go = m_GemPool.Spawn().gameObject;
            go.name = $"[gem] {Data.instance}";
        }
        else
        {
            Debug.LogError(Data.type + " not impletemented");
            return null;
        }

        var pos = new Vector2(Data.longitude, Data.latitude);
        IMarker marker = MapsAPI.Instance.AddMarker(pos, go);
        marker.customData = Data;
        marker.Setup(Data);
        marker.OnClick += onClickMarker;

        UpdateMarker(marker);

        Markers.Add(Data.instance, new List<IMarker> { marker });

        return marker;
    }

    public static void DeleteMarker(string ID, float despawnDelay = 2)
    {
        if (Markers.ContainsKey(ID))
        {
            //remove from dictionary
            Markers.Remove(ID);

            IMarker marker = Markers[ID][0];
            marker.inMapView = false;
            marker.interactable = false;
            
            //despawn
            if (marker.type == MarkerType.WITCH)
                Instance.m_ToDespawn.Add((m_WitchPool, marker));
            else if (marker.type == MarkerType.SPIRIT)
                Instance.m_ToDespawn.Add((m_SpiritPool, marker));
            else if (marker.type == MarkerType.HERB)
                Instance.m_ToDespawn.Add((m_HerbPool, marker));
            else if (marker.type == MarkerType.GEM)
                Instance.m_ToDespawn.Add((m_GemPool, marker));
            else if (marker.type == MarkerType.TOOL)
                Instance.m_ToDespawn.Add((m_ToolPool, marker));

            if (Instance.m_DespawnTimer < despawnDelay)
                Instance.m_DespawnTimer = despawnDelay;

            if (Instance.m_DespawnCoroutine == null)
                Instance.m_DespawnCoroutine =  Instance.StartCoroutine(Instance.DespawnCoroutine());

            MapsAPI.Instance.RemoveMarker(marker);
        }
    }

    private void SetupWitch(WitchMarker marker, Token data)
    {
        if (!LoginUIManager.isInFTF)
        {
            marker.Setup(data);

            //todo: setup stance (friend/enemy/coven)
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
        instanceID = Data.instance;
        selectedType = Data.Type;

        //show the basic available info, and waut for the map/select response to fill the details

        if (Data.Type == MarkerType.WITCH)
        {
            UIPlayerInfo.Instance.Show(m as WitchMarker, Data as WitchToken);
        }
        else if (Data.Type == MarkerType.SPIRIT)
        {
            UISpiritInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.PORTAL)
        {
            UIPortalInfo.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.PLACE_OF_POWER)
        {
            UIPopInfoNew.Instance.Show(m, Data);
        }
        else if (Data.Type == MarkerType.HERB || Data.Type == MarkerType.TOOL || Data.Type == MarkerType.GEM)
        {
            PickUpCollectibleAPI.PickUpCollectable(m as CollectableMarker);
            return;
        }

        TargetMarkerDetailData data = new TargetMarkerDetailData();
        data.target = instanceID;

        if (selectedType == MarkerType.ENERGY && lastEnergyInstance != instanceID)
        {
            if (PlayerDataManager.playerData.energy >= (PlayerDataManager.playerData.baseEnergy * 2))
            {
                UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("energy_full"));
                return;
            }

            //var g = Instantiate(energyParticles);
            //g.transform.position = SelectedMarker3DT.GetChild(1).position;
            LeanTween.scale(SelectedMarker3DT.gameObject, Vector3.zero, .3f).setOnComplete(() =>
            {
                DeleteMarker(instanceID);
            });
            var energyData = new { target = Data.instance };
            APIManager.Instance.Post("map/pickup", JsonConvert.SerializeObject(energyData), (string s, int r) =>
            {
                Debug.Log(s);

                if (r == 200 && s != "")
                {

                    UIEnergyBarGlow.Instance.Glow();
                    SoundManagerOneShot.Instance.PlayEnergyCollect();
                    PlayerDataManager.playerData.energy += (Data as CollectableToken).amount;
                    if (PlayerDataManager.playerData.energy >= (PlayerDataManager.playerData.baseEnergy * 2))
                        PlayerDataManager.playerData.energy = PlayerDataManager.playerData.baseEnergy * 2;
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
                APIManager.Instance.Post("location/select", JsonConvert.SerializeObject(data), (response, result) => GetResponse(m, instanceID, response, result));
            else
                APIManager.Instance.Get(
                    "character/select/"+Data.instance+"?selection=map",
                    "",
                    (response, result) => GetResponse(m, instanceID, response, result));
        }
    }

    public void GetResponse(IMarker marker, string instance, string response, int code)
    {
        if (code == 200)
        {
            switch (marker.type)
            {
                case MarkerType.WITCH:
                    FirstTapVideoManager.Instance.CheckSpellCasting();

                    MapWitchData witch = JsonConvert.DeserializeObject<MapWitchData>(response);
                    witch.token = marker.token as WitchToken;

                    if (UIPlayerInfo.isShowing && UIPlayerInfo.Instance.Witch.instance == instance)
                        UIPlayerInfo.Instance.SetupDetails(witch);
                    break;

                case MarkerType.SPIRIT:
                    FirstTapVideoManager.Instance.CheckSpellCasting();
                    MapSpiritData spirit = JsonConvert.DeserializeObject<MapSpiritData>(response);
                    spirit.token = marker.token as SpiritToken;

                    if (UISpiritInfo.isOpen && UISpiritInfo.Instance.Spirit.instance == instance)
                        UISpiritInfo.Instance.SetupDetails(spirit);

                    if (spirit.state == "dead")
                        RemoveTokenHandler.ForceEvent(instance);

                    break;

                case MarkerType.PLACE_OF_POWER:
                    LocationMarkerData location = JsonConvert.DeserializeObject<LocationMarkerData>(response);
                    UIPopInfoNew.SetupDetails(location, instance);
                    break;
                case MarkerType.PORTAL:
                    PortalMarkerData portal = JsonConvert.DeserializeObject<PortalMarkerData>(response);

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


    public void SetupStance(Transform witchMarker, Token data)
    {
    }

    /// <summary>
    /// Returns true if the target is immune to the player.
    /// </summary>
    public static bool IsTargetImmune(WitchToken token)
    {
        if (PlaceOfPower.IsInsideLocation)
            return false;

        return PlayerDataManager.playerData.immunities.Contains(token.instance);
    }

    public static void AddImmunity(string spellCaster, string spellTarget)
    {
        if (spellCaster == PlayerDataManager.playerData.instance)
        {
            PlayerDataManager.playerData.immunities.Add(spellTarget);
        }
        else
        {

        }

        OnImmunityChange?.Invoke(spellCaster, spellTarget, true);
    }

    public static void RemoveImmunity(string caster, string target)
    {
        if (caster == PlayerDataManager.playerData.instance)
        {
            if (PlayerDataManager.playerData.immunities.Contains(target))
                PlayerDataManager.playerData.immunities.Remove(target);
        }
        else
        {

        }

        OnImmunityChange?.Invoke(caster, target, false);
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
            List<List<IMarker>> markerList = new List<List<IMarker>>(Markers.Values);
            foreach (List<IMarker> _marker in markerList)
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
            UpdateMarker(marker, scale);
        }
        else if (marker.inMapView)
        {
            marker.inMapView = false;
            marker.SetAlpha(0, 1f, () => marker.gameObject.SetActive(false));
        }
    }

    public static void UpdateMarker(IMarker marker, float scale)
    {
        marker.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        marker.characterTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
    }

    private static bool m_Highlighting = false;
    private static List<IMarker> m_HighlightedMarkers = new List<IMarker>();

    public static void HighlightMarker(List<IMarker> targets, bool highlight)
    {
        if (highlight && PlaceOfPower.IsInsideLocation)
            return;

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
        {
            if (_marker.inMapView)
                _marker?.SetAlpha(1, 1f);
        }
    }

    public static void HideVisibleMarkers(float time, bool player)
    {
        List<List<IMarker>> markersList = new List<List<IMarker>>(Markers.Values);

        IMarker marker;
        foreach (List<IMarker> _marker in markersList)
        {
            marker = _marker[0];
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
