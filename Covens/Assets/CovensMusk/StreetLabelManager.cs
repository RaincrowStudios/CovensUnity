using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Google.Maps.Feature;
using Google.Maps.Feature.Shape;

public class StreetLabelManager : MonoBehaviour
{
    [SerializeField] private MapsService m_Maps;
    [SerializeField] private CovensMuskMap m_MapsWrapper;
    [SerializeField] private TextMeshPro m_LabelPrefab;
    [SerializeField] private int m_BatchSize = 50;

    private class StreetPoint
    {
        public GameObject gameObject;
        public Vector2 point;

        public StreetPoint(GameObject gameObject, Vector2 point)
        {
            this.gameObject = gameObject;
            this.point = point;
        }
    }

    private class StreetLabel
    {
        public string name;
        public Google.Maps.Feature.SegmentMetadata.UsageType usage;
        public List<StreetPoint> vertices;
        public TextMeshPro label;
        public bool initialized;

        public StreetLabel(string name, SegmentMetadata.UsageType usage, GameObject gameObject, Line line)
        {
            this.name = name;
            this.usage = usage;
            this.vertices = new List<StreetPoint>();
            AddLine(gameObject, line);
        }

        public void AddLine(GameObject gameObject, Line line)
        {
            for(int i = 0; i < line.Vertices.Length; i++)
                vertices.Add(new StreetPoint(gameObject, line.Vertices[i]));

            initialized = false;
        }
    }

    private SimplePool<TextMeshPro> m_LabelPool;
    private Dictionary<string, StreetLabel> m_StreetDictionary;
    private int m_BatchIndex;
    private Coroutine m_UpdateCoroutine;

    private void Awake()
    {
        m_Maps.Events.SegmentEvents.DidCreate.AddListener(OnCreateSegment);
        m_Maps.Events.MapEvents.Loaded.AddListener(OnMapLoaded);
        m_MapsWrapper.OnChangeMuskZoom += OnMapStartReload;

        m_LabelPool = new SimplePool<TextMeshPro>(m_LabelPrefab, 50);
        m_StreetDictionary = new Dictionary<string, StreetLabel>();
    }

    private void OnCreateSegment(DidCreateSegmentArgs e)
    {
        //if the segment has a name, store its data to be shown on the screen.
        if(string.IsNullOrEmpty(e.MapFeature.Metadata.Name) == false)
        {
            SegmentMetadata data = e.MapFeature.Metadata;

            if (m_UpdateCoroutine != null)
            {
                StopCoroutine(m_UpdateCoroutine);
                m_UpdateCoroutine = null;
            }

            StreetLabel label;
            if (m_StreetDictionary.ContainsKey(data.PlaceId))
            {
                label = m_StreetDictionary[data.PlaceId];
                label.AddLine(e.GameObject, e.MapFeature.Shape.Lines[0]);
            }
            else
            {
                m_StreetDictionary.Add(data.PlaceId, new StreetLabel(data.Name, data.Usage, e.GameObject, e.MapFeature.Shape.Lines[0]));
            }
        }
    }

    private void OnMapStartReload()
    {
        //despawn all labels to avoid the labels teleporting when the map reloads

        foreach(StreetLabel street in m_StreetDictionary.Values)
            m_LabelPool.Despawn(street.label);
        m_StreetDictionary.Clear();
    }

    private void OnMapLoaded(MapLoadedArgs e)
    {
        m_BatchIndex = 0;
        m_UpdateCoroutine = StartCoroutine(UpdateLabelsCoroutine());
    }

    private void SetPosition(StreetLabel street)
    {
        StreetPoint startVertex, endVertex;
        Vector3 startPos, endPos, midPos;
        Vector3 labelPos;

        if (street.vertices.Count % 2 == 1)
        {
            StreetPoint midVertex;
            int leftIndex = Mathf.FloorToInt((street.vertices.Count - 1) * 0.4f);
            int righIndex = Mathf.CeilToInt((street.vertices.Count  - 1) * 0.6f);
            
            startVertex = street.vertices[leftIndex];
            endVertex = street.vertices[righIndex];
            midVertex = street.vertices[street.vertices.Count / 2];

            startPos = startVertex.gameObject.transform.position + new Vector3(startVertex.point.x, 0.1f, startVertex.point.y);
            endPos = endVertex.gameObject.transform.position + new Vector3(endVertex.point.x, 0.1f, endVertex.point.y);

            if (street.vertices.Count > 3)
                midPos = midVertex.gameObject.transform.position + new Vector3(midVertex.point.x, 0.1f, midVertex.point.y);
            else
                midPos = (endPos + startPos) / 2;
        }
        else
        {
            int midIndex = street.vertices.Count / 2;
            startVertex = street.vertices[Mathf.Max(midIndex - 1, 0)];
            endVertex = street.vertices[Mathf.Min(midIndex + 1, street.vertices.Count - 1)];

            startPos = startVertex.gameObject.transform.position + new Vector3(startVertex.point.x, 0.1f, startVertex.point.y);
            endPos = endVertex.gameObject.transform.position + new Vector3(endVertex.point.x, 0.1f, endVertex.point.y);
            midPos = (endPos + startPos) / 2;
        }

        labelPos = midPos;

        Vector3 forward = Vector3.up;
        Vector3 right = (endPos - startPos).normalized;
        Vector3 up = Vector3.Cross(forward, right);

        if (street.label == null)
        {
            street.label = m_LabelPool.Spawn();
            street.label.transform.SetParent(m_MapsWrapper.itemContainer);
        }

        street.label.text = street.name;
        street.label.transform.name = street.name;
        street.label.transform.position = labelPos;
        street.label.transform.rotation = Quaternion.LookRotation(-forward, up);
    }

    private IEnumerator UpdateLabelsCoroutine()
    {
        //iterate through the active labels to see if they should be despawned

        List<StreetLabel> streets = new List<StreetLabel>(m_StreetDictionary.Values);
        List<string> ids = new List<string>(m_StreetDictionary.Keys);

        while (m_BatchIndex < streets.Count)
        {
            int from = m_BatchIndex;
            int to = Mathf.Min(m_BatchIndex + m_BatchSize, streets.Count);
            m_BatchIndex = m_BatchIndex + m_BatchSize;

            for (int i = from; i < to; i++)
            {
                if (CleanRemovedSegments(streets[i], ids[i]))
                    SetPosition(streets[i]);
            }

            yield return 0;
        }
    }

    private bool CleanRemovedSegments(StreetLabel street, string id)
    {
        //remove deleted segments
        List<int> toRemove = new List<int>();

        for (int i = 0; i < street.vertices.Count; i++)
        {
            if (street.vertices[i].gameObject == null)
                toRemove.Add(i);
        }

        for (int i = toRemove.Count - 1; i >= 0; i--)
        {
            street.vertices.RemoveAt(toRemove[i]);
        }

        if (street.vertices.Count == 0)
        {
            m_StreetDictionary.Remove(id);
            m_LabelPool.Despawn(street.label);
            return false;
        }

        return true;
    }
}
