using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class DynamicLabelManager : MonoBehaviour
{

    [SerializeField] float visibleZoom = 2.4f;
    [SerializeField] float minScale = .2f;
    [SerializeField] float maxScale = .5f;
    [SerializeField] float iconMultiplier = 2f;


    public GameObject[] marker;


    private SpriteMapsController sm;
    private Camera cam;
    Dictionary<string, Label> markers = new Dictionary<string, Label>();
    private SimplePool<Transform> m_WitchPool;
    private SimplePool<Transform> m_LocationPool;
    private SimplePool<Transform> m_SpiritPool;
    private SimplePool<Transform> m_OtherPool;

    void Start()
    {
        sm = SpriteMapsController.instance;
        sm.onChangePosition += SetLabels;
        sm.onChangeZoom += SetLabels;
        cam = SpriteMapsController.instance.m_Camera;

        m_WitchPool = new SimplePool<Transform>(marker[0].transform, 50);
        m_LocationPool = new SimplePool<Transform>(marker[1].transform, 50);
        m_SpiritPool = new SimplePool<Transform>(marker[2].transform, 50);
        m_OtherPool = new SimplePool<Transform>(marker[3].transform, 50);
    }

    public void GenerateLabels(WSResponse data)
    {

        foreach (var item in data.labels)
        {
            if (!markers.ContainsKey(item.name))
                markers.Add(item.name, new Label
                {
                    name = item.name,
                    type = item.type,
                    pos = sm.GetWorldPosition(item.longitude, item.latitude)
                });
        }

    }

    void SetLabels()
    {
        float scale = 0;
        if (cam.orthographicSize <= visibleZoom)
            scale = MapUtils.scale(minScale, maxScale, visibleZoom, .01f, cam.orthographicSize);

        foreach (var t in markers)
        {
            if (cam.orthographicSize > visibleZoom)
            {
                if (t.Value.created)
                {
                    Destroy(t.Value.k.gameObject);
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
                        token = m_OtherPool.Spawn();

                    token.SetParent(transform);
                    token.position = t.Value.pos;
                    t.Value.k = token;
                    t.Value.created = true;
                    token.GetComponent<DynamicLabelItem>().Setup(sm);
                }

                if (cam.orthographicSize < .03f)
                    t.Value.k.transform.localScale = Vector3.one * scale * iconMultiplier;
                else
                    t.Value.k.transform.localScale = Vector3.one * scale;

            }
            else
            {
                if (t.Value.created)
                {
                    if (t.Value.type == "witch")
                        m_WitchPool.Despawn(t.Value.k);
                    else if (t.Value.type == "location")
                        m_LocationPool.Despawn(t.Value.k);
                    else if (t.Value.type == "spirit")
                        m_SpiritPool.Despawn(t.Value.k);
                    else
                        m_OtherPool.Despawn(t.Value.k);

                    t.Value.created = false;
                }
            }
        }
    }

}

