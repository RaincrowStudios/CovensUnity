using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Lean.Touch;
using System;

[ExecuteInEditMode]
public class SpriteMapsController : MonoBehaviour
{
    private enum MapLODAssetBundle : byte
    {
        None = 0,
        MapState = 1,
        MapCountry = 2
    }

    public static SpriteMapsController instance { get; set; }
    [System.Serializable]
    private struct MapLOD
    {
        [SerializeField] private string name;
        public float minZoom;
        public float maxZoom;
        public GameObject render;
        public TextAsset labelsJson;
        public Label[] labels;
        public MapLODAssetBundle assetBundle;

        public Material[] materials;
    }
    public Action onChangePosition;
    public Action onChangeZoom;

    public static float screenAdjust { get { return 720f / Screen.height; } }

    [Header("Map")]
    [SerializeField] private float m_TileSize = 2.56f;
    [SerializeField] private int m_TileCount = 16;
    public float m_MapWidth { get { return m_TileSize * m_TileCount; } }
    public float m_MapHeight { get { return m_TileSize * m_TileCount; } }

    [SerializeField] private MapLOD[] m_LODs;
    [SerializeField] public Camera m_Camera;

    [Header("Tiles")]
    [SerializeField] Transform m_TileContainer;

    [Header("Labels")]
    [SerializeField] private TextMeshPro m_LabelPrefab;
    [SerializeField] private TextAsset m_GeoData;
    [SerializeField] private float m_LabelMinAlpha = 0;
    [SerializeField] private float m_LabelMaxAlpha = 1f;
    [SerializeField] private float m_LabelMinScale = 0.5f;
    [SerializeField] private float m_LabelMaxScale = 0.1f;

    [Header("Settings")]
    [SerializeField] public float m_MinZoom = 0.5f;
    [SerializeField] public float m_MaxZoom = 9f;
    [SerializeField] public float m_DragInertia = 5f;

    [Header("LeanTouch")]
    [SerializeField] private LeanScreenDepth m_ScreenDepth;
    [SerializeField] public float m_DragSensivity = 1f;
    [SerializeField] public float m_ZoomSensivity = 0.1f;

    private Vector3 m_LastDragPosition;
    private LeanFinger m_LastDragFinger;
    private bool m_Dragging = false;
    private int m_TweenId;
    private int m_CurrentLOD;
    public Color baseColor;
    public List<Material> mats = new List<Material>();
    public List<Color> matColors = new List<Color>();

    float prevScale;
    public Transform labelTransform;
    public TextAsset txtcityLabel;
    public TextAsset txtStateLabel;
    public List<Label> citylabels = new List<Label>();
    public List<Label> stateLabels = new List<Label>();

    [SerializeField] Camera fxCam;
    public static Vector2 mapCenter;
    bool hasRequested = false;


    public float normalizedZoom { get; private set; }

