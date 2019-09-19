using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Raincrow.Maps;
using TMPro;
using Raincrow.GameEventResponses;
using Raincrow.FTF;

public class MarkerSpawner : MarkerManager
{
    public static event System.Action<string, string, bool> OnImmunityChange;

    public static MarkerSpawner Instance { get; set; }
    //public static MarkerType selectedType;
    //public static Transform SelectedMarker3DT = null;
    //public static string instanceID = "";

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

    //[Header("MarkerEnergyRing")]
    //public Sprite[] EnergyRings;
    //private string lastEnergyInstance = "";

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
    private static SimplePool<Transform> m_PopPool;

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
        m_PopPool = new SimplePool<Transform>(unclaimedLoc.transform, 10);

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
            entry.Item1.Despawn(entry.Item2.GameObject.transform);
        }
        m_ToDespawn.Clear();

        m_DespawnCoroutine = null;
    }

    public IMarker AddMarker(Token Data)
    {
        if (Markers.ContainsKey(Data.instance))
        {
            foreach (var item in Markers[Data.instance])
            {
                item.Coords = new Vector2(Data.longitude, Data.latitude);
                item.Setup(Data);
                item.SetWorldPosition(MapsAPI.Instance.GetWorldPosition(Data.longitude, Data.latitude), 1);
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
        else if (Data.Type == MarkerType.ENERGY)
        {
            go = m_EnergyPool.Spawn().gameObject;
            go.name = $"[energy] {Data.instance}";
        }
        else if (Data.Type == MarkerType.PLACE_OF_POWER)
        {
            // #if UNITY_EDITOR
            //             Debug.Log("<color=red>place of power disabled</color>");
            // #endif
            //             return null;
            go = m_PopPool.Spawn().gameObject;
            go.name = $"[PlaceOfPower] {Data.instance}";
        }
        else
        {
            Debug.LogError(Data.type + " not impletemented");
            return null;
        }

        var pos = new Vector2(Data.longitude, Data.latitude);
        IMarker marker = MapsAPI.Instance.AddMarker(pos, go);
        marker.GameObject.SetActive(false);
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
            IMarker marker = Markers[ID][0];

            //marker.inMapView = false;
            marker.Interactable = false;

            //remove from dictionary
            Markers.Remove(ID);

            //despawn
            if (marker.Type == MarkerType.WITCH)
                Instance.m_ToDespawn.Add((m_WitchPool, marker));
            else if (marker.Type == MarkerType.SPIRIT)
                Instance.m_ToDespawn.Add((m_SpiritPool, marker));
            else if (marker.Type == MarkerType.HERB)
                Instance.m_ToDespawn.Add((m_HerbPool, marker));
            else if (marker.Type == MarkerType.GEM)
                Instance.m_ToDespawn.Add((m_GemPool, marker));
            else if (marker.Type == MarkerType.TOOL)
                Instance.m_ToDespawn.Add((m_ToolPool, marker));
            else if (marker.Type == MarkerType.ENERGY)
                Instance.m_ToDespawn.Add((m_EnergyPool, marker));

            if (Instance.m_DespawnTimer < despawnDelay)
                Instance.m_DespawnTimer = despawnDelay;

            if (Instance.m_DespawnCoroutine == null)
                Instance.m_DespawnCoroutine = Instance.StartCoroutine(Instance.DespawnCoroutine());

            MapsAPI.Instance.RemoveMarker(marker);

            if (m_Highlighting)
            {
                m_HighlightedMarkers.Remove(marker);
            }
        }
    }

    private void SetupWitch(WitchMarker marker, Token data)
    {
        if (!PlayerDataManager.IsFTF)
        {
            marker.Setup(data);

            //todo: setup stance (friend/enemy/coven)
            SetupStance(marker.GameObject.transform, data);
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

        var Data = m.Token;

        if (Data.Type == MarkerType.HERB || Data.Type == MarkerType.TOOL || Data.Type == MarkerType.GEM)
        {
            if (FirstTapManager.IsFirstTime("ingredients"))
            {
                FirstTapManager.Show("ingredients", () => onClickMarker(m));
                return;
            }
            PickUpCollectibleAPI.PickUpCollectable(m as CollectableMarker);
            return;
        }

        if (Data.Type == MarkerType.ENERGY)
        {
            if (FirstTapManager.IsFirstTime("energy"))
            {
                FirstTapManager.Show("energy", () => onClickMarker(m));
                return;
            }
            PickUpCollectibleAPI.CollectEnergy(m);
            return;
        }

        //show the basic available info, and waut for the map/select response to fill the details
        if (Data.Type == MarkerType.WITCH)
        {
            UIQuickCast.Open();
            UIQuickCast.UpdateCanCast(m, null);
            UIPlayerInfo.Show(m as WitchMarker, Data as WitchToken, UIQuickCast.Close);

            if (FirstTapManager.IsFirstTime("spellcasting"))
            {
                FirstTapManager.Show("spellcasting", () =>
                {
                    FirstTapManager.Show("quickcasting", null);
                });
            }
        }
        else if (Data.Type == MarkerType.SPIRIT)
        {
            UIQuickCast.Open();
            UIQuickCast.UpdateCanCast(m, null);
            UISpiritInfo.Show(m as SpiritMarker, Data as SpiritToken, UIQuickCast.Close);

            if (FirstTapManager.IsFirstTime("spellcasting"))
            {
                FirstTapManager.Show("spellcasting", () =>
                {
                    FirstTapManager.Show("quickcasting", () =>
                    {
                        FirstTapManager.Show("tier", null);
                    });
                });
            }
            else
            {
                if (FirstTapManager.IsFirstTime("tier"))
                {
                    FirstTapManager.Show("tier", null);
                }
            }
        }

        SoundManagerOneShot.Instance.PlayWhisperFX();
        GetMarkerDetails(Data.instance, (result, response) => GetResponse(m, Data.instance, response, result));
    }

    public static void GetMarkerDetails(string id, System.Action<int, string> callback)
    {
        APIManager.Instance.Get(
                    "character/select/" + id + "?selection=map",
                    "",
                    (response, result) => callback(result, response));
    }

    public void GetResponse(IMarker marker, string instance, string response, int code)
    {
        if (code != 200)
        {
            if (response == "1002")
                RemoveTokenHandler.ForceEvent(instance);
            else
                UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
        }

        switch (marker.Type)
        {
            case MarkerType.WITCH:

                bool witchSelected = UIPlayerInfo.WitchToken != null && UIPlayerInfo.WitchToken.instance == instance;

                if (code == 200)
                {
                    SelectWitchData_Map witch = JsonConvert.DeserializeObject<SelectWitchData_Map>(response);
                    witch.token = marker.Token as WitchToken;

                    if (witchSelected)
                    {
                        UIPlayerInfo.SetupDetails(witch);
                        UIQuickCast.UpdateCanCast(marker, witch);
                    }
                }
                break;

            case MarkerType.SPIRIT:

                bool spiritSelected = UISpiritInfo.SpiritToken != null && UISpiritInfo.SpiritToken.instance == instance;

                if (code == 200)
                {
                    SelectSpiritData_Map spirit = JsonConvert.DeserializeObject<SelectSpiritData_Map>(response);
                    spirit.token = marker.Token as SpiritToken;

                    if (spiritSelected)
                    {
                        UISpiritInfo.SetupDetails(spirit);
                        UIQuickCast.UpdateCanCast(marker, spirit);
                    }

                    if (spirit.state == "dead")
                        RemoveTokenHandler.ForceEvent(instance);
                }
                break;

            case MarkerType.PLACE_OF_POWER:
                LoadPOPManager.EnterPOP(instance);
                break;

            default:
                Debug.LogError("Token selection not implemented for " + marker.Type);
                break;
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
        if (target == PlayerDataManager.playerData.instance)
        {
            if (PlayerDataManager.playerData.immunities.Contains(caster))
                PlayerDataManager.playerData.immunities.Remove(caster);
        }
        else
        {

        }

        OnImmunityChange?.Invoke(caster, target, false);
    }


    public static Sprite GetSpiritTierSprite(string spiritType)
    {
        if (Instance == null)
            return null;

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
            PlayerManager.marker.GameObject.SetActive(m_StreetLevel);
            //  Debug.Log("setting playerMarker");
            PlayerManager.marker.GameObject.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
            PlayerManager.marker.AvatarTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }

        if (m_Highlighting)
        {
            foreach (IMarker _marker in m_HighlightedMarkers)
            {
                if (_marker != null && _marker.GameObject != null && _marker != PlayerManager.marker)
                    UpdateMarker(_marker);
            }
        }
        else
        {
            IMarker aux;
            foreach (var item in Markers)
            {
                aux = item.Value[0];
                UpdateMarker(aux);
            }
        }
    }

    public void UpdateMarker(IMarker marker)
    {
        UpdateMarker(marker, m_PortaitMode, m_StreetLevel, m_MarkerScale);
    }

    public static void UpdateMarker(IMarker marker, bool portraitMode, bool streetLevel, float scale)
    {
        if (streetLevel && MapsAPI.Instance.IsPointInsideView(marker.GameObject.transform.position))
        {
            if (portraitMode)
                marker.EnablePortait();
            else
                marker.EnableAvatar();

            if (!marker.inMapView)
            {
                marker.SetAlpha(0);
                marker.SetAlpha(1, 1f);
                marker.GameObject.SetActive(true);
                marker.inMapView = true;
            }
            UpdateMarker(marker, scale);
        }
        else if (marker.inMapView)
        {
            marker.inMapView = false;
            marker.SetAlpha(0, 1f, () => marker.GameObject.SetActive(false));
        }
    }

    public static void UpdateMarker(IMarker marker, float scale)
    {
        marker.GameObject.transform.localScale = new Vector3(scale, scale, scale);
        marker.AvatarTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
    }

    private static bool m_Highlighting = false;
    private static List<IMarker> m_HighlightedMarkers = new List<IMarker>();

    public static void HighlightMarker(List<IMarker> targets)
    {
        m_Highlighting = targets.Count > 0;

        if (targets == null)
            targets = new List<IMarker>();

        m_HighlightedMarkers = targets;
        //MapsAPI.Instance.EnableBuildingIcons(!highlight);

        IMarker aux;
        foreach (var item in Markers)
        {
            aux = item.Value[0];
            if (aux.inMapView && !targets.Contains(aux))
                aux.SetAlpha(m_Highlighting ? 0.05f : 1, 1f);
        }

        foreach (IMarker _marker in targets)
        {
            _marker.inMapView = true;
            _marker?.SetAlpha(1, 1f);
        }
    }

    //click controller

    private Vector2 m_MouseDownPosition;
    private void Update()
    {
        bool inputUp = false;
        bool inputDown = false;
        if (LocationIslandController.isInBattle) return;
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
