using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapMarkerManager : MonoBehaviour
{
    [SerializeField] private WorldMapMarker m_MarkerPrefab;
    [SerializeField] private CovensMuskMap m_Map;
    [SerializeField] private MapCameraController m_Controller;

    [Header("Sprites")]
    [SerializeField] private Sprite m_WitchSprite;
    [SerializeField] private Sprite m_SpiritSprite;
    [SerializeField] private Sprite m_Pop;
    [SerializeField] private Sprite[] m_CollectableSprite;

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
    [SerializeField] private float m_CollectableScaleModifier;
    [Space(5)]
    [SerializeField] private int m_BatchSize = 50;

    [Space(10)]
    [SerializeField] private bool m_Log;


    private struct MarkerItem
    {
        public string type;
        public string id;
        public float latitude;
        public float longitude;
        public string collectibleType;
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
    private List<MarkerItem> m_MarkersList = new List<MarkerItem>();
    private SimplePool<WorldMapMarker> m_MarkerPool;

    private float m_MarkerScale;
    private float m_LastRequestTime;
    private float m_LastRemoveTime;
    private Vector3 m_LastMarkerPosition;

    private bool m_IsFlying;
    private int m_RequestCount;
    private bool m_DetailedMarkers;
    private bool m_VisibleMarkers;

    private Coroutine m_SpawnCoroutine;

    private int m_BatchIndex;
    private float m_LastZoomValue;
    private float m_Range;
    private int m_LastItemCount;
    private bool m_CanRequest = true;
    private void Awake()
    {
        m_MarkerPool = new SimplePool<WorldMapMarker>(m_MarkerPrefab, 200);
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

        //MapsAPI.Instance.OnChangePosition += OnMapChangePosition;
        MapsAPI.Instance.OnChangeZoom += OnMapChangeZoom;

        //get all surrounding markerson starting flight
        RequestMarkers((int)m_Controller.maxDistanceFromCenter);

        OnMapChangeZoom();
        //OnMapChangePosition();

        m_RequestCount = 0;
        m_IsFlying = true;
    }

    private void OnStopFlying()
    {
        //MapsAPI.Instance.OnChangePosition -= OnMapChangePosition;
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
        m_MarkersList.Clear();
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

            yield return new WaitForSeconds(1);
            StartCoroutine(Connect());
        }
    }

    private void Update()
    {
        if (!m_Connected)
            return;

        string reply = m_Client.RecvString();

        if (m_Client.error != null)
        {
            Debug.LogError("map server error: " + m_Client.error);
            m_Connected = false;
            StartCoroutine(Connect());
        }
        else if (reply != null)
        {
            OnResponse?.Invoke(reply);

#if UNITY_EDITOR
            if (m_Log)
                Debug.Log("[WorldMarkerManager]\n" + reply);
#endif

            var data = JsonConvert.DeserializeObject<WSCommand>(reply);
            if (data.command == "markers")
            {
                m_CanRequest = data.labels.Length != m_LastItemCount;
                if (m_SpawnCoroutine != null)
                {
                    StopCoroutine(m_SpawnCoroutine);
                    m_SpawnCoroutine = null;
                }
                m_SpawnCoroutine = StartCoroutine(HandleMarkers(data.labels));
                m_LastItemCount = data.labels.Length;

            }
        }

        //ignore if items are not showing
        if (m_VisibleMarkers == false)
            return;

        float timeSinceLastRequest = Time.time - m_LastRequestTime;
        float distanceFromLastRequest = Vector2.Distance(m_Controller.CenterPoint.position, m_LastMarkerPosition);
        m_Range = LeanTween.easeOutQuint(m_MinMarkerRange, m_MaxMarkerRange, m_Map.normalizedZoom);
        int count = (int)LeanTween.easeOutCubic(m_MinMarkerCount, m_MaxMarkerCount, m_Map.normalizedZoom);

        m_MarkerCount = m_MarkersList.Count;

        //get all markers in the area
        if (m_Map.normalizedZoom > 0.855f)
        {
            if (distanceFromLastRequest > m_Controller.maxDistanceFromCenter * 0.6f)
                RequestMarkers((int)m_Controller.maxDistanceFromCenter);
        }
        else //get few random markers in the area
        {
            // if (timeSinceLastRequest > 1f)
            // {
            //     if (m_CanRequest)
            //         RequestMarkers((int)m_Range, count);
            // }
            if (distanceFromLastRequest > m_Range / 10f && timeSinceLastRequest > 0.2f)
            {
                m_CanRequest = true;
                RequestMarkers((int)m_Range, count);
            }
        }
    }

    [SerializeField] private int m_MarkerCount;

    public void RequestMarkers(int distance, int count = -1)
    {
        m_RequestCount++;

        m_LastMarkerPosition = MapsAPI.Instance.mapCenter.position;
        m_LastRequestTime = Time.time;

        double lng, lat;
        MapsAPI.Instance.GetPosition(out lng, out lat);

        var req = new
        {
            latitude = lat,
            longitude = lng,
            type = "markers",
            count,
            distance
        };

        string k = JsonConvert.SerializeObject(req);
        m_Client.Send(System.Text.Encoding.UTF8.GetBytes(k));

        OnRequest?.Invoke(k);


#if UNITY_EDITOR
        if (m_Log)
            Debug.Log("[WorldMarkerManager" + m_RequestCount + "] requesting markers\n" + k);
#endif
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

        int batch = 0;

        for (int i = 0; i < markers.Length; i++)
        {
            batch++;
            if (batch > 20)
            {
                batch = 0;
                yield return 0;
            }

            if (m_MarkerCount > 2000)
                yield break;

            if (m_IsFlying && !m_MarkersDictionary.ContainsKey(markers[i].id))
            {
                marker = m_MarkerPool.Spawn();
                marker.name = "[WorldMarker]" + markers[i].id + markers[i].type;
                markers[i].instance = marker;

                if (normalizedMapZoom < m_MarkerDetailedThreshold)
                    renderer = marker.farRenderer;
                else
                    renderer = marker.nearRenderer;

                if (markers[i].type[0] == 'c') //collectable
                {
                    if (markers[i].collectibleType == "gem")
                        marker.nearRenderer.sprite = m_CollectableSprite[0];
                    else if (markers[i].collectibleType == "tool")
                        marker.nearRenderer.sprite = m_CollectableSprite[1];
                    else
                        marker.nearRenderer.sprite = m_CollectableSprite[2];

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
                else if (markers[i].type[0] == 'l') //location
                {
                    marker.nearRenderer.sprite = m_Pop;
                    marker.farRenderer.color = m_PopColor;
                    marker.nearRenderer.transform.localScale = Vector3.one;
                }
                else
                {
                    marker.nearRenderer.sprite = null;
                    marker.farRenderer.color = new Color(0, 0, 0, 0);
                }

                marker.transform.position = MapsAPI.Instance.GetWorldPosition(markers[i].longitude, markers[i].latitude);
                marker.transform.SetParent(MapsAPI.Instance.trackedContainer);
                m_MarkersDictionary.Add(markers[i].id, markers[i]);
                m_MarkersList.Add(markers[i]);

                ScaleMarker(markers[i]);

                newMarkers.Add(renderer);
            }
        }
    }

    private void OnMapChangeZoom()
    {
        m_MarkerScale = LeanTween.easeOutCubic(m_MinScale, m_MaxScale, MapsAPI.Instance.normalizedZoom);
        m_DetailedMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerDetailedThreshold;
        m_VisibleMarkers = MapsAPI.Instance.normalizedZoom > m_MarkerVisibleThreshoold;

        //if entered low flight, request all markers
        if (m_Map.normalizedZoom >= 0.85 && m_LastZoomValue < 0.85)
            RequestMarkers((int)m_Controller.maxDistanceFromCenter, -1);

        m_LastZoomValue = m_Map.normalizedZoom;
    }

    private void LateUpdate()
    {
        //calculate range to iterate
        int from = m_BatchIndex;
        int to = Mathf.Min(m_BatchIndex + m_BatchSize, m_MarkersDictionary.Count - 1);

        m_BatchIndex = m_BatchIndex + m_BatchSize;
        if (m_BatchIndex >= m_MarkersList.Count)
            m_BatchIndex = 0;

        //range to expand the map bounds when checking if the point in inside the bounds
        float range = m_Range / 10;
        MarkerItem aux;


        for (int i = to; i >= from; i--)
        {
            aux = m_MarkersList[i];
            if (!m_Map.IsPointInsideView(aux.instance.transform.position, range))
            {
                m_MarkersDictionary.Remove(aux.id);
                m_MarkerPool.Despawn(aux.instance);
                m_MarkersList.RemoveAt(i);
            }
            else
            {
                ScaleMarker(aux);
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        //float range = LeanTween.easeOutQuint(m_MinMarkerRange, m_MaxMarkerRange, m_Map.normalizedZoom);
        Gizmos.DrawWireSphere(m_Controller.CenterPoint.position, m_Range);
    }
}
