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
        public Transform transform;
        public Vector2 point;

        public StreetPoint(Transform transform, Vector2 point)
        {
            this.transform = transform;
            this.point = point;
        }
    }

    private class StreetLabel
    {
        public string name;
        public SegmentMetadata.UsageType usage;
        public List<StreetPoint> vertices;
        public TextMeshPro label;

        public int startVertex;
        public bool show = true;

        public StreetLabel(string name, SegmentMetadata.UsageType usage, Transform transform, Line line)
        {
            this.name = name;
            this.usage = usage;
            this.vertices = new List<StreetPoint>();
            this.startVertex = 0;
            AddLine(transform, line);
        }

        public void AddLine(Transform transform, Line line)
        {
            int idx = vertices.Count;// Mathf.Max(vertices.Count - 1, 0);

            for (int i = 0; i < line.Vertices.Length; i++)
            {
                vertices.Add(new StreetPoint(transform, line.Vertices[i]));

                //debug
                //Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                //t.SetParent(transform);
                //t.localPosition = new Vector3(line.Vertices[i].x, 0, line.Vertices[i].y);
                //t.name = vertices.Count + ": " + transform.name;
                //debug
            }
            
            UpdateMainVertice(idx);
        }

        public void UpdateMainVertice(int startIndex)
        {
            float curDist = Vector3.Distance(
                vertices[startVertex].transform.position + new Vector3(vertices[startVertex].point.x, 0, vertices[startVertex].point.y),
                vertices[startVertex + 1].transform.position + new Vector3(vertices[startVertex + 1].point.x, 0, vertices[startVertex + 1].point.y)
            );

            for (int i = startIndex; i < vertices.Count - 1; i++)
            {
                if (vertices[i].transform != vertices[i + 1].transform)
                    continue;

                float dist = Vector3.Distance(
                    vertices[i].transform.position + new Vector3(vertices[i].point.x, 0, vertices[i].point.y),
                    vertices[i + 1].transform.position + new Vector3(vertices[i + 1].point.x, 0, vertices[i + 1].point.y)
                );

                if (dist > curDist)
                {
                    curDist = dist;
                    startVertex = i;
                }
            }

            show = curDist > 50;
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
                if (CleanRemovedSegments(label, data.PlaceId))
                {
                    label.AddLine(e.GameObject.transform, e.MapFeature.Shape.Lines[0]);
                }
                else
                {
                    label = new StreetLabel(data.Name, data.Usage, e.GameObject.transform, e.MapFeature.Shape.Lines[0]);
                    m_StreetDictionary[data.PlaceId] = label;
                }
            }
            else
            {
                m_StreetDictionary.Add(data.PlaceId, new StreetLabel(data.Name, data.Usage, e.GameObject.transform, e.MapFeature.Shape.Lines[0]));
            }
        }
    }

    private void OnMapStartReload()
    {
        //despawn all labels to avoid the labels teleporting when the map reloads
        //foreach(StreetLabel street in m_StreetDictionary.Values)
        //    m_LabelPool.Despawn(street.label);
        //m_StreetDictionary.Clear();

        //foreach (StreetLabel street in m_StreetDictionary.Values)
        //    street.label.gameObject.SetActive(false);
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

        endVertex = street.vertices[street.startVertex];
        startVertex = street.vertices[street.startVertex + 1];

        startPos = startVertex.transform.position + new Vector3(startVertex.point.x, 0.1f, startVertex.point.y);
        endPos = endVertex.transform.position + new Vector3(endVertex.point.x, 0.1f, endVertex.point.y);
        midPos = (endPos + startPos) / 2;

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
        street.label.transform.position = midPos;
        street.label.transform.rotation = Quaternion.LookRotation(-forward, up);
        street.label.gameObject.SetActive(street.show);
    }

    private IEnumerator UpdateLabelsCoroutine()
    {
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
                {
                    SetPosition(streets[i]);
                }
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
            if (street.vertices[i].transform == null)
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
        else
        {
            street.startVertex = 0;
            street.UpdateMainVertice(0);
        }

        return true;
    }
}
