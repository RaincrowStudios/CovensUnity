using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class createcountryLabels : MonoBehaviour
{
    [SerializeField] private TextAsset m_GeoData;

    public static createcountryLabels instance { get; set; }
    public GameObject Text;
    public Transform map;

    void Awake()
    {
        instance = this;
    }
		
    public class Properties
    {
        public string name { get; set; }
	
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties2
    {
        public string NAME { get; set; }
		public string CITY_NAME { get; set; }
		public int POP_RANK { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<float> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Properties2 properties { get; set; }
        public int zoom { get; set; }
        public int scale { get; set; }
        public Geometry geometry { get; set; }
    }

    public class RootObject
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public List<Feature> features { get; set; }
    }
}