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
        public MarkerItem[] markers;
    }

    private WebSocket m_Client;
    private bool m_Connected;

    //MARKERS
    private Dictionary<string, MarkerItem> m_MarkersDictionary = new Dictionary<string, MarkerItem>();
    private SimplePool<WorldMapMarker> m_MarkerPool;

    private void Awake()
    {
        m_MarkerPool = new SimplePool<WorldMapMarker>(m_MarkerPrefab, 10);
    }

    private void OnEnable()
    {
        StartCoroutine(SocketListen());

        MapsAPI.Instance.OnChangePosition += OnMapChangePosition;
        MapsAPI.Instance.OnChangeZoom += OnMapChangeZoom;
    }

    private void OnDisable()
    {
        MapsAPI.Instance.OnChangePosition -= OnMapChangePosition;
        MapsAPI.Instance.OnChangeZoom -= OnMapChangeZoom;
    }

    private IEnumerator SocketListen()
    {
        if (m_Connected == false)
        {
            yield return 0;
            m_Client = new WebSocket(new System.Uri(CovenConstants.wsMapServer));
            yield return m_Client.Connect();
        }

        if (string.IsNullOrEmpty(m_Client.error))
        {
            Debug.Log("connected to mapserver");
            m_Connected = true;

            while (true)
            {
                string reply = m_Client.RecvString();

                if(m_Client.error != null)
                {
                    Debug.LogError("map server error: " + m_Client.error);
                    m_Connected = false;
                    StartCoroutine(SocketListen());
                    yield break;
                }

                if (reply != null)
                {
                    var data = JsonConvert.DeserializeObject<WSCommand>(reply);
                    if (data.command == "markers")
                        HandleMarkers(data.markers);
                }

                yield return 0;
            }
        }
        else
        {
            Debug.LogError("error connecting to map server: " + m_Client.error);
            m_Connected = false;
            StartCoroutine(SocketListen());
        }
    }

    public void RequestLabel(Vector2 pos, int distance)
    {
        var req = new
        {
            latitude = pos.y,
            longitude = pos.x,
            type = "markers",
            distance
        };
        string k = JsonConvert.SerializeObject(req);
        m_Client.Send(System.Text.Encoding.UTF8.GetBytes(k));
    }

    private void HandleMarkers(MarkerItem[] markers)
    {
        bool streetLevel = MapsAPI.Instance.streetLevel;
        float normalizedMapZoom = MapsAPI.Instance.normalizedZoom;
        List<SpriteRenderer> newMarkers = new List<SpriteRenderer>();
        WorldMapMarker marker;
        SpriteRenderer renderer;

        for (int i = 0; i < markers.Length; i++)
        {
            if (!m_MarkersDictionary.ContainsKey(markers[i].name))
            {
                marker = m_MarkerPool.Spawn();
                markers[i].instance = marker;
                
                //setup the marker
                marker.gameObject.SetActive(false);
                if (normalizedMapZoom < m_MarkerDetailedThreshold)
                    renderer = marker.farRenderer;
                else
                    renderer = marker.nearRenderer;

                if (markers[i].type[0] == 'c') //collectable
                {
                    marker.nearRenderer.sprite = m_CollectableSprite;
                    marker.farRenderer.color = m_CollectableColor;
                }
                else if (markers[i].type[0] == 'w') //witch
                {
                    marker.nearRenderer.sprite = m_WitchSprite;
                    marker.farRenderer.color = m_WitchColor;
                }
                else if (markers[i].type[0] == 's') //spirit
                {
                    marker.nearRenderer.sprite = m_SpiritSprite;
                    marker.farRenderer.color = m_SpiritColor;
                }
                else
                {
                    marker.nearRenderer.sprite = null;
                    marker.farRenderer.color = Color.white;
                }

                if (streetLevel || normalizedMapZoom < m_MarkerVisibleThreshoold) //hide it but keep it cached to be shown when changing zoom
                    marker.gameObject.SetActive(false);
                else
                    newMarkers.Add(renderer);   //add it to be shown
            }
        }

        if (newMarkers.Count > 0)
        {
            for (int i = 0; i < newMarkers.Count; i++)
                newMarkers[i].color = new Color(newMarkers[i].color.r, newMarkers[i].color.g, newMarkers[i].color.b, 0);

            //animate the newly added markers
            LeanTween.value(0, 1, 0.2f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    for (int i = 0; i < newMarkers.Count; i++)
                        newMarkers[i].color = new Color(newMarkers[i].color.r, newMarkers[i].color.g, newMarkers[i].color.b, t);
                });
        }
    }

    private void OnMapChangeZoom()
    {
        float normalizedMapZoom = MapsAPI.Instance.normalizedZoom;
        foreach (MarkerItem _item in m_MarkersDictionary.Values)
        {

        }
    }

    private void OnMapChangePosition()
    {
        float normalizedMapZoom = MapsAPI.Instance.normalizedZoom;
        foreach (MarkerItem _item in m_MarkersDictionary.Values)
        {

        }
    }
}
