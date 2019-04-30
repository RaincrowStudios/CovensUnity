using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Event;
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
        public float panSensivity;

        public CameraDat(int zoomLv, float distance, int correction, int segmentWidth, int loadDistance, float zoomSensivity, float panSensivity)
        {
            this.zoomLv = zoomLv;
            this.distance = distance;
            this.correction = correction;
            this.segmentWidth = segmentWidth;
            this.loadDistance = loadDistance;
            this.zoomSensivity = zoomSensivity;
            this.panSensivity = panSensivity;
        }
    };

    //public static CovensMuskMap Instance { get; private set; }

    private CameraDat[] m_CameraSettings;
    private CameraDat m_CamDat;

    private int m_MinZoom;
    private int m_MaxZoom;

    private float m_Zoom;
    private float m_NormalizedZoom;

    private float m_LastFloatOriginUpdate;

    private bool m_BuildingsEnabled;
    private System.Action m_OnMapLoaded;

    public bool refreshMap = false;
    public float zoom { get { return m_Zoom; } }
    public float normalizedZoom { get { return m_NormalizedZoom; } }
    public CameraDat cameraDat { get { return m_CamDat; } }

    public bool streetLevel { get { return m_MapsService.ZoomLevel == 17; } }

    public Transform itemContainer { get { return m_TrackedObjectsContainer.transform; } }

    public Vector3 topLeftBorder { get; private set; }
    public Vector3 botRightBorder { get; private set; }


    private Vector3 m_LocalBotLeft;
    private Vector3 m_LocalTopRight;

    public Bounds coordsBounds { get; set; }

    public System.Action onMoveFloatingOrigin;
    public System.Action onWillChangeZoomLevel;

    private void Awake()
    {
        MapsAPI.Instance.InstantiateMap();

        DontDestroyOnLoad(this.gameObject);
        //Instance = this;

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
                BuildingMaterial = new Material(m_WallMaterial),
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
            new CameraDat(2,    8192*4,     640,    1250,   4096000*2,  2f,     1.5f),
            new CameraDat(3,    8192*2,     320,    625,    4096000,    2f,     1f),
            new CameraDat(4,    8192,       160,    312,    2048000,    1.5f,   1f),
            new CameraDat(5,    4096,       80,     156,    1024000,    1,      1f),
            new CameraDat(6,    2048,       40,     78,     512000,     0.8f,   1f),
            new CameraDat(7,    1024,       20,     39,     256000,     0.4f,   1f),
            new CameraDat(8,    512,        10,     19,     128000,     0.4f,   1f),
            new CameraDat(9,    256,        5,      9,      64000,      0.3f,   1f),
            new CameraDat(10,   128,        2,      4,      32000,      0.3f,   1f),
            new CameraDat(11,   64,         1,      2,      16000,      0.3f,   1f),
            new CameraDat(12,   32,         1,      1,      8000,       0.3f,      1f),
            new CameraDat(13,   16,         1,      1,      4000,       0.3f,   1f),
            //new CameraDat(14,   16,         1,      1,      4000,       0.3f,      1f),
            new CameraDat(15,   4,          1,      1,      1200,       0.2f,   1f),
            //new CameraDat(16, 6f,         1,      1,      1200,       1, 1f),
            new CameraDat(17,   2f,         1,      1,      600,        0.2f,   1f),
        };

        //dont generate regions
        m_MapsService.Events.RegionEvents.WillCreate.AddListener(e => e.Cancel = true);

        m_MapsService.Events.MapEvents.LoadError.AddListener(OnMapLoadError);
        m_MapsService.Events.MapEvents.Progress.AddListener(OnMapLoadProgress);
        m_MapsService.Events.MapEvents.Loaded.AddListener(OnMapLoaded);
        m_MapsService.Events.ExtrudedStructureEvents.WillCreate.AddListener(OnWillCreateExtrudedStructure);
        m_MapsService.Events.ExtrudedStructureEvents.DidCreate.AddListener(OnDidCreateExtrudedStructure);
        m_MapsService.Events.ModeledStructureEvents.WillCreate.AddListener(OnWillCreateModeledStructure);
        m_MapsService.Events.ModeledStructureEvents.DidCreate.AddListener(OnDidCreateModeledStructure);

        //force layer of spawned objects
        int mapLayer = 17;
        m_MapsService.Events.AreaWaterEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, mapLayer));
        m_MapsService.Events.LineWaterEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, mapLayer));
        m_MapsService.Events.SegmentEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, mapLayer));
        m_MapsService.Events.ExtrudedStructureEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, mapLayer));
        m_MapsService.Events.ModeledStructureEvents.DidCreate.AddListener(e => MapCameraUtils.SetLayer(e.GameObject.transform, mapLayer));

        //initialize zooom properties based on cameradat settings
        m_MinZoom = m_CameraSettings[0].zoomLv;
        m_MaxZoom = m_CameraSettings[m_CameraSettings.Length - 1].zoomLv;
        m_CamDat = m_CameraSettings[m_CameraSettings.Length - 1];

        m_BuildingsEnabled = Application.isEditor;
    }

    private void Start()
    {
        LatLng startingPosition = new LatLng(0, 0);
        m_MapsService.InitFloatingOrigin(startingPosition);
        m_MapsService.ZoomLevel = -1;

        if (m_InitOnStart)
            InitMap(m_Longitude, m_Latitude, 1, null);
    }

    public void InitMap(double longitude, double latitude, float normalizedZoom, System.Action callback)
    {
        LatLng newPosition = new LatLng(latitude, longitude);

        m_MapsService.MoveFloatingOrigin(newPosition, new GameObject[] { m_TrackedObjectsContainer });
        m_MapCenter.localPosition = Vector3.zero;

        refreshMap = true;

        UpdateBorders();
        SetZoom(normalizedZoom);

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
            onWillChangeZoomLevel?.Invoke();

            //update segment width
            m_MapStyle.SegmentStyle = new SegmentStyle.Builder
            {
                Material = m_MapStyle.SegmentStyle.Material,
                Width = 10.0f * m_CamDat.segmentWidth,
            }.Build();
            
            //LeanTween.cancel(m_UnloadDelayId, true);
            //m_UnloadDelayId = LeanTween.value(0, 0, 0.2f)
            //    .setOnComplete(() =>
            //    {
            //unload the previous zoom level
            m_MapsService.MakeMapLoadRegion()
                .UnloadOutside(oldZoomLv);
            //    }).uniqueId;
            
            //load the map at the new zoomlevel
            m_MapsService.MakeMapLoadRegion()
                .AddCircle(m_MapCenter.position, m_CamDat.loadDistance)
                .SetLoadingPoint(m_MapCenter.position)
                .Load(m_MapStyle, m_MapsService.ZoomLevel)
                .UnloadOutside(m_MapsService.ZoomLevel);

            refreshMap = false;
        }

        UpdateBounds();
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

            if (Time.time - m_LastFloatOriginUpdate > 0.25f)
            {
                float distanceFromOrigin = Vector3.Distance(m_MapCenter.position, Vector3.zero);

                //reposition floating origin
                if (distanceFromOrigin > 4000000)
                    MoveFloatingOrigin(m_MapCenter.position, true);
            }

            m_MapsService.MakeMapLoadRegion()
                .AddCircle(m_MapCenter.position, m_CamDat.loadDistance)
                .SetLoadingPoint(m_MapCenter.position)
                .Load(m_MapStyle, m_MapsService.ZoomLevel)
                .UnloadOutside(m_MapsService.ZoomLevel);

            UpdateBounds();
        }
    }
       
    public void MoveFloatingOrigin(Vector3 worldPos, bool recenterMap)
    {
        m_LastFloatOriginUpdate = Time.deltaTime;

        if (recenterMap)
        {
            m_MapsService.MoveFloatingOrigin(worldPos, new GameObject[] { m_TrackedObjectsContainer });
            m_MapCenter.localPosition = Vector3.zero;
        }
        else
        {
            m_MapsService.MoveFloatingOrigin(worldPos, new GameObject[] { m_TrackedObjectsContainer, m_MapCenter.gameObject });
        }

        UpdateBorders();

        onMoveFloatingOrigin?.Invoke();

        refreshMap = true;
    }

    private void OnMapLoaded(MapLoadedArgs e)
    {
        m_OnMapLoaded?.Invoke();
        m_OnMapLoaded = null;
    }

    private void OnMapLoadProgress(MapLoadProgressArgs e)
    {
        //Debug.Log(e.Progress + "(" + m_MapsService.ZoomLevel + ")");
    }

    private void OnMapLoadError(MapLoadErrorArgs args)
    {
        Debug.LogError("load map error [" + args.DetailedErrorCode + "] " + args.Message);
    }

    private void OnDidCreateExtrudedStructure(DidCreateExtrudedStructureArgs e)
    {
        float height = e.MapFeature.Shape.BoundingBox.size.y;
        
        if (height > 50)
        {
            e.GameObject.transform.localScale = new Vector3(1, 50 / height, 1);
        }

#if UNITY_EDITOR
        e.GameObject.name = "h(" + height + ") " + e.GameObject.name;
#endif
    }

    private void OnWillCreateExtrudedStructure(WillCreateExtrudedStructureArgs e)
    {
        e.Cancel = !m_BuildingsEnabled;
    }

    private void OnWillCreateModeledStructure(WillCreateModeledStructureArgs e)
    {
        e.Cancel = !m_BuildingsEnabled;
    }

    private void OnDidCreateModeledStructure(DidCreateModeledStructureArgs e)
    {
    }

    private void UpdateBorders()
    {
        topLeftBorder = GetWorldPosition(-179.9, 84.9);
        botRightBorder = GetWorldPosition(179.9, -84.9);
    }

    private void UpdateBounds()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 coordsBotLeft;
        Vector3 coordsTopRight;
        Vector3 coordsCenter;

        Ray ray;
        float distance;
        double lat, lng;

        MapsAPI.Instance.GetPosition(out lng, out lat);
        coordsCenter = new Vector3((float)lng, (float)lat);

        ray = m_Camera.ViewportPointToRay(new Vector3(-0.075f, -0.2f, -m_Camera.transform.localPosition.z));
        plane.Raycast(ray, out distance);
        m_LocalBotLeft = ray.GetPoint(distance);
        MapsAPI.Instance.GetPosition(m_LocalBotLeft, out lng, out lat);
        coordsBotLeft = new Vector3((float)lng, (float)lat);

        ray = m_Camera.ViewportPointToRay(new Vector3(1.075f, 1.075f, -m_Camera.transform.localPosition.z));
        plane.Raycast(ray, out distance);
        m_LocalTopRight = ray.GetPoint(distance);
        MapsAPI.Instance.GetPosition(m_LocalTopRight, out lng, out lat);
        coordsTopRight = new Vector3((float)lng, (float)lat);

        coordsBounds = new Bounds(coordsCenter, coordsTopRight - coordsBotLeft);        
        m_LocalBotLeft = m_MapCenter.InverseTransformPoint(m_LocalBotLeft);
        m_LocalTopRight = m_MapCenter.InverseTransformPoint(m_LocalTopRight);
    }

    public Vector3 GetWorldPosition()
    {
        return m_MapCenter.position;
    }

    public Vector3 GetWorldPosition(double longitude, double latitude)
    {
        return m_MapsService.Coords.FromLatLngToVector3(new LatLng(latitude, longitude));
    }

    public void SetPosition(double longitude, double latitude)
    {
        InitMap(longitude, latitude, normalizedZoom, null);
    }
    
    public void GetCoordinates(out double longitude, out double latitude)
    {
        GetCoordinates(m_MapCenter.position, out longitude, out latitude);
    }

    public void GetCoordinates(Vector3 worldPosition, out double longitude, out double latitude)
    {
        LatLng results = m_MapsService.Coords.FromVector3ToLatLng(worldPosition);
        longitude = results.Lng;
        latitude = results.Lat;
    }

    public void HideMap(bool hide)
    {
        this.gameObject.SetActive(!hide);
    }

    public void EnableBuildings(bool enable)
    {        
        if (m_BuildingsEnabled != enable)
        {    
            //reload the map with the new settings only after it finishes reloading
            m_OnMapLoaded += () =>
            {
                m_BuildingsEnabled = enable;
                m_MapsService.MakeMapLoadRegion()
                    .AddCircle(m_MapCenter.position, m_CamDat.loadDistance)
                    .SetLoadingPoint(m_MapCenter.position)
                    .Load(m_MapStyle, m_MapsService.ZoomLevel)
                    .UnloadOutside(m_MapsService.ZoomLevel);
            };

            //unload the current map
            m_BuildingsEnabled = true;

            m_MapsService.MakeMapLoadRegion()
                .AddCircle(m_MapCenter.position + Vector3.right * m_CamDat.loadDistance * 2, 0)
                .UnloadOutside(m_MapsService.ZoomLevel);
        }
    }

    public bool IsPointInsideView(Vector3 point)
    {
        Vector3 localPoint = m_MapCenter.InverseTransformPoint(point);
        return
            localPoint.x > m_LocalBotLeft.x && localPoint.x < m_LocalTopRight.x &&
            localPoint.z > m_LocalBotLeft.z && localPoint.z < m_LocalTopRight.z;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_MapCenter.position + m_LocalBotLeft, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(m_MapCenter.position + m_LocalTopRight, 5);;

        double lng, lat;
        GetCoordinates(m_MapCenter.position, out lng, out lat);
        drawString("lng:" + lng + " lat:" + lat, m_MapCenter.position);
    }

    public static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
#endif
    }

    [ContextMenu("enable buildings")]
    private void Debug_EnableBuildings()
    {
        EnableBuildings(true);
    }
    [ContextMenu("disable buildings")]
    private void Debug_DisableBuildings()
    {
        EnableBuildings(false);
    }
}
