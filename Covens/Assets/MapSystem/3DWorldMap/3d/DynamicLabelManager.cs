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

    void Start()
    {
        sm = SpriteMapsController.instance;
        sm.onChangePosition += SetLabels;
        sm.onChangeZoom += SetLabels;
        cam = SpriteMapsController.instance.m_Camera;
    }

    public void GenerateLabels(WSResponse data)
    {
        if (markers.Count > 50)
        {
            markers.Clear();
        }

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
                    GameObject prefab = null;
                    if (t.Value.type == "witch")
                        prefab = marker[0];
                    else if (t.Value.type == "location")
                        prefab = marker[1];
                    else if (t.Value.type == "spirit")
                        prefab = marker[2];
                    else
                        prefab = marker[3];

                    var token = Instantiate(prefab, Vector3.zero, Quaternion.identity).transform;

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
                    Destroy(t.Value.k.gameObject);
                    t.Value.created = false;
                }
            }
        }
    }

}

