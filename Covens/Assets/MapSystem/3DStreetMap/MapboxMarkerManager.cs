using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapboxMarkerManager : MonoBehaviour
{
    public struct MarkerData
    {
        public string id;
        public Vector2d latlon;
    }

    public struct MarkerInstance
    {
        public Transform transform;
        public MarkerData data;
    }

    [SerializeField] private GameObject m_SpiritMarkerPrefab;
    [SerializeField] private AbstractMap m_Map;
    
    private Transform m_MarkerContainer;
    private Dictionary<string, int> m_MarkersDict = new Dictionary<string, int>();
    private List<MarkerInstance> m_MarkersList = new List<MarkerInstance>();

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (m_Map == null)
            m_Map = FindObjectOfType<AbstractMap>();

        this.transform.position = m_Map.transform.position;
        m_MarkerContainer = new GameObject("marker container").transform;
        m_MarkerContainer.transform.SetParent(this.transform);
    }

    private void Start()
    {
        Debug_SpawnMarker();
    }

    public void SpawnMarker(MarkerData data)
    {
        MarkerInstance newMarker = new MarkerInstance();
        newMarker.data = data;
        newMarker.transform = new GameObject("marker:" + data.id).transform;
        newMarker.transform.SetParent(m_MarkerContainer);
        GameObject.Instantiate(m_SpiritMarkerPrefab, newMarker.transform);

        Vector3 worldPos = m_Map.GeoToWorldPosition(data.latlon, false);
        newMarker.transform.position = worldPos;

        m_MarkersDict.Add(newMarker.data.id, m_MarkersList.Count);
        m_MarkersList.Add(newMarker);
    }

    [Header("TESTS")]
    [SerializeField] private Vector2d m_LatLon;

    [ContextMenu("Test spawn")]
    private void Debug_SpawnMarker()
    {
        MarkerData marker = new MarkerData
        {
            id = "test",
            latlon = m_LatLon
        };
        SpawnMarker(marker);
    }
}
