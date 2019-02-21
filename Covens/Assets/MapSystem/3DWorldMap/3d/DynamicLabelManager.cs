using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class DynamicLabelManager : MonoBehaviour
{

    [SerializeField] float minScaleCity = .2f;
    [SerializeField] float maxScaleCity = .6f;

    [SerializeField] float visibleZoomCity = 2.4f;
    [SerializeField] float minVisibleZoomCity = 1.3f;

    [SerializeField] float minScaleTown = .2f;
    [SerializeField] float maxScaleTown = .6f;

    [SerializeField] float visibleZoomTown = 2.4f;
    [SerializeField] float minVisibleZoomTown = 1.3f;



    [SerializeField] float visibleZoomMarker = 2.4f;
    [SerializeField] float minVisibleZoomMarker = 1.3f;

    [SerializeField] TextMeshPro m_LabelPrefab;
    public GameObject heatDot;

    public GameObject spirit;
    public GameObject[] collectible;
    public GameObject pop;
    public GameObject witch;

    private SpriteMapsController sm;
    private Camera cam;
    private float minZoom;
    private float maxZoom;
    Dictionary<string, Label> labels = new Dictionary<string, Label>();
    Dictionary<string, Label> labelsTowns = new Dictionary<string, Label>();
    Dictionary<string, Label> markers = new Dictionary<string, Label>();

    void Start()
    {
        sm = SpriteMapsController.instance;
        sm.OnMapUpdated += SetLabels;
        cam = SpriteMapsController.instance.m_Camera;
        minZoom = sm.m_MinZoom;
        maxZoom = sm.m_MaxZoom;
    }

    public void GenerateLabels(WSResponse data)
    {
        if (labels.Count > 1000)
            labels.Clear();

        if (labelsTowns.Count > 1000)
            labelsTowns.Clear();

        if (markers.Count > 1000)
            markers.Clear();

        if (data.command == "city")
        {
            foreach (var item in data.labels)
            {
                if (!labels.ContainsKey(item.name))
                    labels.Add(item.name, new Label
                    {
                        name = item.name,
                        pos = sm.GetWorldPosition(item.longitude, item.latitude)
                    });
            }
        }
        else if (data.command == "town")
        {
            foreach (var item in data.labels)
            {
                if (!labelsTowns.ContainsKey(item.name))
                    labelsTowns.Add(item.name, new Label
                    {
                        name = item.name,
                        pos = sm.GetWorldPosition(item.longitude, item.latitude)
                    });
            }
        }
        else
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
        //		print (labels.Count + "   +++ " );


    }

    void SetLabels()
    {
        //		print (labels.Count);
        TextMeshPro label;
        float clampZoomCity = Mathf.Clamp(cam.orthographicSize, minVisibleZoomCity, visibleZoomCity);
        float multiplierCity = MapUtils.scale(minScaleCity, maxScaleCity, minVisibleZoomCity, visibleZoomCity, clampZoomCity);
        var alphaCity = MapUtils.scale(.45f, 0, minVisibleZoomCity, visibleZoomCity, clampZoomCity);


        foreach (var t in labels)
        {
            if (cam.orthographicSize > visibleZoomCity || cam.orthographicSize < minVisibleZoomCity)
            {
                if (t.Value.created)
                {
                    Destroy(t.Value.t.gameObject);
                    t.Value.created = false;
                }
                continue;
            }
            if (MapUtils.inMapView(t.Value.pos, cam))
            {
                if (!t.Value.created)
                {
                    label = (TextMeshPro)Instantiate(m_LabelPrefab, Vector3.zero, Quaternion.identity);
                    label.fontSize += .01f;
                    label.text = t.Value.name;
                    label.transform.SetParent(transform);
                    label.transform.position = t.Value.pos;
                    t.Value.t = label;
                    t.Value.created = true;
                }
                t.Value.t.transform.localScale = Vector3.one * multiplierCity;
                t.Value.t.alpha = alphaCity;
            }
            else
            {
                if (t.Value.created)
                {
                    Destroy(t.Value.t.gameObject);
                    t.Value.created = false;
                }
            }
        }

        float clampZoomTown = Mathf.Clamp(cam.orthographicSize, minVisibleZoomTown, visibleZoomTown);
        float multiplierTown = MapUtils.scale(minScaleTown, maxScaleTown, minVisibleZoomTown, visibleZoomTown, clampZoomTown);
        var alphaTown = MapUtils.scale(.45f, 0, minVisibleZoomTown, visibleZoomTown, clampZoomTown);


        foreach (var t in labelsTowns)
        {
            if (cam.orthographicSize > visibleZoomTown || cam.orthographicSize < minVisibleZoomTown)
            {
                if (t.Value.created)
                {
                    Destroy(t.Value.t.gameObject);
                    t.Value.created = false;
                }
                continue;
            }
            if (MapUtils.inMapView(t.Value.pos, cam))
            {
                if (!t.Value.created)
                {
                    label = (TextMeshPro)Instantiate(m_LabelPrefab, Vector3.zero, Quaternion.identity);
                    label.fontSize += .01f;
                    label.text = t.Value.name;
                    label.transform.SetParent(transform);
                    label.transform.position = t.Value.pos;
                    t.Value.t = label;
                    t.Value.created = true;
                }
                t.Value.t.transform.localScale = Vector3.one * multiplierTown;
                t.Value.t.alpha = alphaTown;
            }
            else
            {
                if (t.Value.created)
                {
                    Destroy(t.Value.t.gameObject);
                    t.Value.created = false;
                }
            }
        }

        //		float clampZoomMarker = Mathf.Clamp (cam.orthographicSize, minVisibleZoomMarker, visibleZoomMarker);
        //		float multiplierMarker = MapUtils.scale(minScaleMarker, maxScaleMarker, minVisibleZoomMarker, visibleZoomMarker, clampZoomMarker); 


        foreach (var t in markers)
        {
            if (cam.orthographicSize > visibleZoomMarker || cam.orthographicSize < minVisibleZoomMarker)
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
                        prefab = witch;
                    else if (t.Value.type == "collectible")
                        prefab = collectible[Random.Range(0, 3)];
                    else if (t.Value.type == "spirit")
                        prefab = spirit;
                    else if (t.Value.type == "location")
                        prefab = pop;

                    var token = Instantiate(prefab, Vector3.zero, Quaternion.identity).transform;

                    token.SetParent(transform);
                    token.position = t.Value.pos;
                    t.Value.k = token;
                    t.Value.created = true;
                }
                //				t.Value.k.transform.localScale = Vector3.one * multiplierMarker ;

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

