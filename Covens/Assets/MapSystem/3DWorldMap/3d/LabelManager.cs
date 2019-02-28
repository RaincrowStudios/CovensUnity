using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class LabelManager : MonoBehaviour
{

    [SerializeField] float minScale = .2f;
    [SerializeField] float maxScale = .6f;
    [SerializeField] TextAsset txtAsset;
    [SerializeField] TextMeshPro m_LabelPrefab;
    [SerializeField] float visibleZoom = 2.4f;
    [SerializeField] float minVisibleZoom = 1.3f;

    private SpriteMapsController sm;
    private Camera cam;
    private float minZoom;
    private float maxZoom;
    List<Label> labels = new List<Label>();
    void Start()
    {
        sm = SpriteMapsController.instance;
        InitLabels();
        sm.onChangePosition += SetLabels;
        sm.onChangeZoom += SetLabels;
        cam = SpriteMapsController.instance.m_Camera;
        minZoom = sm.m_MinZoom;
        maxZoom = sm.m_MaxZoom;
    }

    void InitLabels()
    {
        var dataState = JsonConvert.DeserializeObject<List<stateLabel>>(txtAsset.text);
        foreach (var item in dataState)
        {
            labels.Add(new Label { name = item.name, zoom = 3, pos = sm.GetWorldPosition(item.coordinates[0], item.coordinates[1]) });
        }
    }

    void SetLabels()
    {
        TextMeshPro label;
        float clampZoom = Mathf.Clamp(cam.orthographicSize, minVisibleZoom, visibleZoom);
        float multiplier = MapUtils.scale(minScale, maxScale, minVisibleZoom, visibleZoom, clampZoom);
        var alpha = MapUtils.scale(.45f, 0, minVisibleZoom, visibleZoom, clampZoom);
        foreach (var t in labels)
        {
            if (cam.orthographicSize > visibleZoom || cam.orthographicSize < minVisibleZoom)
            {
                if (t.created)
                {
                    Destroy(t.t.gameObject);
                    t.created = false;
                }
                continue;
            }
            if (MapUtils.inMapView(t.pos, cam))
            {
                if (!t.created)
                {
                    label = (TextMeshPro)Instantiate(m_LabelPrefab, Vector3.zero, Quaternion.identity);
                    label.fontSize += .01f;
                    label.text = t.name;
                    label.transform.SetParent(transform);
                    label.transform.position = t.pos;
                    t.t = label;
                    t.created = true;
                }
                t.t.transform.localScale = Vector3.one * multiplier;
                t.t.alpha = alpha;
            }
            else
            {
                if (t.created)
                {
                    Destroy(t.t.gameObject);
                    t.created = false;
                }
            }
        }

    }

}

