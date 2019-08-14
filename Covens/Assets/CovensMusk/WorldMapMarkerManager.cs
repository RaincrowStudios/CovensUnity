using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorldMapMarker;

public class WorldMapMarkerManager : MonoBehaviour
{
    public enum MarkerType
    {
        character0 = 0,
        character1 = 1,
        character2 = 2,
        character3 = 3,
        character4 = 4,
        character5 = 5,

        spiritguardian = 6,
        spiritharvester = 7,
        spiritforbidden = 8,
        spirithealer = 9,
        spiritwarrior = 10,
        spirittrickster = 11,
        spiritfamiliar = 12,

        itemherb = 13,
        itemgem = 14,
        itemtool = 15,

        placeOfPower1 = 16,
        placeOfPower2 = 17,
        placeOfPower3 = 18,
        placeOfPower4 = 19
    }

    [SerializeField] private WorldMapMarker m_MarkerPrefab;
    [SerializeField] private CovensMuskMap m_Map;
    [SerializeField] private MapCameraController m_Controller;

    [Header("Sprites")]
    [SerializeField] private Sprite m_WitchFemaleAfrican;
    [SerializeField] private Sprite m_WitchFemaleCaucasian;
    [SerializeField] private Sprite m_WitchFemaleAsian;
    [SerializeField] private Sprite m_WitchMaleAfrican;
    [SerializeField] private Sprite m_WitchMaleCaucasian;
    [SerializeField] private Sprite m_WitchMaleAsian;

    [Space(2)]
    [SerializeField] private Sprite m_SpiritGuardian;
    [SerializeField] private Sprite m_SpiritHarvester;
    [SerializeField] private Sprite m_SpiritForbidden;
    [SerializeField] private Sprite m_SpiritHealer;
    [SerializeField] private Sprite m_SpiritWarrior;
    [SerializeField] private Sprite m_SpiritTrickster;
    [SerializeField] private Sprite m_SpiritFamiliar;

    [Space(2)]
    [SerializeField] private Sprite m_CollectableHerb;
    [SerializeField] private Sprite m_CollectableGem;
    [SerializeField] private Sprite m_CollectableTool;

    [Space(2)]
    [SerializeField] private Sprite m_PlaceOfPower1;
    [SerializeField] private Sprite m_PlaceOfPower2;
    [SerializeField] private Sprite m_PlaceOfPower3;
    [SerializeField] private Sprite m_PlaceOfPower4;

    [Header("Colors")]
    [SerializeField] private Color m_WitchColor;
    [SerializeField] private Color m_SpiritColor;
    [SerializeField] private Color m_CollectableColor;
    [SerializeField] private Color m_PopColor;

    [Header("Settings")]
    [SerializeField] private float m_MarkerDetailedThreshold = 0.7f;
    [SerializeField] private float m_MarkerVisibleThreshoold = 0.5f;
    [Space(5)]
    [SerializeField] private float m_MinMarkerRange = 100000;
    [SerializeField] private float m_MaxMarkerRange = 5000;
    [Space(5)]
    [SerializeField] private float m_MinMarkerCount = 200;
    [SerializeField] private float m_MaxMarkerCount = 50;
    [Space(5)]
    [SerializeField] private float m_MinScale = 150;
    [SerializeField] private float m_MaxScale = 5;
    [Space(5)]
    [SerializeField] private int m_UpdateBatchSize = 200;
    
    private Sprite[] m_MarkerSpriteMap;
    private Color[] m_MarkerColorMap;

    private List<WorldMapMarker> m_MarkersList = new List<WorldMapMarker>();
    private List<WorldMapMarker> m_DespawnList = new List<WorldMapMarker>();
    private SimplePool<WorldMapMarker> m_MarkerPool;

    private float m_MarkerScale;
    private float m_LastScaleTime;
    private float m_LastRequestTime;
    private Vector2 m_LastMarkerPosition;

    private bool m_DetailedMarkers;
    private bool m_VisibleMarkers;

    private Coroutine m_SpawnCoroutine;
    private Coroutine m_RequestCoroutine;
    
    private int m_UpdateFrom;
    private int m_UpdateTo;

