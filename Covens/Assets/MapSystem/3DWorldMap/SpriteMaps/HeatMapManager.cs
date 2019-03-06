using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HeatMapManager : MonoBehaviour
{
    public static HeatMapManager instance { get; set; }
    public GameObject heatDot;
    SpriteMapsController sm;
    List<HeatPoint> heatpoints = new List<HeatPoint>();

    Camera cam;
    public float minScale = .2f;
    public float maxScale = .6f;
    float minZoom;
    float maxZoom;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        sm = SpriteMapsController.instance;
        sm.onChangePosition += updateHeatMaps;
        sm.onChangeZoom += updateHeatMaps;
        cam = Camera.main;
        minZoom = sm.m_MinZoom;
        maxZoom = sm.m_MaxZoom;
        createHeatMap(PlayerDataManager.config.heatmap);
    }

    public void createHeatMap(List<HeatMapPoints> heatpoints)
    {
        foreach (var item in heatpoints)
        {
            this.heatpoints.Add(new HeatPoint
            {
                count = item.count,
                pos = sm.GetWorldPosition(item.longitude, item.latitude)
            });
        }
    }

    void updateHeatMaps()
    {
        //		float mapScale
        float sMultiplier = MapUtils.scale(minScale, maxScale, minZoom, maxZoom, cam.orthographicSize);

        foreach (var item in heatpoints)
        {
            if (MapUtils.inMapView(item.pos, SpriteMapsController.instance.m_Camera))
            {
                if (!item.created)
                {
                    Transform dot = Instantiate(heatDot, Vector3.zero, Quaternion.identity).transform;
                    dot.SetParent(transform);
                    dot.position = item.pos;
                    item.created = true;
                    item.t = dot;
                    item.sr = dot.GetComponent<SpriteRenderer>();
                }
                scaleDot(item, sMultiplier);
            }
            else
            {
                if (item.created)
                {
                    Destroy(item.t.gameObject);
                    item.created = false;
                }
            }
        }
    }

    void scaleDot(HeatPoint hp, float multiplier)
    {
        Mathf.Clamp(hp.count, 1, 700);
        float sCount = MapUtils.scale(3, 7, 1, 700, hp.count);
        //	float alphaS = MapUtils.scale (1, .5f, 1, 400, hp.count);
        hp.t.localScale = Vector3.one * multiplier * sCount;
        //	hp.sr.color = new Color (hp.sr.color.r, hp.sr.color.g,  hp.sr.color.b, alphaS);

    }
}



public class HeatPoint
{
    public bool created = false;
    public Transform t;
    public SpriteRenderer sr;
    public Vector3 pos;
    public int count;
}

