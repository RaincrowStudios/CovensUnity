using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel;

public class WorldMapLabelManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_LabelPrefab;
    [SerializeField] private int m_BatchSize = 50;

    [Header("city")]
    [SerializeField] private TextAsset m_CityJson;
    [SerializeField] private float m_CityZoomThreshold;
    [SerializeField] private float m_CityMinScale = 1000;
    [SerializeField] private float m_CityMaxScale = 20000;

    [Header("countries")]
    [SerializeField] private TextAsset m_CountryJson;
    [SerializeField] private float m_CountryZoomThreshold;
    [SerializeField] private float m_CountryMinScale = 5000;
    [SerializeField] private float m_CountryMaxScale = 20000;

    private struct CountrySettings
    {
        //public string type;
        //public string name;
        //public object crs;
        public List<LabelEntry> features;
    }

    private class LabelEntry
    {
        public string type;
        public float zoom;
        public float scale;
        public LabelGeometry geometry;
        public LabelProperty properties;

        [Newtonsoft.Json.JsonIgnore] public TextMeshPro instance;
        [Newtonsoft.Json.JsonIgnore] public Vector3 coordinates;
    }

    private struct LabelProperty
    {
        public string NAME;
        public string CITY_NAME;
        public int POP_RANK;
    }

    private struct LabelGeometry
    {
        public string type;
        public double[] coordinates;
    }

    private SimplePool<TextMeshPro> m_LabelPool;
    private List<LabelEntry> m_Cities;
    private List<LabelEntry> m_Countries;
    private Dictionary<string, LabelEntry> m_ActiveLabels;
    private float m_CityNormalizedZoom;
    private float m_CountryNormalizedZoom;
    private float m_FontScale;

    private bool m_CityLevel;
    private bool m_CountryLevel;

    private int m_BatchIndex = 0;

    private void Awake()
    {
        m_Cities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LabelEntry>>(m_CityJson.text);
        m_Countries = Newtonsoft.Json.JsonConvert.DeserializeObject<CountrySettings>(m_CountryJson.text).features;
        m_LabelPool = new SimplePool<TextMeshPro>(m_LabelPrefab, 20);
        m_ActiveLabels = new Dictionary<string, LabelEntry>();

        double[] auxCoords;

        for (int i = 0; i < m_Cities.Count; i++)
        {
            auxCoords = m_Cities[i].geometry.coordinates;
            m_Cities[i].coordinates = new Vector3((float)auxCoords[0], (float)auxCoords[1], 0);
            m_Cities[i].properties.NAME = m_Cities[i].properties.CITY_NAME;
            m_Cities[i].properties.CITY_NAME = null;
            m_Cities[i].zoom = 1f;
        }

        for (int i = 0; i < m_Countries.Count; i++)
        {
            auxCoords = m_Countries[i].geometry.coordinates;
            m_Countries[i].coordinates = new Vector3((float)auxCoords[0], (float)auxCoords[1], 0);
        }
    }

    private void Start()
    {
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;
    }

    private void OnStartFlying()
    {
        //MapsAPI.Instance.OnCameraUpdate += OnCameraUpdate;
        StartCoroutine(UpdateLabelsCoroutine());
    }

    private void OnStopFlying()
    {
        //MapsAPI.Instance.OnCameraUpdate -= OnCameraUpdate;
        StopAllCoroutines();
    }

    private void Update()//OnCameraUpdate(bool position, bool zoom, bool twist)
    {
        bool cityLevel =
            MapsAPI.Instance.normalizedZoom < m_CityZoomThreshold &&
            MapsAPI.Instance.normalizedZoom > m_CountryZoomThreshold;

        bool countryLevel = MapsAPI.Instance.normalizedZoom < m_CountryZoomThreshold;
        bool despawn = false;

        if (m_CountryLevel != countryLevel)
        {
            if (countryLevel)
            {
            }
            else
            {
                despawn = true;
            }
            m_CountryLevel = countryLevel;
        }


        if (m_CityLevel != cityLevel)
        {
            if (cityLevel)
            {
            }
            else
            {
                despawn = true;
            }
            m_CityLevel = cityLevel;
        }


        if (despawn)
        {
            foreach(var entry in m_ActiveLabels.Values)
            {
                m_LabelPool.Despawn(entry.instance);
                entry.instance = null;
            }
            m_ActiveLabels.Clear();
            m_BatchIndex = 0;
        }

        if (m_CountryLevel)
        {
            m_CountryNormalizedZoom = (Mathf.Clamp(MapsAPI.Instance.normalizedZoom, 0.05f, m_CountryZoomThreshold) - m_CountryZoomThreshold) / (0.05f - m_CountryZoomThreshold);
            m_FontScale = Mathf.Lerp(m_CountryMinScale, m_CountryMaxScale, LeanTween.easeInCubic(0, 1, m_CountryNormalizedZoom));
        }
        else
        {
            m_CityNormalizedZoom = (Mathf.Clamp(MapsAPI.Instance.normalizedZoom, m_CountryZoomThreshold, m_CityZoomThreshold) - m_CityZoomThreshold) / (m_CountryZoomThreshold - m_CityZoomThreshold);
            m_FontScale = Mathf.Lerp(m_CityMinScale, m_CityMaxScale, LeanTween.easeInCubic(0, 1, m_CityNormalizedZoom));
        }
            
        foreach (var entry in m_ActiveLabels.Values)
        {
            //update scale
            entry.instance.transform.localScale = Vector3.one * m_FontScale * entry.zoom;
            //update alpha
            entry.instance.alpha = Mathf.Lerp(entry.instance.alpha, 1, Time.deltaTime * 2);
        }
    }

    /// <summary>
    /// Checks if the labels is in/outside the screen and add or remove from the dictionary
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateLabelsCoroutine()
    {
        List<LabelEntry> auxList;
        LabelEntry auxLabel;

        while (true)
        {
            if (m_CountryLevel == false && m_CityLevel == false)
            {
                yield return 0;
                continue;
            }

            if(m_CountryLevel)
                auxList = m_Countries;
            else
                auxList = m_Cities;

            int from = m_BatchIndex;
            int to = Mathf.Min(m_BatchIndex + m_BatchSize, auxList.Count);

            m_BatchIndex = m_BatchIndex + m_BatchSize;

            if (m_BatchIndex >= auxList.Count)
                m_BatchIndex = 0;

            for (int i = from; i < to; i++)
            {
                auxLabel = auxList[i];
                if (MapsAPI.Instance.coordinateBounds.Contains(auxLabel.coordinates)) //is in screen view
                {
                    if (auxLabel.instance == null) //is not showing
                    {
                        //spawn
                        auxLabel.instance = m_LabelPool.Spawn();
                        auxLabel.instance.alpha = 0;
                        auxLabel.instance.text = auxLabel.properties.NAME;
                        auxLabel.instance.transform.SetParent(MapsAPI.Instance.trackedContainer);
                        m_ActiveLabels.Add(auxLabel.properties.NAME, auxLabel);

                        //set position
                        auxLabel.instance.transform.position = MapsAPI.Instance.GetWorldPosition(auxLabel.coordinates.x, auxLabel.coordinates.y);
                        //update scale
                        auxLabel.instance.transform.localScale = Vector3.one * m_FontScale * auxLabel.zoom;
                    }
                }
                else if (auxLabel.instance != null)
                {
                    //despawn
                    m_LabelPool.Despawn(auxLabel.instance);
                    auxLabel.instance.alpha = 0;
                    auxLabel.instance = null;
                    m_ActiveLabels.Remove(auxLabel.properties.NAME);
                }
            }

            yield return 0;
        }
    }

    private void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log("[WorldLabels] " + msg);
#endif
    }
}
