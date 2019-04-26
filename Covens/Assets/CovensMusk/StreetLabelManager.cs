//#define DEBUG_SEGMENTS

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
    [SerializeField] private MapCameraController m_MapController;
    [SerializeField] private CovensMuskMap m_MapsWrapper;
    [SerializeField] private TextMeshPro m_LabelPrefab;
    [SerializeField] private int m_BatchSize = 50;

    private class SegmentGroupDebugger : MonoBehaviour
    {
        public Vector3 midPoint;
        public Vector3 midVertex;
        public List<Vector3> vertices = new List<Vector3>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < vertices.Count; i++)
            {
                Gizmos.DrawWireSphere(vertices[i], 1);
                drawString(i.ToString(), vertices[i], Color.white);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(midVertex, 1);
            drawString("mid vertex", midVertex, Color.red);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(midPoint, Vector3.one * 1);
            drawString("mid point", midPoint, Color.green);
        }

        private static void drawString(string text, Vector3 worldPos, Color? colour = null)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
            UnityEditor.Handles.EndGUI();
#endif
        }
    }

    private class StreetPoint
    {
        public Transform transform;
        public Vector2 point;
        public Vector2 direction;

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
        public List<Transform> transforms;
        public TextMeshPro label;

        public Vector3 midPoint;
        public int midVertex;
        public int longVertex;
        public bool show = true;

        public SegmentGroupDebugger debugger;

        public StreetLabel(string name, SegmentMetadata.UsageType usage, Transform transform, Line line, SegmentGroupDebugger debugger)
        {
            this.name = name;
            this.usage = usage;
            this.vertices = new List<StreetPoint>();
            this.transforms = new List<Transform>();
            this.debugger = debugger;
            AddLine(transform, line);
        }

        public void AddLine(Transform transform, Line line)
        {
            int idx = vertices.Count;// Mathf.Max(vertices.Count - 1, 0);
            transforms.Add(transform);

            for (int i = 0; i < line.Vertices.Length; i++)
            {
                vertices.Add(new StreetPoint(transform, line.Vertices[i]));

#if DEBUG_SEGMENTS
                debugger.vertices.Add(transform.position + new Vector3(line.Vertices[i].x, 0, line.Vertices[i].y));
#endif
            }

            UpdateMainVertices(idx);
        }

        public void UpdateMainVertices(int startIndex)
        {
            midPoint = Vector3.zero;
            for (int i = 0; i < transforms.Count; i++)
                midPoint += transforms[i].position;
            midPoint /= transforms.Count;

#if DEBUG_SEGMENTS
            debugger.midPoint = midPoint;
#endif

            float curDist = Vector3.Distance(midPoint, vertices[midVertex].transform.position + new Vector3(vertices[midVertex].point.x, 0, vertices[midVertex].point.y));
            float maxDist = Vector2.Distance(vertices[longVertex].point, vertices[longVertex + 1].point);

            for (int i = startIndex; i < vertices.Count - 1; i++)
            {
                if (vertices[i].transform != vertices[i + 1].transform)
                    continue;

                vertices[i].direction = (vertices[i + 1].point - vertices[i].point);

                float dist = Vector3.Distance(midPoint, vertices[i].transform.position + new Vector3(vertices[i].point.x, 0, vertices[i].point.y));
                if (dist < curDist)
                {
                    curDist = dist;
                    midVertex = i;
                }

                dist = Vector2.Distance(vertices[i].point, vertices[i + 1].point);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    longVertex = i;
                }

            }

            show = maxDist > 30;

#if DEBUG_SEGMENTS
            debugger.midVertex = vertices[midVertex].transform.position + new Vector3(vertices[midVertex].point.x, 0, vertices[midVertex].point.y);
#endif
        }
    }


    private SimplePool<TextMeshPro> m_LabelPool;
    private Dictionary<string, StreetLabel> m_StreetDictionary;
    private int m_BatchIndex;
    private Coroutine m_UpdateCoroutine;

    private float[] m_ScaleModifier = new float[]
    {
        1,      //Unspecified
        1,      //Road
        0.9f,      //LocalRoad
        1.3f,   //ArterialRoad
        1.6f,  //Highway
        1.6f,  //ControlledAccessHighway
        1,      //Foothpath
        1,      //Rail
        1,      //Ferry
    };

    private bool[][] m_UsageByZoom = new bool[][]
    {
        //              unsp    road    localroad   arterialroad    highway     ctrlacchigh     foothpath   rail    ferry
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //0
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //1
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //2
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //3
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //4
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //5
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //6
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //7
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //8
        new bool[] {    false,  false,  false,      false,          false,      false,          false,      false,  false   },  //9
        new bool[] {    false,  false,  false,      false,          true,       true,           false,      false,  false   },  //10
        new bool[] {    false,  false,  false,      false,          true,       true,           false,      false,  false   },  //11
        new bool[] {    false,  false,  false,      false,          true,       true,           false,      false,  false   },  //12
        new bool[] {    false,  false,  false,      false,          true,       true,           false,      false,  false   },  //13
        new bool[] {    false,  false,  false,      false,          true,       true,           false,      false,  false   },  //14
        new bool[] {    false,  true,   true,       false,         true,       true,           false,      false,  false   },  //15
        new bool[] {    false,  true,   true,       false,          true,       true,           false,      false,  false   },  //16
        new bool[] {    true,   true,   true,       true,           true,       true,           true,       true,   true   },  //17
        new bool[] {    true,   true,   true,       true,           true,       true,           true,       true,   true   },  //18
    };


    private void Awake()
    {
        m_Maps.Events.SegmentEvents.DidCreate.AddListener(OnCreateSegment);
        m_Maps.Events.MapEvents.Loaded.AddListener(OnMapLoaded);
        m_MapsWrapper.onWillChangeZoomLevel += OnWillChangeZoomLevel;
        
        m_LabelPool = new SimplePool<TextMeshPro>(m_LabelPrefab, 50);
        m_StreetDictionary = new Dictionary<string, StreetLabel>();
    }

    private void Start()
    {
        m_LabelScale = m_MaxScale;
    }

    private void OnCreateSegment(DidCreateSegmentArgs e)
    {
        //if the segment has a name, store its data to be shown on the screen.
        if (string.IsNullOrEmpty(e.MapFeature.Metadata.Name) == false && e.MapFeature.Metadata.Name.Length > 3)
        {
            SegmentMetadata data = e.MapFeature.Metadata;
            StreetLabel label;
            SegmentGroupDebugger debugger = null;

            if (m_UpdateCoroutine != null)
            {
                StopCoroutine(m_UpdateCoroutine);
                m_UpdateCoroutine = null;
            }

#if DEBUG_SEGMENTS
            Transform groupTransform;
            if (m_StreetDictionary.ContainsKey(data.PlaceId))
                CleanRemovedSegments(m_StreetDictionary[data.PlaceId], data.PlaceId);
            if (m_StreetDictionary.ContainsKey(data.PlaceId))
            {
                groupTransform = m_StreetDictionary[data.PlaceId].vertices[0].transform.parent;
            }
            else
            {
                groupTransform = new GameObject("[" + data.Usage.ToString() + "] " + data.Name + " => " + data.PlaceId).transform;
                groupTransform.position = e.GameObject.transform.position;
                groupTransform.SetParent(this.transform);
                debugger = groupTransform.gameObject.AddComponent<SegmentGroupDebugger>();
            }
            e.GameObject.transform.SetParent(groupTransform);
#endif

            if (m_StreetDictionary.ContainsKey(data.PlaceId))
            {
                label = m_StreetDictionary[data.PlaceId];
                if (CleanRemovedSegments(label, data.PlaceId))
                {
                    label.AddLine(e.GameObject.transform, e.MapFeature.Shape.Lines[0]);
                }
                else
                {
                    label = new StreetLabel(data.Name, data.Usage, e.GameObject.transform, e.MapFeature.Shape.Lines[0], debugger);
                    m_StreetDictionary[data.PlaceId] = label;
                }
            }
            else
            {
                label = new StreetLabel(data.Name, data.Usage, e.GameObject.transform, e.MapFeature.Shape.Lines[0], debugger);
                m_StreetDictionary.Add(data.PlaceId, label);
            }
        }
    }

    private void OnMapLoaded(MapLoadedArgs e)
    {
        m_BatchIndex = 0;
        m_UpdateCoroutine = StartCoroutine(UpdateLabelsCoroutine());
    }

    private void SetPosition(StreetLabel street)
    {
        StreetPoint startVertex, endVertex;
        Vector3 right;

        if (street.vertices.Count > 3)
        {
            startVertex = street.vertices[street.midVertex];
            endVertex = street.vertices[street.midVertex + 1];
            right = new Vector3(street.vertices[street.midVertex].direction.x, 0, street.vertices[street.midVertex].direction.y);
        }
        else
        {
            startVertex = street.vertices[street.longVertex];
            endVertex = street.vertices[street.longVertex + 1];
            right = new Vector3(street.vertices[street.longVertex].direction.x, 0, street.vertices[street.longVertex].direction.y);
        }
        
        Vector3 forward = Vector3.up;
        Vector3 up = Vector3.Cross(forward, right);

        Vector3 startPos = startVertex.transform.position + new Vector3(startVertex.point.x, 0.1f, startVertex.point.y);
        Vector3 endPos = endVertex.transform.position + new Vector3(endVertex.point.x, 0.1f, endVertex.point.y);
        Vector3 pos = (endPos + startPos) / 2;

        if (street.label == null)
        {
            street.label = m_LabelPool.Spawn();
            street.label.alpha = 0;
            SetScale(street);
            street.label.transform.SetParent(m_MapsWrapper.itemContainer);
#if DEBUG_SEGMENTS
            street.label.transform.SetParent(street.transforms[0].parent);
#endif
        }

        street.label.text = street.name;
        street.label.transform.position = pos;
        street.label.transform.rotation = Quaternion.LookRotation(-forward, up);
        street.label.gameObject.SetActive(street.show);

#if UNITY_EDITOR
        street.label.transform.name = "[" + street.usage + "] " + street.name;
#endif
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

        for (int i = street.vertices.Count - 1; i >= 0; i--)
        {
            if (street.vertices[i].transform == null)
                street.vertices.RemoveAt(i);
        }

        for (int i = street.transforms.Count - 1; i >= 0; i--)
        {
            if (street.transforms[i] == null)
                street.transforms.RemoveAt(i);
        }

        //all the segments are gone
        if (street.vertices.Count == 0)
        {
            m_StreetDictionary.Remove(id);
            m_LabelPool.Despawn(street.label);
            return false;
        }
        //resets the indexes to recalculate them
        else
        {
            street.midVertex = 0;
            street.longVertex = 0;
            street.UpdateMainVertices(0);
        }

        return true;
    }

    private void OnWillChangeZoomLevel()
    {
        if (m_UpdateCoroutine != null)
        {
            StopCoroutine(m_UpdateCoroutine);
            m_UpdateCoroutine = null;
        }

        foreach (var street in m_StreetDictionary)
        {
            m_LabelPool.Despawn(street.Value.label);
            street.Value.label = null;
        }

        m_StreetDictionary.Clear();
    }

    private float m_NormalizedZoom;
    private float m_LabelScale;
    public float m_MinScale = 1;
    public float m_MaxScale = 1000;

    private void LateUpdate()
    {
        m_NormalizedZoom = Mathf.Min(1 - MapsAPI.Instance.normalizedZoom, 0.25f) / 0.25f;
        m_LabelScale = LeanTween.easeInQuint(m_MinScale, m_MaxScale, m_NormalizedZoom);
        
        foreach (var street in m_StreetDictionary)
        {
            SetScale(street.Value);
        };
    }

    private void SetScale(StreetLabel street)
    {
        if (street.label == null)
            return;

        if (m_UsageByZoom[m_Maps.ZoomLevel][(int)street.usage])
        {
            street.label.transform.localScale = Vector3.one * m_MapsWrapper.cameraDat.segmentWidth * m_ScaleModifier[(int)street.usage] * m_LabelScale;
            street.label.alpha += Time.deltaTime * 2f;
        }
        else
        {
            street.label.gameObject.SetActive(false);
        }
    }
}
