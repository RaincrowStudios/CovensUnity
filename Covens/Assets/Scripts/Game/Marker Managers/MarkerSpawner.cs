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

public class MarkerSpawner : MonoBehaviour
{
    public enum MarkerType
    {
        NONE = 0,
        PORTAL = 1,
        SPIRIT = 2,
        DUKE = 3,
        PLACE_OF_POWER = 4,
        WITCH = 5,
        SUMMONING_EVENT = 6,
        GEM = 7,
        HERB = 8,
        TOOL = 9,
        SILVER = 10,
        LORE = 11,
        ENERGY = 12,
        BOSS = 13,
        LOOT = 14,
    }

    public enum MarkerSchool
    {
        SHADOW = -1,
        GREY = 0,
        WHITE = 1
    }

    public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();

    public static Dictionary<string, WorldBossMarker> Bosses = new Dictionary<string, WorldBossMarker>();
    
    public event System.Action<string, string, bool> OnImmunityChange;

    public event System.Action<IMarker> OnSelectMarker;
    public event System.Action<string, CharacterMarkerData> OnReceiveMarkerData;

    public static MarkerSpawner Instance { get; set; }

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

    [Header("Collectibles")]
    public GameObject herb;
    public GameObject tool;
    public GameObject gem;
    public GameObject energyIcon;

    [Header("World Boss")]
    [SerializeField] private GameObject m_WorldBossPrefab;
    [SerializeField] private GameObject m_LootPrefab;

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
    private static SimplePool<Transform> m_BossPool;
    private static SimplePool<Transform> m_LootPool;


