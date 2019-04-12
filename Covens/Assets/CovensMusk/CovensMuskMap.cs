using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Feature.Style;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovensMuskMap : MonoBehaviour
{
    [SerializeField] private bool m_InitOnStart;

    [SerializeField] private double m_Longitude = -122.3224;
    [SerializeField] private double m_Latitude = 47.70168;

    [SerializeField] private MapsService m_MapsService;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Transform m_MapCenter;
    [SerializeField] private GameObject m_TrackedObjectsContainer;
    
    //musk style properties
    [SerializeField] private Material m_WallMaterial;
    [SerializeField] private Material m_RoofMaterial;
    [SerializeField] private Material m_WaterMaterial;
    [SerializeField] private Material m_SegmentMaterial;
    [SerializeField] private GameObjectOptions m_MapStyle;

    //musk properties
    [SerializeField] private float m_MinCamDistance;
    [SerializeField] private float m_MaxCamDistance;

    [System.Serializable]
    public class CameraDat
    {
        public int zoomLv;
        public float distance;
        public int correction;
        public int segmentWidth;
        public int loadDistance;
        public float zoomSensivity;

        public CameraDat(int zoomLv, float distance, int correction, int segmentWidth, int loadDistance, float zoomSensivity)
        {
            this.zoomLv = zoomLv;
            this.distance = distance;
            this.correction = correction;
            this.segmentWidth = segmentWidth;
            this.loadDistance = loadDistance;
            this.zoomSensivity = zoomSensivity;
        }
    };

    public static CovensMuskMap Instance { get; private set; }

    private Coords m_CoordsUtil;
    private CameraDat[] m_CameraSettings;
    private CameraDat m_CamDat;

    private int m_MinZoom;
    private int m_MaxZoom;

    private float m_Zoom;
    private float m_NormalizedZoom;

    private int m_UnloadDelayId;

    public bool refreshMap = false;
    public float zoom { get { return m_Zoom; } }
    public float normalizedZoom { get { return m_NormalizedZoom; } }
    public CameraDat cameraDat { get { return m_CamDat; } }

    private void Awake()
    {
        Instance = this;

        //initialize map style
        m_MapStyle  = new GameObjectOptions
        {
            ExtrudedStructureStyle = new ExtrudedStructureStyle.Builder
            {
                RoofMaterial = new Material(m_RoofMaterial),
                WallMaterial = new Material(m_WallMaterial)
            }.Build(),
            ModeledStructureStyle = new ModeledStructureStyle.Builder
            {
                BuildingMaterial = new Material(m_WallMaterial)
            }.Build(),
            RegionStyle = new RegionStyle.Builder
            {
                Fill = false,
                FillMaterial = null
            }.Build(),
            AreaWaterStyle = new AreaWaterStyle.Builder
            {
                Fill = true,
                FillMaterial = new Material(m_WaterMaterial)
            }.Build(),
            LineWaterStyle = new LineWaterStyle.Builder
            {
                Material = new Material(m_WaterMaterial)
            }.Build(),
            SegmentStyle = new SegmentStyle.Builder
            {
                Material = new Material(m_SegmentMaterial),
                Width = 5
            }.Build(),
        };

        m_CameraSettings = new CameraDat[]
        {
            //new CameraDat(1, 8192*8, 1280, 2500, 4096000*4),
            new CameraDat(2,    8192*4,     640,    1250,   4096000*2,  2f),
            new CameraDat(3,    8192*2,     320,    625,    4096000,    2f),
            new CameraDat(4,    8192,       160,    312,    2048000,    1.5f),
            new CameraDat(5,    4096,       80,     156,    1024000,    1),
            new CameraDat(6,    2048,       40,     78,     512000,     0.8f),
            new CameraDat(7,    1024,       20,     39,     256000,     0.4f),
            new CameraDat(8,    512,        10,     19,     128000,     0.3f),
            new CameraDat(9,    256,        5,      9,      64000,      0.2f),
            new CameraDat(10,   128,        2,      4,      32000,      0.2f),
            new CameraDat(11,   64,         1,      2,      16000,      0.2f),
            //new CameraDat(12, 32,         1,      1,      8000,       1),
            new CameraDat(13,   16,         1,      1,      4000,       0.2f),
            //new CameraDat(14, 16,         1,      1,      4000,       1),
            //new CameraDat(15, 4,          1,      1,      1200,       1),
            new CameraDat(15,   4,          1,      1,      1200,       0.1f),
            //new CameraDat(16, 6f,         1,      1,      1200,       1),
            new CameraDat(17,   2f,         1,      1,      600,        0.1f),
        };

        //dont generate regions
        m_MapsService.Events.RegionEvents.WillCreate.AddListener(e => e.Cancel = true);

        //force layer of spawned objects
        int markerLayer = 20;
        m_MapsService.Events.AreaWaterEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, markerLayer));
        m_MapsService.Events.LineWaterEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, markerLayer));
        m_MapsService.Events.SegmentEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, markerLayer));
        m_MapsService.Events.ExtrudedStructureEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, markerLayer));
        m_MapsService.Events.ModeledStructureEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, markerLayer));

        //initialize zooom properties based on cameradat settings
        m_MinZoom = m_CameraSettings[0].zoomLv;
        m_MaxZoom = m_CameraSettings[m_CameraSettings.Length - 1].zoomLv;
        m_CamDat = m_CameraSettings[m_CameraSettings.Length - 1];


        //initialize the map at the statue of liberty, cuz thats what I used as reference to setup the map borders quads
        LatLng startingPosition = new LatLng(40.689247, -74.044502); //statue of liberty
        m_MapsService.InitFloatingOrigin(startingPosition);
        m_MapsService.ZoomLevel = -1;
    }

    private void Start()
    {
        if (m_InitOnStart)
            InitMap(m_Longitude, m_Latitude, 1, null);
    }

    public void InitMap(double longitude, double latitude, float normalizedZoom, System.Action callback)
    {
        LatLng newPosition = new LatLng(latitude, longitude);

        m_MapsService.MoveFloatingOrigin(newPosition, new GameObject[] { m_TrackedObjectsContainer });
        m_MapCenter.localPosition = Vector3.zero;

        SetZoom(normalizedZoom);

        refreshMap = true;
        
        m_CoordsUtil = new Coords(m_MapsService.ZoomLevel);
        m_CoordsUtil.InitFloatingOrigin(newPosition);

        callback?.Invoke();
    }

    public void SetZoom(float normalizedZoom)
    {
        m_Zoom = Mathf.Lerp(m_MinZoom, m_MaxZoom, normalizedZoom);
        m_NormalizedZoom = normalizedZoom;

        m_Camera.transform.localPosition = new Vector3(
            0,
            0,
            LeanTween.easeOutQuint(m_MaxCamDistance, m_MinCamDistance, m_NormalizedZoom)
        );

        int oldZoomLv = m_MapsService.ZoomLevel;
        var cameraDat = GetCameraDat();

        if (cameraDat != null)
        {
            m_CamDat = cameraDat;
            m_MapsService.ZoomLevel = m_CamDat.zoomLv;
        }

        if (oldZoomLv != m_MapsService.ZoomLevel)
        {
            //update segment width
            m_MapStyle.SegmentStyle = new SegmentStyle.Builder
            {
                Material = m_MapStyle.SegmentStyle.Material,
                Width = 10.0f * m_CamDat.segmentWidth,
            }.Build();
            
            LeanTween.cancel(m_UnloadDelayId, true);
            m_UnloadDelayId = LeanTween.value(0, 0, 0.2f)
                .setOnComplete(() =>
                {
                    //unload the previous zoom level
                    m_MapsService.MakeMapLoadRegion()
                            .UnloadOutside(oldZoomLv);
                }).uniqueId;


            //load the map at the new zoomlevel
            m_MapsService.MakeMapLoadRegion()
                .AddCircle(m_MapCenter.position, m_CamDat.loadDistance)
                .SetLoadingPoint(m_MapCenter.position)
                .Load(m_MapStyle, m_MapsService.ZoomLevel)
                .UnloadOutside(m_MapsService.ZoomLevel);

            refreshMap = false;
        }
    }

    private CameraDat GetCameraDat()
    {
        CameraDat dat = null;
        float fovy = Mathf.Deg2Rad * m_Camera.fieldOfView;
        float halfFovy = fovy * 0.5f;
        float d = Mathf.Abs(m_Camera.transform.localPosition.z);
        float radius = d / Mathf.Cos(halfFovy);

        for(int i = 0; i < m_CameraSettings.Length; i++)
        {
            if (m_CameraSettings[i].zoomLv > m_CameraSettings[m_CameraSettings.Length - 1].zoomLv) break;

            if (dat != null)
            {
                float dist = m_CameraSettings[i].distance;
                if (radius >= dist * 500)
                {
                    break;
                }
            }
            dat = m_CameraSettings[i];
        }

        return dat;
    }

    private void LateUpdate()
    {
        if (refreshMap)
        {
            refreshMap = false;

            float distanceFromOrigin = Vector3.Distance(m_MapCenter.position, Vector3.zero);

            if (distanceFromOrigin > 5000000)
            {
                Vector3 prevPos = m_MapCenter.localPosition;
                m_MapCenter.localPosition = Vector3.zero;
                m_MapsService.MoveFloatingOrigin(prevPos, new GameObject[] { m_TrackedObjectsContainer });
            }

            m_MapsService.MakeMapLoadRegion()
                .AddCircle(m_MapCenter.position, m_CamDat.loadDistance)
                .SetLoadingPoint(m_MapCenter.position)
                .Load(m_MapStyle, m_MapsService.ZoomLevel)
                .UnloadOutside(m_MapsService.ZoomLevel);
        }
    }

    public Vector3 GetWorldPosition()
    {
        return m_MapCenter.position;
    }

    public Vector3 GetWorldPosition(double longitude, double latitude)
    {
        return m_CoordsUtil.FromLatLngToVector3(new LatLng(latitude, longitude));
    }

    public void SetPosition(double longitude, double latitude)
    {
        Debug.LogError("todo: try to change position first");
        InitMap(longitude, latitude, normalizedZoom, null);
    }
    
    public void GetCoordinates(out double longitude, out double latitude)
    {
        GetCoordinates(m_MapCenter.position, out longitude, out latitude);
    }

    public void GetCoordinates(Vector3 worldPosition, out double longitude, out double latitude)
    {
        LatLng results = m_CoordsUtil.FromVector3ToLatLng(worldPosition);
        longitude = results.Lng;
        latitude = results.Lat;
    }

    public void HideMap(bool hide)
    {
        this.gameObject.SetActive(!hide);
    }
}
