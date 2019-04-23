using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapMarkerManager : MonoBehaviour
{
    [SerializeField] private WorldMapMarker m_MarkerPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite m_WitchSprite;
    [SerializeField] private Sprite m_SpiritSprite;
    [SerializeField] private Sprite m_CollectableSprite;

    [Header("Colors")]
    [SerializeField] private Color m_WitchColor;
    [SerializeField] private Color m_SpiritColor;
    [SerializeField] private Color m_CollectableColor;

    [Header("Settings")]
    [SerializeField] private float m_MarkerDetailedThreshold = 0.7f;
    [SerializeField] private float m_MarkerVisibleThreshoold = 0.5f;

    [SerializeField] private float m_MinMarkerRange = 100000;
    [SerializeField] private float m_MaxMarkerRange = 1000;

    [SerializeField] private float m_MinScale = 150;
    [SerializeField] private float m_MaxScale = 5;
    [SerializeField] private float m_CollectableScaleModifier;


    private struct MarkerItem
    {
        public string type;
        public string name;
        public float latitude;
        public float longitude;
        public WorldMapMarker instance;
    }

    private struct WSCommand
    {
        public string command;
        public MarkerItem[] labels;
    }


    public static event System.Action<string> OnRequest;
    public static event System.Action<string> OnResponse;

    private WebSocket m_Client;
    private bool m_Connected;

    //MARKERS
    private Dictionary<string, MarkerItem> m_MarkersDictionary = new Dictionary<string, MarkerItem>();
    private SimplePool<WorldMapMarker> m_MarkerPool;

    private float m_MarkerScale;
    private float m_LastMarkerRequestTime;
    private Vector3 m_LastMarkerPosition;

    private bool m_IsFlying;
    private int m_RequestCount;
    private bool m_DetailedMarkers;
    private bool m_VisibleMarkers;

    private Coroutine m_SpawnCoroutine;

    private void Awake()
    {
        m_MarkerPool = new SimplePool<WorldMapMarker>(m_MarkerPrefab, 10);
    }

    private void Start()
    {
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;
    }

    private void OnEnable()
    {
        StartCoroutine(Connect());
    }

    private void OnStartFlying()
    {
        gameObject.SetActive(true);

        MapsAPI.Instance.OnChangePosition += OnMapChangePosition;
        MapsAPI.Instance.OnChangeZoom += OnMapChangeZoom;

        RequestMarkers((int)LeanTween.easeOutCubic(m_MinMarkerRange, m_MaxMarkerRange, MapsAPI.Instance.normalizedZoom));

        OnMapChangeZoom();
        OnMapChangePosition();

        m_RequestCount = 0;
        m_IsFlying = true;
    }

    private void OnStopFlying()
    {
        Debug.Log("worldmap stop flying");
        MapsAPI.Instance.OnChangePosition -= OnMapChangePosition;
        MapsAPI.Instance.OnChangeZoom -= OnMapChangeZoom;

        m_IsFlying = false;

        if (m_SpawnCoroutine != null)
        {
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }

        //despawn all markers
        foreach (MarkerItem _item in m_MarkersDictionary.Values)
            m_MarkerPool.Despawn(_item.instance);

        m_MarkersDictionary.Clear();
    }

    private IEnumerator Connect()
    {
        if (m_Connected)
            yield break;

        m_Client = new WebSocket(new System.Uri(CovenConstants.wsMapServer));
        yield return m_Client.Connect();
        
        if (string.IsNullOrEmpty(m_Client.error))
        {
            Debug.Log("connected to mapserver");
            m_Connected = true;
        }
        else
        {
            Debug.LogError("error connecting to map server: " + m_Client.error);
            m_Connected = false;
            StartCoroutine(Connect());
        }
    }

    private void Update()
    {
        if (!m_Connected)
            return;

        string reply = m_Client.RecvString();

        if(m_Client.error != null)
        {
            Debug.LogError("map server error: " + m_Client.error);
            m_Connected = false;
            StartCoroutine(Connect());
        }
        else if (reply != null)
        {
            OnResponse?.Invoke(reply);

            var data = JsonConvert.DeserializeObject<WSCommand>(reply);

            if (data.command == "markers")
            {
                if (m_SpawnCoroutine != null)
                {
                    StopCoroutine(m_SpawnCoroutine);
                    m_SpawnCoroutine = null;
                }
                m_SpawnCoroutine = StartCoroutine(HandleMarkers(data.labels));
            }
        }

        //ignore if items are not showing
        if (m_VisibleMarkers == false)
            return;

        //ignore if in cooldown
        if (Time.time - m_LastMarkerRequestTime < 0.5f)
            return;

        float distanceFromLastRequest = Vector2.Distance(MapsAPI.Instance.mapCenter.position, m_LastMarkerPosition);
        float range = LeanTween.easeOutCubic(m_MinMarkerRange, m_MaxMarkerRange, MapsAPI.Instance.normalizedZoom);

        //ignore if the camera didnt move enough
        if (distanceFromLastRequest < range / 10)
            return;

        //remove items out of screen
        if (m_MarkersDictionary.Count > 100)
        {
            List<MarkerItem> toRemove = new List<MarkerItem>();
            foreach(MarkerItem _item in m_MarkersDictionary.Values)
            {
                if (MapsAPI.Instance.worldspaceBounds.Contains(_item.instance.transform.position) == false)
                    toRemove.Add(_item);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                m_MarkersDictionary.Remove(toRemove[i].name);
                m_MarkerPool.Despawn(toRemove[i].instance);
            }
        }

        //finally request mo' markers
        RequestMarkers((int)range);
    }
    
    public void RequestMarkers(int distance)
    {
        m_RequestCount++;

        m_LastMarkerPosition = MapsAPI.Instance.mapCenter.position;
        m_LastMarkerRequestTime = Time.time;

        double lng, lat;
        MapsAPI.Instance.GetPosition(out lng, out lat);

        var req = new
        {
            latitude = lat,
            longitude = lng,
            type = "markers",
            distance
        };

//#if UNITY_EDITOR
//        Debug.Log("[WorldMarkerManager"+m_RequestCount+"] requesting markers for " + lat + ", " + lng);
//#endif

        string k = JsonConvert.SerializeObject(req);
        m_Client.Send(System.Text.Encoding.UTF8.GetBytes(k));

        OnRequest?.Invoke(k);
    }

    private IEnumerator HandleMarkers(MarkerItem[] markers)
    {
        if (markers == null)
            yield break;

        bool streetLevel = MapsAPI.Instance.streetLevel;
        float normalizedMapZoom = MapsAPI.Instance.normalizedZoom;
        List<SpriteRenderer> newMarkers = new List<SpriteRenderer>();
        WorldMapMarker marker;
        SpriteRenderer renderer;

        for (int i = 0; i < markers.Length; i++)
        {
            if (m_IsFlying && !m_MarkersDictionary.ContainsKey(markers[i].name))
            {
                marker = m_MarkerPool.Spawn();
                marker.name = "[WorldMarker]" + markers[i].name;
                markers[i].instance = marker;

                if (normalizedMapZoom < m_MarkerDetailedThreshold)
                    renderer = marker.farRenderer;
                else
                    renderer = marker.nearRenderer;

                if (markers[i].type[0] == 'c') //collectable
                {
                    marker.nearRenderer.sprite = m_CollectableSprite;
                    marker.farRenderer.color = m_CollectableColor;
                    marker.nearRenderer.transform.localScale = new Vector3(m_CollectableScaleModifier, m_CollectableScaleModifier, m_CollectableScaleModifier);
                }
                else if (markers[i].type[0] == 'w') //witch
                {
                    marker.nearRenderer.sprite = m_WitchSprite;
                    marker.farRenderer.color = m_WitchColor;
                    marker.nearRenderer.transform.localScale = Vector3.one;
                }
                else if (markers[i].type[0] == 's') //spirit
                {
                    marker.nearRenderer.sprite = m_SpiritSprite;
                    marker.farRenderer.color = m_SpiritColor;
                    marker.nearRenderer.transform.localScale = Vector3.one;
                }
                else
                {
                    marker.nearRenderer.sprite = null;
                    marker.farRenderer.color = Color.white;
                }

                marker.transform.position = MapsAPI.Instance.GetWorldPosition(markers[i].longitude, markers[i].latitude);
                marker.transform.SetParent(MapsAPI.Instance.trackedContainer);
                m_MarkersDictionary.Add(markers[i].name, markers[i]);

                ScaleMarker(markers[i]);

                newMarkers.Add(renderer);

                yield return 0;
            }
        }
    }

    private void OnMapChangeZoom()
    {
        m_MarkerScale = LeanTween.easeOutCubic(m_MinScale, m_MaxScale, MapsAPI.Instance.normalizedZoom);
        m_DetailedMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerDetailedThreshold;
        m_VisibleMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerVisibleThreshoold;

        foreach (MarkerItem _item in m_MarkersDictionary.Values)
        {
            ScaleMarker(_item);
        }
    }

    private void OnMapChangePosition()
    {
        //foreach (MarkerItem _item in m_MarkersDictionary.Values)
        //{
        //}
    }

    private void ScaleMarker(MarkerItem marker)
    {
        marker.instance.gameObject.SetActive(m_VisibleMarkers);
        marker.instance.nearRenderer.enabled = m_DetailedMarkers;
        marker.instance.farRenderer.enabled = !m_DetailedMarkers;
        marker.instance.transform.localScale = new Vector3(m_MarkerScale, m_MarkerScale, m_MarkerScale);
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

    [ContextMenu("Request markers")]
    private void DebugRequestMarkers()
    {
        if (Application.isPlaying == false)
            return;

        RequestMarkers(1000);
    }
}