    public static float m_MarkerScale { get; private set; }
    public static bool m_PortaitMode { get; private set; }
    private static bool m_StreetLevel;
    private const float MARKER_SCALE_MIN = 1;
    private const float MARKER_SCALE_MAX = 2;

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
        m_EnergyPool = new SimplePool<Transform>(energyIcon.transform, 5);
        m_PopPool = new SimplePool<Transform>(unclaimedLoc.transform, 0);
        m_BossPool = new SimplePool<Transform>(m_WorldBossPrefab.transform, 0);
        m_LootPool = new SimplePool<Transform>(m_LootPrefab.transform, 0);
    }

    private void Start()
    {
        //init the map/markers variables
        UpdateMarkerProperties();
        MapsAPI.Instance.OnCameraUpdate += (a, b, c) => UpdateMarkerProperties();
        PlayerManager.onStartFlight += UpdateMarkerProperties;
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
                item.SetWorldPosition(MapsAPI.Instance.GetWorldPosition(Data.longitude, Data.latitude), 0);
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
            Debug.Log("pop disabled");
            return null;
            //go = m_PopPool.Spawn().gameObject;
            //go.name = $"[PlaceOfPower] {Data.instance}";
        }
        else if (Data.Type == MarkerType.BOSS)
        {
            go = m_BossPool.Spawn().gameObject;
            go.name = $"[WorldBoss] {Data.instance}";
        }
        else if (Data.Type == MarkerType.LOOT)
        {
            go = m_LootPool.Spawn().gameObject;
            go.name = $"[Loot] {Data.instance}";
        }
        else
        {
            Debug.LogError(Data.type + " not impletemented");
            return null;
        }

        var pos = new Vector2(Data.longitude, Data.latitude);
        IMarker marker = MapsAPI.Instance.AddMarker(pos, go);
        marker.Setup(Data);
        marker.OnClick += OnClickMarker;

        UpdateMarker(marker);

        Markers.Add(Data.instance, new List<IMarker> { marker });

        return marker;
    }
    
    public static IMarker GetMarker(string instance)
    {
        if (string.IsNullOrEmpty(instance))
            return null;

        if (PlayerDataManager.playerData.instance == instance)
            return PlayerManager.witchMarker;

        if (Markers.ContainsKey(instance))
            return Markers[instance][0];

        return null;
    }

    public static void DeleteMarker(string ID, float despawnDelay = 2)
    {
        if (!Markers.ContainsKey(ID))
            return;


        IMarker marker = Markers[ID][0];

        //marker.inMapView = false;
        marker.Interactable = false;

        //remove from dictionary
        Markers.Remove(ID);

        //remove from mapsapi tracker
        MapsAPI.Instance.RemoveMarker(marker);

        ////remove from highlight list
        //if (m_Highlighting)
        //    m_HighlightedMarkers.Remove(marker);

        SimplePool<Transform> pool = null;
        if (marker.Type == MarkerType.WITCH)
            pool = m_WitchPool; //Instance.m_ToDespawn.Add((m_WitchPool, marker));
        else if (marker.Type == MarkerType.SPIRIT)
            pool = m_SpiritPool; //Instance.m_ToDespawn.Add((m_SpiritPool, marker));
        else if (marker.Type == MarkerType.HERB)
            pool = m_HerbPool; //Instance.m_ToDespawn.Add((m_HerbPool, marker));
        else if (marker.Type == MarkerType.GEM)
            pool = m_GemPool; //Instance.m_ToDespawn.Add((m_GemPool, marker));
        else if (marker.Type == MarkerType.TOOL)
            pool = m_ToolPool; // Instance.m_ToDespawn.Add((m_ToolPool, marker));
        else if (marker.Type == MarkerType.ENERGY)
            pool = m_EnergyPool; //Instance.m_ToDespawn.Add((m_EnergyPool, marker));
        else if (marker.Type == MarkerType.BOSS)
            pool = m_BossPool;
        else if (marker.Type == MarkerType.LOOT)
            pool = m_LootPool;
        else if (marker.Type == MarkerType.PLACE_OF_POWER)
        {
            int? degree = (marker.Token as PopToken).lastOwnedBy?.degree;
            // if (!degree.HasValue)
            pool = m_PopPool;
            // else if (degree.Value < 0)
            //     pool = m_PopPoolShadow;
            // else if (degree.Value > 0)
            //     pool = m_PopPoolWhite;
            // else
            //     pool = m_PopPoolGrey;
        }
        else
        {
            Debug.LogError("no pool for " + marker.Name + " - " + marker.Type);
        }

        if (pool == null)
        {
            marker.OnDespawn();
        }
        else if (despawnDelay == 0)
        {
            marker.OnDespawn();
            pool.Despawn(marker.GameObject.transform);
        }
        else
        {
            marker.OnWillDespawn();

            if (Instance.m_DespawnTimer < despawnDelay)
                Instance.m_DespawnTimer = despawnDelay;

            if (Instance.m_DespawnCoroutine == null)
                Instance.m_DespawnCoroutine = Instance.StartCoroutine(Instance.DespawnCoroutine());

            Instance.m_ToDespawn.Add((pool, marker));
        }
    }

    private void OnClickMarker(IMarker m)
    {
        Debug.Log("OnClickMarker " + m.GameObject.name);

        //if (!UIStateManager.isMain)
        //    return;

        if (PlayerManager.isFlying || PlayerDataManager.playerData.energy <= 0)
        {
            return;
        }

        OnSelectMarker?.Invoke(m);

        var Data = m.Token;

        if (Data.Type == MarkerType.HERB || Data.Type == MarkerType.TOOL || Data.Type == MarkerType.GEM)
        {
            if (FirstTapManager.IsFirstTime("ingredients"))
            {
                FirstTapManager.Show("ingredients", () => OnClickMarker(m));
                return;
            }
            PickUpCollectibleAPI.PickUpCollectable(m as CollectableMarker);
            return;
        }

        if (Data.Type == MarkerType.ENERGY)
        {
            if (FirstTapManager.IsFirstTime("energy"))
            {
                FirstTapManager.Show("energy", () => OnClickMarker(m));
                return;
            }
            PickUpCollectibleAPI.CollectEnergy(m as EnergyMarker);
            return;
        }
        
        if (Data.Type == MarkerType.LOOT)
        {
            PickUpCollectibleAPI.PickUpLoot(m as LootMarker);
            return;
        }

        //show the basic available info, and waut for the map/select response to fill the details
        if (Data.Type == MarkerType.BOSS)
        {
            //ui is opened by UIMain listener
            UIQuickCast.UpdateTarget(m, null);
        }
        else if (Data.Type == MarkerType.WITCH)
        {
            //if (!WitchMarker.DisableSpriteGeneration)
            (m as WitchMarker).GenerateAvatar(null, true);
            UIQuickCast.UpdateTarget(m, null);
            UIPlayerInfo.Show(m as WitchMarker, Data as WitchToken, UIQuickCast.Close);
        }
        else if (Data.Type == MarkerType.SPIRIT)
        {
            UIQuickCast.UpdateTarget(m, null);
            UISpiritInfo.Show(m as SpiritMarker, Data as SpiritToken, UIQuickCast.Close);
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
                    OnReceiveMarkerData?.Invoke(instance, witch);

                    if (witchSelected)
                    {
                        UIPlayerInfo.SetupDetails(witch);
                        UIQuickCast.UpdateTarget(marker, witch);
                    }
                }
                break;

            case MarkerType.SPIRIT:

                bool spiritSelected = UISpiritInfo.SpiritToken != null && UISpiritInfo.SpiritToken.instance == instance;

                if (code == 200)
                {
                    SelectSpiritData_Map spirit = JsonConvert.DeserializeObject<SelectSpiritData_Map>(response);
                    spirit.token = marker.Token as SpiritToken;
                    OnReceiveMarkerData?.Invoke(instance, spirit);

                    if (spiritSelected)
                    {
                        UISpiritInfo.SetupDetails(spirit);
                        UIQuickCast.UpdateTarget(marker, spirit);
                    }

                    if (spirit.state == "dead")
                        RemoveTokenHandler.ForceEvent(instance);
                }
                break;

            case MarkerType.PLACE_OF_POWER:
                Debug.LogError("place of power:\n" + response);
                break;

            case MarkerType.BOSS:

                BossMarkerData boss = JsonConvert.DeserializeObject<BossMarkerData>(response);
                OnReceiveMarkerData?.Invoke(instance, boss);
                UIQuickCast.UpdateTarget(marker, boss);
                break;

            default:
                Debug.LogError("Token selection not implemented for " + marker.Type);
                break;
        }
    }

    /// <summary>
    /// Returns true if the target is immune to the player.
    /// </summary>
    public static bool IsTargetImmune(Token token)
    {
        if (MarkerManagerAPI.IsGarden)
            return false;

        return PlayerDataManager.playerData.immunities.Contains(token.instance);
    }

    public void AddImmunity(string spellCaster, string spellTarget)
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

    public void RemoveImmunity(string caster, string target)
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

    public static void ClearImmunities()
    {
        PlayerDataManager.playerData.immunities.Clear();
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

    private static void UpdateMarkerProperties()
    {
        m_PortaitMode = MapsAPI.Instance.streetLevelNormalizedZoom > 0.6f;
        m_StreetLevel = MapsAPI.Instance.streetLevel;
        m_MarkerScale = MARKER_SCALE_MAX * MapsAPI.Instance.normalizedZoom + (MARKER_SCALE_MIN - MapsAPI.Instance.normalizedZoom);
    }

    public void UpdateMarkers()
    {
        if (PlayerManager.marker != null)
        {
            PlayerManager.marker.GameObject.SetActive(true);
            PlayerManager.marker.GameObject.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
            PlayerManager.marker.AvatarTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
        }

        //if (m_Highlighting)
        //{
        //    foreach (IMarker _marker in m_HighlightedMarkers)
        //    {
        //        if (_marker != null && _marker.GameObject != null && _marker != PlayerManager.marker)
        //            UpdateMarker(_marker);
        //    }
        //}
        //else
        //{
        IMarker aux;
        foreach (var item in Markers)
        {
            aux = item.Value[0];
            UpdateMarker(aux);
        }
        //}
    }

    public void UpdateMarker(IMarker marker)
    {
        UpdateMarker(marker, m_PortaitMode, m_StreetLevel, m_MarkerScale);
    }

    public static void UpdateMarker(IMarker marker, bool portraitMode, bool streetLevel, float scale)
    {
        UpdateMarker(marker, scale);

        if (streetLevel)
        {
            if (MapsAPI.Instance.IsPointInsideView(marker.GameObject.transform.position, 15))
            {
                if (portraitMode)
                    marker.EnablePortait();
                else
                    marker.EnableAvatar();

                if (!marker.inMapView)
                {
                    marker.inMapView = true;
                    marker.OnEnterMapView();
                }
            }
            else if (marker.inMapView)
            {
                marker.inMapView = false;
                marker.OnLeaveMapView();
            }
        }
        else
        {
            if (marker.inMapView)
                marker.OnLeaveMapView();
        }
    }

    public static void UpdateMarker(IMarker marker, float scale)
    {
        marker.GameObject.transform.localScale = new Vector3(scale, scale, scale);
        marker.AvatarTransform.rotation = MapsAPI.Instance.camera.transform.rotation;
    }

    public static void HighlightMarkers(List<MuskMarker> targets)
    {
        MapCameraUtils.HighlightMarkers(targets);
    }
    
    public static void ApplyStatusEffect(string targetId, string casterId, StatusEffect effect)
    {
        IMarker caster = GetMarker(casterId);
        IMarker target = GetMarker(targetId);

        if (target != null && target.Token is CharacterToken)
        {
            CharacterToken characterToken = target.Token as CharacterToken;

            //init if empety
            if (characterToken.effects == null)
                characterToken.effects = new List<StatusEffect>();

            //remove the old effect if it already exists
            foreach (StatusEffect item in characterToken.effects)
            {
                if (item.spell == effect.spell)
                {
                    characterToken.effects.Remove(item);
                    item.CancelExpiration();
                    break;
                }
            }

            //add the new effect
            characterToken.effects.Add(effect);

            //schedule the local expiration
            effect.ScheduleExpiration(() => ExpireStatusEffectHandler.ExpireEffect(targetId, effect));

            //if (effect.spell == "spell_channeling")
            //    SpellChanneling.SpawnFX(target, effect);

            target.OnApplyStatusEffect(effect);
        }

        SpellCastHandler.OnApplyEffect?.Invoke(targetId, casterId, effect);

        if (targetId == PlayerDataManager.playerData.instance)
            PlayerConditionManager.OnApplyEffect(effect, caster);
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
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1 << 20 | 1 << 22))
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
