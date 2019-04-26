using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Raincrow.Maps;

public class HeatMapManager : MonoBehaviour
{
    public static HeatMapManager instance { get; set; }
    public GameObject heatDot;
    List<HeatPoint> heatpoints = new List<HeatPoint>();
    IMaps map;
    public float minScale = .2f;
    public float maxScale = .6f;
    // float minZoom;
    // float maxZoom;

    void Awake()
    {
        instance = this;
    }

    public void createHeatMap(List<HeatMapPoints> heatpoints)
    {
        map = MapsAPI.Instance;
        map.OnChangePosition += updateHeatMaps;
        map.OnChangeZoom += updateHeatMaps;
        Debug.Log("Creating Heat maps");
        Debug.Log("Heat Points Count " + heatpoints.Count);
        foreach (var item in heatpoints)
        {
            var hp = new HeatPoint();
            hp.count = item.count;
            hp.pos = map.GetWorldPosition(item.longitude, item.latitude);
            Transform dot = Instantiate(heatDot, Vector3.zero, Quaternion.identity).transform;
            dot.SetParent(map.trackedContainer);
            dot.position = hp.pos;
            hp.t = dot;
            this.heatpoints.Add(hp);
        }
    }

    void updateHeatMaps()
    {
        //		float mapScale
        float sMultiplier = MapUtils.scale(minScale, maxScale, 0, 1, map.normalizedZoom);

        foreach (var item in heatpoints)
        {
            if (map.normalizedZoom < .59f)
            {
                item.t.gameObject.SetActive(true);
                scaleDot(item, sMultiplier);
            }
            else
            {
                item.t.gameObject.SetActive(false);
            }
        }
    }

    void scaleDot(HeatPoint hp, float multiplier)
    {
        Mathf.Clamp(hp.count, 1, 1500);
        float sCount = MapUtils.scale(3, 5, 1, 700, hp.count);
        //	float alphaS = MapUtils.scale (1, .5f, 1, 400, hp.count);
        hp.t.localScale = Vector3.one * multiplier * sCount;
    }
}



public class HeatPoint
{
    public bool created = false;
    public Transform t;
    public Vector3 pos;
    public int count;
}
