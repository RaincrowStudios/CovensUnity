using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class DynamicLabelManager : MonoBehaviour
{
    public static DynamicLabelManager instance { get; set; }
    [SerializeField] float visibleZoom = 2.4f;
    [SerializeField] float minScale = .2f;
    [SerializeField] float maxScale = .5f;
    [SerializeField] float iconMultiplier = 2f;

    [Header("marker prefabs")]
    [SerializeField] private GameObject m_WitchPrefab;
    [SerializeField] private GameObject m_LocationPrefab;
    [SerializeField] private GameObject m_SpiritPrefab;
    [SerializeField] private GameObject m_ToolPrefab;
    [SerializeField] private GameObject m_GemPrefab;
    [SerializeField] private GameObject m_HerbPrefab;
    [SerializeField] private Transform m_collider;
    [SerializeField] private float radius = 10;


    private SpriteMapsController sm;
    private Camera cam;
    Dictionary<string, Label> markers = new Dictionary<string, Label>();

    private SimplePool<Transform> m_WitchPool;
    private SimplePool<Transform> m_LocationPool;
    private SimplePool<Transform> m_SpiritPool;
    private SimplePool<Transform> m_ToolPool;
    private SimplePool<Transform> m_GemPool;
    private SimplePool<Transform> m_HerbPool;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        sm = SpriteMapsController.instance;
        sm.onChangePosition += SetLabels;
        sm.onChangeZoom += SetLabels;
        cam = SpriteMapsController.instance.m_Camera;

        m_WitchPool = new SimplePool<Transform>(m_WitchPrefab.transform, 5);
        m_LocationPool = new SimplePool<Transform>(m_LocationPrefab.transform, 5);
        m_SpiritPool = new SimplePool<Transform>(m_SpiritPrefab.transform, 5);

        m_ToolPool = new SimplePool<Transform>(m_ToolPrefab.transform, 5);
        m_GemPool = new SimplePool<Transform>(m_GemPrefab.transform, 5);
        m_HerbPool = new SimplePool<Transform>(m_HerbPrefab.transform, 5);
    }

    public void GenerateLabels(WSResponse data)
    {

        foreach (var item in data.labels)
        {
            if (!markers.ContainsKey(item.name))
                markers.Add(item.name, new Label
                {
                    id = item.id,
                    name = item.name,
                    type = item.type,
                    pos = sm.GetWorldPosition(item.longitude, item.latitude)
                });
        }

    }

    public void ScanForItems()
    {
        MarkerManagerAPI.instancesInRange.Clear();
        Collider[] items = Physics.OverlapSphere(m_collider.position, radius);
        foreach (var item in items)
        {
            if (item.tag == "worldMapItem")
            {
                MarkerManagerAPI.instancesInRange.Add(item.name);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ScanForItems();
        }
    }

    void SetLabels()
    {
        float scale = 0;
        if (cam.orthographicSize <= visibleZoom)
            scale = sm.normalizedZoom * maxScale + (1 - sm.normalizedZoom) * minScale * iconMultiplier;

        foreach (var t in markers)
        {
            if (cam.orthographicSize > visibleZoom)
            {
                if (t.Value.created)
                {
                    Despawn(t.Value.type, t.Value.k);
                    t.Value.created = false;
                }
                continue;
            }
            if (MapUtils.inMapView(t.Value.pos, cam))
            {
                if (!t.Value.created)
                {
                    Transform token;
                    if (t.Value.type == "witch")
                        token = m_WitchPool.Spawn();
                    else if (t.Value.type == "location")
                        token = m_LocationPool.Spawn();
                    else if (t.Value.type == "spirit")
                        token = m_SpiritPool.Spawn();
                    else
                    {
                        int rand = Random.Range(0, 3);

                        if (rand == 0)
                            token = m_ToolPool.Spawn();
                        else if (rand == 1)
                            token = m_GemPool.Spawn();
                        else
                            token = m_HerbPool.Spawn();
                    }
                    token.tag = "worldMapItem";
                    token.name = t.Value.id;
                    token.SetParent(transform);
                    token.position = t.Value.pos;
                    t.Value.k = token;
                    t.Value.created = true;
                    token.GetComponent<DynamicLabelItem>().Setup(sm);
                }

                //if (cam.orthographicSize < .03f)
                //    t.Value.k.transform.localScale = Vector3.one * scale * iconMultiplier;
                //else
                t.Value.k.transform.localScale = Vector3.one * scale;

            }
            else
            {
                if (t.Value.created)
                {
                    Despawn(t.Value.type, t.Value.k);
                    t.Value.created = false;
                }
            }
        }
    }

    private void Despawn(string type, Transform item)
    {
        if (type == "witch")
            m_WitchPool.Despawn(item);
        else if (type == "location")
            m_LocationPool.Despawn(item);
        else if (type == "spirit")
            m_SpiritPool.Despawn(item);
        else
        {

        }
    }
}

