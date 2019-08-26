using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Raincrow.Maps;
using Newtonsoft.Json;

public class HeatMapManager : MonoBehaviour
{
    public static HeatMapManager instance { get; set; }
    public GameObject heatDot;
    HeatPoint[] m_Heatpoints = new HeatPoint[0];

    IMaps map;
    public float minScale = .2f;
    public float maxScale = .6f;

    private long LastUpdate
    {
        get => long.Parse(PlayerPrefs.GetString("HeatMapManager.LastUpdate", "0"));
        set => PlayerPrefs.SetString("HeatMapManager.LastUpdate", value.ToString());
    }

    private string CachedHeatmapsJson
    {
        get => PlayerPrefs.GetString("HeatMapManager.CachedHeatmap", "[]");
        set => PlayerPrefs.SetString("HeatMapManager.CachedHeatmap", value);
    }

    void Awake()
    {
        instance = this;
        map = MapsAPI.Instance;
    }

    private void Start()
    {
        RetrieveHeatmap(createHeatMap);
    }

    private void OnEnable()
    {
        map.OnCameraUpdate += updateHeatMaps;
    }

    private void OnDisable()
    {
        map.OnCameraUpdate -= updateHeatMaps;
    }

    private void RetrieveHeatmap(System.Action<HeatPoint[]> callback = null)
    {
        System.DateTime lastUpdate = new System.DateTime(LastUpdate);
        System.TimeSpan timeSince = System.DateTime.UtcNow - lastUpdate;

        if (timeSince.TotalHours < 12)
        {
            Debug.Log(timeSince.TotalHours + " hours since last heatmap update");
            callback?.Invoke(JsonConvert.DeserializeObject<List<HeatPoint>>(CachedHeatmapsJson).ToArray());
            return;
        }

        Debug.Log("retrieving heatmaps");
        APIManager.Instance.GetRaincrow("heatmap", null, (response, result) =>
        {
            if (result == 200)
            {
                Debug.Log("retireved heatmaps");

                LastUpdate = System.DateTime.UtcNow.Ticks;
                CachedHeatmapsJson = response;

                callback?.Invoke(JsonConvert.DeserializeObject<List<HeatPoint>>(response).ToArray());
            }
            else
            {
                Debug.LogError("heatmaps failed\n" + response);
                callback?.Invoke(null);
            }
        });
    }

    private void createHeatMap(HeatPoint[] heatpoints)
    {
        m_Heatpoints = heatpoints;
        
        for(int i = 0; i < m_Heatpoints.Length; i++)
        {
            Transform dot = Instantiate(heatDot, Vector3.zero, Quaternion.identity).transform;
            dot.SetParent(map.trackedContainer);
            m_Heatpoints[i].t = dot;
        }
    }

    void updateHeatMaps(bool a, bool b, bool c)
    {
        float sMultiplier = MapUtils.scale(minScale, maxScale, 0, 1, map.normalizedZoom);

        foreach (var item in m_Heatpoints)
        {
            if (map.normalizedZoom < .59f)
            {
                item.t.position = map.GetWorldPosition(item.longitude, item.latitude);
                item.t.gameObject.SetActive(true);
                //scaleDot(item, sMultiplier);
                float sCount = MapUtils.scale(3, 5, 1, 700, item.count);
                item.t.localScale = Vector3.one * sMultiplier * sCount;
            }
            else
            {
                item.t.gameObject.SetActive(false);
            }
        }
    }

    //void scaleDot(HeatPoint hp, float multiplier)
    //{
    //    Mathf.Clamp(hp.count, 1, 1500);
    //    float sCount = MapUtils.scale(3, 5, 1, 700, hp.count);
    //    //	float alphaS = MapUtils.scale (1, .5f, 1, 400, hp.count);
    //    hp.t.localScale = Vector3.one * multiplier * sCount;
    //}


    [ContextMenu("get heatmaps")]
    private void Debug_GetHeatMatps()
    {
        RetrieveHeatmap();
    }
}


public struct HeatPoint
{
    [JsonIgnore]
    public Transform t;

    public int count;
    public double longitude;
    public double latitude;
}