    public float zoom
    {
        get { return m_Camera.orthographicSize; }
        set
        {
            value = Mathf.Clamp(value, m_MinZoom, m_MaxZoom);
            if (value != m_Camera.orthographicSize)
            {
                m_Camera.orthographicSize = value;
                fxCam.orthographicSize = m_Camera.orthographicSize;

                normalizedZoom = (Mathf.Clamp(m_Camera.orthographicSize, m_MinZoom, m_MaxZoom) - m_MinZoom) / (m_MaxZoom - m_MinZoom);

                UpdateLabels();
                UpdateZoomColor();
                UpdateLOD();

                onChangeZoom?.Invoke();
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        for (int i = 0; i < m_LODs.Length; i++)
        {
            TextMeshPro label;
            Label labelData;
            createcountryLabels.Feature item;

            if (m_LODs[i].labelsJson != null)
            {
                Transform labelsParent = new GameObject("labels").transform;
                labelsParent.SetParent(m_LODs[i].render.transform);

                createcountryLabels.RootObject data = JsonConvert.DeserializeObject<createcountryLabels.RootObject>(m_LODs[i].labelsJson.text);
                m_LODs[i].labels = new Label[data.features.Count];

                for (int j = 0; j < m_LODs[i].labels.Length; j++)
                {
                    item = data.features[j];
                    label = Instantiate(m_LabelPrefab, Vector3.zero, Quaternion.identity);
                    label.text = data.features[j].properties.NAME;
                    label.color = new Color(0, 0, 0, 0);
                    label.transform.SetParent(labelsParent);
                    label.transform.position = GetWorldPosition(item.geometry.coordinates[0], item.geometry.coordinates[1]);
                    labelData = new Label
                    {
                        t = label,
                        initScale = MapUtils.scale(.2f, .3f, 0, 1, item.scale),
                        zoom = MapUtils.scale(5, 18, 1, 12, item.zoom)
                    };
                    m_LODs[i].labels[j] = labelData;
                }
            }
            else
            {
                m_LODs[i].labels = new Label[0];
            }

            if (m_LODs[i].assetBundle == MapLODAssetBundle.MapState)
            {
                int aux = i;
                DownloadedAssets.InstantiateStateMap(obj =>
                {
                    SetMaterial.SetMaterials(obj.GetComponentsInChildren<MeshRenderer>(), m_LODs[aux].materials);
                    obj.transform.SetParent(m_LODs[aux].render.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;
                    obj.gameObject.SetActive(true);
                });
            }
            else if (m_LODs[i].assetBundle == MapLODAssetBundle.MapCountry)
            {
                int aux = i;
                DownloadedAssets.InstantiateCountryMap(obj =>
                {
                    SetMaterial.SetMaterials(obj.GetComponentsInChildren<MeshRenderer>(), m_LODs[aux].materials);
                    obj.transform.SetParent(m_LODs[aux].render.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;
                    obj.gameObject.SetActive(true);
                });
            }
            else
            {
                SetMaterial.SetMaterials(m_LODs[i].render.GetComponentsInChildren<MeshRenderer>(), m_LODs[i].materials);
            }
        }

        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;

        UpdateLOD();
        UpdateZoomColor();
        UpdateLabels();
    }

    public Vector3 GetWorldPosition(float lng, float lat)
    {
        Vector3 localPos = GetLocalPosition(lng, lat);
        return m_TileContainer.position + localPos; ;

    }

    public Vector2 GetLocalPosition(float lng, float lat)
    {
        // get x value
        float x = (lng + 180) * (m_MapWidth / 360);
        // convert from degrees to radians
        float latRad = -lat * Mathf.Deg2Rad;
        // get y value
        float mercN = Mathf.Log(Mathf.Tan((Mathf.PI / 4) + (latRad / 2)));
        float y = (m_MapHeight / 2) - (m_MapWidth * mercN / (2 * Mathf.PI));

        return new Vector2(x, y); ;
    }

    public Vector2 GetCoordinatesFromWorldPosition(float x, float y)
    {
        Vector2 localPos = new Vector3(x, y, 0) - m_TileContainer.position;
        return GetCoordinatesFromLocalPosition(localPos.x, localPos.y);
    }

    public Vector2 GetCoordinatesFromLocalPosition(float x, float y)
    {
        x = Mathf.Repeat(x, m_MapWidth);

        return new Vector2
        {
            x = ((360 * x) / m_MapWidth) - 180,
            y = -90 * (-1 + (4 * Mathf.Atan(Mathf.Pow((float)System.Math.E, (Mathf.PI - (2 * Mathf.PI * y) / m_MapHeight)))) / Mathf.PI),
        };
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            HandlePan();
            HandleZoom();
        }
    }


    private void UpdateLabels()
    {
        float t = (m_Camera.orthographicSize - m_MinZoom) / (m_MaxZoom - m_MinZoom);

        float multiplier = MapUtils.scale(1, .05f, m_MaxZoom, m_MinZoom, m_Camera.orthographicSize);
        foreach (var item in m_LODs[m_CurrentLOD].labels)
        {
            if (m_Camera.orthographicSize <= (item.zoom + 2) && MapUtils.inMapView(item.t.transform.position, m_Camera))
            {
                var alpha = MapUtils.scale(.6f, 0, 3, item.zoom + 2, m_Camera.orthographicSize);
                Color color = new Color(1, 1, 1, alpha);
                item.t.gameObject.SetActive(true);
                item.t.transform.localScale = Vector3.one * item.initScale * multiplier * item.zoom;
                item.t.color = color;
            }
            else
            {
                item.t.gameObject.SetActive(false);
            }
        }

        var mapC = GetCoordinatesFromWorldPosition(m_Camera.transform.position.x, m_Camera.transform.position.y);
        mapCenter.x = mapC.y;
        mapCenter.y = mapC.x;

    }


    private void UpdateLOD()
    {
        if (m_Camera.orthographicSize < m_LODs[m_CurrentLOD].minZoom)
        {
            m_LODs[m_CurrentLOD].render.SetActive(false);
            m_CurrentLOD = Mathf.Max(m_CurrentLOD - 1, 0);
            m_LODs[m_CurrentLOD].render.SetActive(true);
            return;
        }

        if (m_Camera.orthographicSize > m_LODs[m_CurrentLOD].maxZoom)
        {
            m_LODs[m_CurrentLOD].render.SetActive(false);
            m_CurrentLOD = Mathf.Min(m_CurrentLOD + 1, m_LODs.Length - 1);
            m_LODs[m_CurrentLOD].render.SetActive(true);
            return;
        }
    }

    public Vector3 ClampCameraY(Vector3 position)
    {
        //clamp vertically
        float minY = m_TileContainer.position.y;// + m_Camera.orthographicSize;
        float maxY = m_TileContainer.position.y + m_MapHeight;// - m_Camera.orthographicSize;

        float minX = m_TileContainer.position.x;// + m_Camera.orthographicSize * Screen.width / Screen.height;
        float maxX = m_TileContainer.position.x + m_MapWidth;// - m_Camera.orthographicSize * Screen.width / Screen.height;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        position.z = m_Camera.transform.position.z;

        return position;
    }

    private void HandlePan()
    {
        var fingers = LeanSelectable.GetFingers(true, true, 1);
        if (fingers.Count != 1)
            return;

        m_LastDragFinger = fingers[0];
        var lastScreenPoint = m_LastDragPosition = LeanGesture.GetLastScreenCenter(fingers);
        var screenPoint = LeanGesture.GetScreenCenter(fingers);

        var worldDelta = m_ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);

        Vector3 pos = ClampCameraY(m_Camera.transform.position - worldDelta * m_DragSensivity);

        if (pos != m_Camera.transform.position)
        {
            m_Camera.transform.position = pos;
            UpdateLabels();
            onChangePosition?.Invoke();
        }
    }

    private void HandleZoom()
    {
        // Get the fingers we want to use
        var fingers = LeanSelectable.GetFingers(true, true, 2);

#if !UNITY_EDITOR
        if (fingers.Count != 2)
            return;
#else
        if (Input.GetAxis("Mouse ScrollWheel") == 0 && fingers.Count != 2)
            return;
#endif

        m_LastDragFinger = null;

        // Get the pinch ratio of these fingers
        var pinchRatio = LeanGesture.GetPinchRatio(fingers, m_ZoomSensivity);

#if UNITY_EDITOR
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            pinchRatio = LeanGesture.GetPinchRatio(fingers, -m_ZoomSensivity);
        }
#endif
        float zoom = m_Camera.orthographicSize * pinchRatio;


        // Perform the translation if this is a relative scale
        if (zoom > m_MinZoom && zoom < m_MaxZoom)
        {
            var pinchScreenCenter = LeanGesture.GetScreenCenter(fingers);
#if UNITY_EDITOR
            pinchScreenCenter = Input.mousePosition;
#endif
            var screenPosition = m_Camera.WorldToScreenPoint(m_Camera.transform.position);

            // Push the screen position away from the reference point based on the scale
            screenPosition.x = pinchScreenCenter.x + (screenPosition.x - pinchScreenCenter.x) * pinchRatio;
            screenPosition.y = pinchScreenCenter.y + (screenPosition.y - pinchScreenCenter.y) * pinchRatio;

            // Convert back to world space
            m_Camera.transform.position = ClampCameraY(m_Camera.ScreenToWorldPoint(screenPosition));
        }


        // Modify the zoom value
        this.zoom = zoom;
    }

    private void OnFingerDown(LeanFinger finger)
    {
        LeanTween.cancel(m_TweenId);
    }

    private void OnFingerUp(LeanFinger finger)
    {
#if !UNITY_EDITOR
        if (m_LastDragFinger != finger)
            return;
#endif

        var fingers = LeanSelectable.GetFingers(true, true);
        if (fingers.Count == 1)
        {
            var screenPoint = LeanGesture.GetScreenCenter(fingers);
            var delta = m_ScreenDepth.ConvertDelta(m_LastDragPosition, screenPoint, gameObject);

            Vector3 dir = delta * m_DragInertia * m_DragSensivity;
            Vector3 pos = ClampCameraY(m_Camera.transform.position - dir);
            //.setonupdate
            m_TweenId = LeanTween.move(m_Camera.gameObject, pos, 1f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    onChangePosition?.Invoke();
                })
                .uniqueId;
        }
    }

    public void UpdateZoomColor()
    {

        float clampScale = Mathf.Clamp(m_Camera.orthographicSize, 5, m_MaxZoom);
        float scaleZoom = MapUtils.scale(0, 1, m_MaxZoom, 5, clampScale);
        if (prevScale != scaleZoom)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                mats[i].color = Color.Lerp(baseColor, matColors[i], scaleZoom);

            }
            prevScale = scaleZoom;
        }
    }
}

public class Label
{
    public TextMeshPro t;
    public Transform k;
    public float initScale = 1;
    public float zoom;
    public Vector3 pos;
    public string name;
    public string id;
    public string type;
    public bool created = false;
}

public class stateLabel
{
    public string name { get; set; }
    public List<float> coordinates { get; set; }
}