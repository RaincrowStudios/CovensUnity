using Lean.Touch;
using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public static float screenAdjust { get { return 720f / Screen.height; } }

    [SerializeField] private Camera m_Camera;
    [SerializeField] private AbstractMap m_Map;

    [Header("Settings")]
    [SerializeField] private float m_MinZoom = 0.5f;
    [SerializeField] private float m_MaxZoom = 9f;
    [SerializeField] private float m_MinAngle = 30f;
    [SerializeField] private float m_MaxAngle = 50f;
    [SerializeField] private float m_DragInertia = 10f;

    [Header("LeanTouch")]
    [SerializeField] private LeanScreenDepth m_ScreenDepth;
    [SerializeField] private float m_DragSensivity = 1f;
    [SerializeField] private float m_ZoomSensivity = 0.1f;
    [SerializeField] private float m_RotateSensivity = 1f;

    /// <summary>
    /// X and Z axis movement
    /// and Y axis rotation
    /// </summary>
    [SerializeField] private Transform m_CenterPoint;

    /// <summary>
    /// X axis rotation
    /// </summary>
    [SerializeField] private Transform m_RotationPivot; 

    private Vector2 m_LastDragPosition;
    private LeanFinger m_LastDragFinger;
    private int m_TweenId;
    private int m_ZoomTweenId;
    private int m_MoveTweenId;
    private System.Action m_OnUserPan;
    private System.Action m_OnUserZoom;

    public new Camera camera { get { return m_Camera; } }
    public bool controlEnabled { get; private set; }
    public bool zoomEnabled { get; private set; }
    public bool panEnabled { get; private set; }
    public float maxZoom { get { return m_MaxZoom; } }
    public float minZoom { get { return m_MinZoom; } }
    public float zoom
    {
        get { return m_Camera.fieldOfView; }
        set
        {
            SetZoom(value, true);
        }
    }
    public float startingZoom { get; private set; }

    public Transform CenterPoint { get { return m_CenterPoint; } }
    public Transform RotationPivot { get { return m_RotationPivot; } }

    public System.Action onChangeZoom;
    public System.Action onChangePosition;

    private void Awake()
    {
        m_Map = FindObjectOfType<AbstractMap>();
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;
        controlEnabled = true;
        panEnabled = true;
        zoomEnabled = true;
        startingZoom = minZoom + (maxZoom - minZoom) * 0.2f;
    }

    private void Update()
    {
        if (!controlEnabled)
            return;

        HandlePan();
        HandleZoom();
        HandleRotate();
    }
    
    private void OnFingerDown(LeanFinger finger)
    {
        LeanTween.cancel(m_TweenId);
    }

    private void OnFingerUp(LeanFinger finger)
    {
        if (!controlEnabled)
            return;

        if (!panEnabled)
            return;

#if !UNITY_EDITOR
        if (m_LastDragFinger != finger)
            return;
#endif

        var fingers = LeanSelectable.GetFingers(true, true);
        if (fingers.Count == 1)
        {
            var screenPoint = LeanGesture.GetScreenCenter(fingers);

            Vector2 delta = (screenPoint - m_LastDragPosition) * m_DragSensivity * (720f / Screen.height) * (zoom / m_MinZoom) * m_DragInertia;
            Vector3 localPos =
                m_CenterPoint.localPosition
                - m_CenterPoint.forward * delta.y * (m_MaxAngle / m_RotationPivot.eulerAngles.x)
                - m_CenterPoint.right * delta.x;

            bool triggerChanged = localPos.magnitude > 5;

            m_TweenId = LeanTween.moveLocal(m_CenterPoint.gameObject, localPos, 1f)
                .setOnUpdate((float t) =>
                {
                    if (triggerChanged)
                        m_CenterPoint.hasChanged = true;
                    onChangePosition?.Invoke();
                })
                .setEaseOutCubic()
                .uniqueId;
        }
    }

    private void HandlePan()
    {
        if (!panEnabled)
            return;

        var fingers = LeanSelectable.GetFingers(true, true, 1);
        if (fingers.Count != 1)
            return;

        m_LastDragFinger = fingers[0];
        var lastScreenPoint = m_LastDragPosition = LeanGesture.GetLastScreenCenter(fingers);
        var screenPoint = LeanGesture.GetScreenCenter(fingers);
                
        Vector2 delta = (screenPoint - lastScreenPoint) * m_DragSensivity * (720f/Screen.height) * (zoom / m_MinZoom);
        Vector3 localPos = 
            m_CenterPoint.localPosition 
            - m_CenterPoint.forward * delta.y * (m_MaxAngle / m_RotationPivot.eulerAngles.x)
            - m_CenterPoint.right * delta.x;

        if (m_CenterPoint.localPosition != localPos)
        {
            m_CenterPoint.localPosition = localPos;
            m_CenterPoint.hasChanged = true;
            onChangePosition?.Invoke();
            m_OnUserPan?.Invoke();
        }
    }

    private void HandleZoom()
    {
        if (!zoomEnabled)
            return;

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
        float zoom = m_Camera.fieldOfView * pinchRatio;
        this.zoom = zoom;
        m_OnUserZoom?.Invoke();
    }

    private void HandleRotate()
    {
        var fingers = LeanSelectable.GetFingers(true, true, 2);

        if (fingers.Count != 2)
            return;

        float twist = LeanGesture.GetTwistDegrees(fingers);
        m_CenterPoint.Rotate(new Vector3(0, twist * m_RotateSensivity, 0), Space.World);
    }

    private Vector3 ClampCamera(Vector3 position)
    {
        return position;
    }

    public void EnableControl(bool enable)
    {
        if (enable)
        {

        }
        else
        {
            LeanTween.cancel(m_TweenId);
        }
        controlEnabled = enable;
    }

    public void SetZoom(float zoom, bool clamp, float time, bool allowCancel)
    {
        LeanTween.cancel(m_ZoomTweenId, true);

        bool previousValue = zoomEnabled;
        zoomEnabled = allowCancel;

        System.Action _action = () => { };
        if (allowCancel)
        {
            _action = () => LeanTween.cancel(m_ZoomTweenId, true);
            m_OnUserZoom += _action;
        }

        m_ZoomTweenId = LeanTween.value(m_Camera.fieldOfView, clamp ? Mathf.Clamp(zoom, m_MinZoom, m_MaxZoom) : zoom, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                SetZoom(t, clamp);
            })
            .setOnComplete(() => 
            {
                zoomEnabled = previousValue;
                m_OnUserZoom -= _action;
            })
            .uniqueId;
    }

    public void SetPosition(Vector3 pos, float time, bool allowCancel)
    {
        LeanTween.cancel(m_MoveTweenId, true);
        
        bool previousValue = panEnabled;
        panEnabled = allowCancel;

        System.Action _action = ()=> { };
        if (allowCancel)
            _action = () => LeanTween.cancel(m_MoveTweenId, true);
        m_OnUserPan += _action;

        m_MoveTweenId = LeanTween.move(m_CenterPoint.gameObject, pos, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CenterPoint.hasChanged = true;
                onChangePosition?.Invoke();
            })
            .setOnComplete(() =>
            {
                panEnabled = previousValue;
                m_OnUserPan -= _action;
            })
            .uniqueId;
    }

    public float normalizedZoom { get; private set; }

    public void SetZoom(float value, bool clamp)
    {
        if (value != m_Camera.fieldOfView)
        {
            if (clamp)
                m_Camera.fieldOfView = Mathf.Clamp(value, m_MinZoom, m_MaxZoom);
            else
                m_Camera.fieldOfView = value;

            normalizedZoom = (Mathf.Clamp(m_Camera.fieldOfView, m_MinZoom, m_MaxZoom) - m_MinZoom) / (m_MaxZoom - m_MinZoom);

            m_RotationPivot.localEulerAngles = new Vector3(Mathf.Lerp(m_MinAngle, m_MaxAngle, normalizedZoom), 0, 0);

            m_CenterPoint.hasChanged = true;
            onChangeZoom?.Invoke();
        }
    }
}