    private int updateFrom
    {
        get => m_UpdateFrom;
        set
        {
            m_UpdateFrom = value;
            m_UpdateTo = Mathf.Min(m_UpdateFrom + m_UpdateBatchSize, m_MarkersList.Count);
        }
    }

    private int m_BatchIndex;
    private float m_Range;

    private void Awake()
    {
        m_MarkerPool = new SimplePool<WorldMapMarker>(m_MarkerPrefab, 200);
        m_MarkerSpriteMap = new Sprite[]
        {
            m_WitchFemaleAfrican,
            m_WitchFemaleCaucasian,
            m_WitchFemaleAsian,
            m_WitchMaleAfrican,
            m_WitchMaleCaucasian,
            m_WitchMaleAsian,

            m_SpiritGuardian,
            m_SpiritHarvester,
            m_SpiritForbidden,
            m_SpiritHealer,
            m_SpiritWarrior,
            m_SpiritTrickster,
            m_SpiritFamiliar,

            m_CollectableHerb,
            m_CollectableGem,
            m_CollectableTool,

            m_PlaceOfPower1,
            m_PlaceOfPower1,
            m_PlaceOfPower1,
            m_PlaceOfPower1,
        };

        m_MarkerColorMap = new Color[]
        {
            m_WitchColor,
            m_WitchColor,
            m_WitchColor,
            m_WitchColor,
            m_WitchColor,
            m_WitchColor,

            m_SpiritColor,
            m_SpiritColor,
            m_SpiritColor,
            m_SpiritColor,
            m_SpiritColor,
            m_SpiritColor,
            m_SpiritColor,

            m_CollectableColor,
            m_CollectableColor,
            m_CollectableColor,

            m_PopColor,
            m_PopColor,
            m_PopColor,
            m_PopColor,
        };
    }

    private void Start()
    {
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;
    }
    
    private void OnStartFlying()
    {
        if (LoginAPIManager.characterLoggedIn == false)
            return;

        MapsAPI.Instance.OnChangeZoom += OnMapChangeZoom;
        OnMapChangeZoom();

        //get all surrounding markers on starting flight
        RequestMarkers((int)m_Controller.maxDistanceFromCenter);
    }

    private void OnStopFlying()
    {
        MapsAPI.Instance.OnChangeZoom -= OnMapChangeZoom;

        //stop spawning markers
        if (m_SpawnCoroutine != null)
        {
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }

        if (m_RequestCoroutine != null)
        {
            StopCoroutine(m_RequestCoroutine);
            m_RequestCoroutine = null;
        }

        //despawn all markers
        foreach (WorldMapMarker _item in m_MarkersList)
            m_MarkerPool.Despawn(_item);
        m_MarkersList.Clear();

        updateFrom = 0;
    }

    private void Update()
    {
        //ignore if items are not showing
        if (!m_VisibleMarkers)
            return;

        float timeSinceLastRequest = Time.time - m_LastRequestTime;
        if (timeSinceLastRequest < 2f)
            return;

        double distanceFromLastRequest = MapsAPI.Instance.DistanceBetweenPointsD(MapsAPI.Instance.position, m_LastMarkerPosition) * 1000;
        if (distanceFromLastRequest > m_Range)
        {
            float range;
            if (m_Map.normalizedZoom > 0.855f)
                range = m_Controller.maxDistanceFromCenter;
            else
                range = LeanTween.easeOutQuint(m_MinMarkerRange, m_MaxMarkerRange, m_Map.normalizedZoom);

            RequestMarkers((int)range);
        }
    }

