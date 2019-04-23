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
        public List<GameObject> gameObjects;
        public TextMeshPro label;
        public bool initialized;

        public StreetLabel(string name, SegmentMetadata.UsageType usage, GameObject gameObject, Line line)
        {
            this.name = name;
            this.usage = usage;
            this.vertices = new List<StreetPoint>();
            this.gameObjects = new List<GameObject>();
            AddLine(gameObject, line);
        }

        public void AddLine(GameObject gameObject, Line line)
        {
            this.gameObjects.Add(gameObject);
            for(int i = 0; i < line.Vertices.Length; i++)
            {
                vertices.Add(new StreetPoint(gameObject, line.Vertices[i]));
            }

            initialized = false;
        }
    }

    private SimplePool<TextMeshPro> m_LabelPool;
    private Dictionary<string, StreetLabel> m_StreetDictionary;
    private int m_BatchIndex;

    private void Awake()
    {
        m_Maps.Events.SegmentEvents.DidCreate.AddListener(OnCreateSegment);

        m_LabelPool = new SimplePool<TextMeshPro>(m_LabelPrefab, 50);
        m_StreetDictionary = new Dictionary<string, StreetLabel>();
    }

    private void Start()
    {
        StartCoroutine(UpdateLabelsCoroutine());
    }

    private void OnCreateSegment(DidCreateSegmentArgs e)
    {
        SegmentMetadata data = e.MapFeature.Metadata;

        if(string.IsNullOrEmpty(data.Name) == false)
        {
            //debug
            Transform parent;
            //debug

            StreetLabel label;
            if (m_StreetDictionary.ContainsKey(data.PlaceId))
            {
                label = m_StreetDictionary[data.PlaceId];
                label.AddLine(e.GameObject, e.MapFeature.Shape.Lines[0]);

                //debug
                parent = label.vertices[0].gameObject.transform.parent;
                //debug
            }
            else
            {
                m_StreetDictionary.Add(data.PlaceId, new StreetLabel(data.Name, data.Usage, e.GameObject, e.MapFeature.Shape.Lines[0]));

                //debug
                parent = new GameObject(data.Name + "(" + data.PlaceId + ")").transform;
                parent.SetParent(this.transform);
                //debug
            }

            //debug
            e.GameObject.transform.SetParent(parent);
            e.GameObject.transform.SetAsFirstSibling();
            for (int i = 0; i < e.MapFeature.Shape.Lines[0].Vertices.Length; i++)
            {
                Vector3 pos = e.GameObject.transform.position;
                pos.x += e.MapFeature.Shape.Lines[0].Vertices[i].x;
                pos.z += e.MapFeature.Shape.Lines[0].Vertices[i].y;
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.SetParent(parent);
                t.position = pos;
            }
            //debug
        }
    }

    private void SetPosition(StreetLabel street)
    {
        StreetPoint startVertex, endVertex;
        Vector3 startPos, endPos;

        if (street.vertices.Count > 5)
        {
            startVertex = street.vertices[(int)(street.vertices.Count * 0.3f)];
            endVertex = street.vertices[(int)((street.vertices.Count - 1) * 0.6f)];
        }
        else
        {
            startVertex = street.vertices[0];
            endVertex = street.vertices[street.vertices.Count - 1];
        }

        startPos = startVertex.gameObject.transform.position + new Vector3(startVertex.point.x, 1, startVertex.point.y);
        endPos = endVertex.gameObject.transform.position + new Vector3(endVertex.point.x, 1, endVertex.point.y);

        startPos = street.vertices[0].gameObject.transform.position;
        endPos = street.vertices[street.vertices.Count - 1].gameObject.transform.position;

        //Vector3 firstPoint, lastPoint;
        //firstPoint = street.vertices[0].gameObject.transform.position + new Vector3(street.vertices[0].point.x, 0, street.vertices[0].point.y);
        //lastPoint = street.vertices[street.vertices.Count - 1].gameObject.transform.position + new Vector3(street.vertices[street.vertices.Count - 1].point.x, 0, street.vertices[street.vertices.Count - 1].point.y);

        if (street.label == null)
        {
            street.label = m_LabelPool.Spawn();
            //street.label.transform.SetParent(m_MapsWrapper.itemContainer);
            street.label.transform.SetParent(startVertex.gameObject.transform.parent);
        }

        street.label.transform.position = (startPos + endPos) / 2 + new Vector3(0, 1, 0);
        street.label.text = street.name;
    }

    //iterate through the active labels to see if they should be despawned
    private IEnumerator UpdateLabelsCoroutine()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            foreach (StreetLabel street in m_StreetDictionary.Values)
            {
                SetPosition(street);
            }
        }
    }
}
