using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldMapLabelManager : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_LabelPrefab;

    [Header("city")]
    [SerializeField] private TextAsset m_CityJson;
    [SerializeField] private float m_CityZoomThreshold;

    [Header("countries")]
    [SerializeField] private TextAsset m_CountryJson;
    [SerializeField] private float m_CountryZoomThreshold;

    private struct CityLabelEntry
    {
        public string type;
        public CityLabelProperty properties;
        public CityLabelGeometry geometry;
    }

    private struct CityLabelProperty
    {
        public string CITY_NAME;
        public int POP_RANK;
    }

    private struct CityLabelGeometry
    {
        public string type;
        public double[] coordinates;
    }

    private List<CityLabelEntry> m_Cities;
    private SimplePool<TextMeshPro> m_LabelPool;
    private List<TextMeshPro> m_ActiveLabels = new List<TextMeshPro>();

    private bool m_CityLevel;
    private bool m_CountryLevel;
    
    private void Awake()
    {
        m_Cities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CityLabelEntry>>(m_CityJson.text);
        m_LabelPool = new SimplePool<TextMeshPro>(m_LabelPrefab, 20);
    }

    private void Start()
    {
        MapsAPI.Instance.OnExitStreetLevel += OnStartFlying;
        MapsAPI.Instance.OnEnterStreetLevel += OnStopFlying;
    }

    private void OnStartFlying()
    {
        MapsAPI.Instance.OnCameraUpdate += OnCameraUpdate;
    }

    private void OnStopFlying()
    {
        MapsAPI.Instance.OnCameraUpdate -= OnCameraUpdate;
    }

    private void OnCameraUpdate(bool position, bool zoom, bool twist)
    {
        bool cityLevel = 
            MapsAPI.Instance.normalizedZoom < m_CityZoomThreshold &&
            MapsAPI.Instance.normalizedZoom > m_CountryZoomThreshold;

        bool countryLevel = MapsAPI.Instance.normalizedZoom < m_CountryZoomThreshold;

        if (m_CountryLevel != countryLevel)
        {
            if (countryLevel)
            {
                Log("entered country level");
            }
            else
            {
                Log("left country level");
                //despawn all visible country labels
            }
            m_CountryLevel = countryLevel;
        }


        if (m_CityLevel != cityLevel)
        {
            if(cityLevel)
            {
                Log("entered city level");
            }
            else
            {
                Log("left city level");
                //despawn all visible citylabels
            }
            m_CityLevel = cityLevel;
        }

        if(countryLevel)
        {
            //update country labels
        }

        if (cityLevel)
        {
            //update city labels
        }
    }

    private void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log("[WorldLabels] " + msg);
#endif
    }
}