    public void RequestMarkers(int distance)
    {
        m_Range = distance;
        distance = (int)(distance * 1.5f);

        m_LastMarkerPosition = MapsAPI.Instance.position;
        m_LastRequestTime = Time.time;

        double lng, lat;
        MapsAPI.Instance.GetPosition(out lng, out lat);

        if (m_RequestCoroutine != null)
        {
            StopCoroutine(m_RequestCoroutine);
            m_RequestCoroutine = null;
        }

        m_RequestCoroutine = StartCoroutine(APIManagerServer.RequestCoroutine(
            CovenConstants.wsMapServer + "?latitude=" + lat.ToString().Replace(',', '.') + "&longitude=" + lng.ToString().Replace(',', '.') + "&radius=" + distance,
            "",
            "GET",
            false,
            false,
            (response, result) =>
            {
                m_RequestCoroutine = null;
                if (result == 200)
                {
                    MarkerItem[] markers = JsonConvert.DeserializeObject<MarkerItem[]>(response);
                    
                    //stop spawning
                    if (m_SpawnCoroutine != null)
                    {
                        StopCoroutine(m_SpawnCoroutine);
                        m_SpawnCoroutine = null;
                    }

                    //despawn old
                    StartCoroutine(DespawnCoroutine(m_MarkersList.ToArray()));
                    m_MarkersList.Clear();
                    updateFrom = 0;

                    //spawn new markers
                    m_SpawnCoroutine = StartCoroutine(SpawnCoroutine(markers));
                }
                else
                {
                    Debug.LogError("failed to retrieve markers\n" + result + " " + response);
                }
            }));
    }
    
    private void OnMapChangeZoom()
    {
        m_MarkerScale = LeanTween.easeOutCubic(m_MinScale, m_MaxScale, MapsAPI.Instance.normalizedZoom);
        m_DetailedMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerDetailedThreshold;
        m_VisibleMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerVisibleThreshoold;

        if (Time.time - m_LastScaleTime < 0.05f)
            return;

        m_LastScaleTime = Time.time;
        updateFrom = 0;
    }

    private void ScaleMarker(WorldMapMarker marker)
    {
        marker.gameObject.SetActive(m_VisibleMarkers);
        marker.nearRenderer.enabled = m_DetailedMarkers;
        marker.farRenderer.enabled = !m_DetailedMarkers;
        marker.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
    }

    private IEnumerator SpawnCoroutine(MarkerItem[] markers)
    {
        int batchSize = 100;
        int from = 0;
        int to = Mathf.Min(from + batchSize, markers.Length);

        WorldMapMarker item;
        while (from < markers.Length)
        {
            for (int i = from; i < to; i++)
            {
                item = m_MarkerPool.Spawn(MapsAPI.Instance.trackedContainer);
                item.transform.position = MapsAPI.Instance.GetWorldPosition(markers[i].longitude, markers[i].latitude);

                item.farRenderer.color = m_MarkerColorMap[markers[i].type];
                item.nearRenderer.sprite = m_MarkerSpriteMap[markers[i].type];

                m_MarkersList.Add(item);
                ScaleMarker(item);
            }

            yield return 0;
            from += batchSize;
            to = Mathf.Min(from + batchSize, markers.Length);
        }

        m_SpawnCoroutine = null;
    }

    private IEnumerator DespawnCoroutine(WorldMapMarker[] markers)
    {
        int batchSize = 500;
        int from = 0;
        int to = Mathf.Min(from + batchSize, markers.Length);

        while (from < markers.Length)
        {
            for (int i = from; i < to; i++)
                m_MarkerPool.Despawn(markers[i]);

            from += batchSize;
            to = Mathf.Min(from + batchSize, markers.Length);
            yield return 0;
        }
    }

    private void LateUpdate()
    {
        //batch update of the markers
        for (int i = m_UpdateFrom; i < m_UpdateTo; i++)
        {
            ScaleMarker(m_MarkersList[i]);
        }
        if (m_UpdateFrom < m_MarkersList.Count)
        {
            m_UpdateFrom = m_UpdateFrom + m_UpdateBatchSize;
            m_UpdateTo = Mathf.Min(m_UpdateFrom + m_UpdateBatchSize, m_MarkersList.Count);
        }
    }

    [ContextMenu("Start Flying")]
    private void DebugStartFlying()
    {
        if (Application.isPlaying == false)
            return;

        OnStartFlying();
    }

    [ContextMenu("Stop Flying")]
    private void DebugStopFlying()
    {
        if (Application.isPlaying == false)
            return;

        OnStopFlying();
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        //float range = LeanTween.easeOutQuint(m_MinMarkerRange, m_MaxMarkerRange, m_Map.normalizedZoom);
        Gizmos.DrawWireSphere(m_Controller.CenterPoint.position, m_Range);
    }
}
