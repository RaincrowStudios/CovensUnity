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
    private bool m_Dragging = false;
    private int m_TweenId;
    private int m_ZoomTweenId;
    private int m_MoveTweenId;
    
    public new Camera camera { get { return m_Camera; } }
    public bool controlEnabled { get; private set; }
    public bool zoomEnabled { get; private set; }
    public float maxZoom { get { return m_MaxZoom; } }
    public float minZoom { get { return m_MinZoom; } }
    public float zoom
    {
        get { return m_Camera.fieldOfView; }
        set
        {
            if (value != m_Camera.fieldOfView)
            {
                m_Camera.fieldOfView = Mathf.Clamp(value, m_MinZoom, m_MaxZoom);

                float t = (m_Camera.fieldOfView - m_MinZoom) / (m_MaxZoom - m_MinZoom);
                m_RotationPivot.localEulerAngles = new Vector3(Mathf.Lerp(m_MinAngle, m_MaxAngle, t), 0, 0);

                m_CenterPoint.hasChanged = true;
                onChangeZoom?.Invoke();
            }
        }
    }

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

            m_TweenId = LeanTween.moveLocal(m_CenterPoint.gameObject, localPos, 1f)
                .setOnUpdate((float t) =>
                {
                    m_Camera.transform.hasChanged = true;
                    onChangePosition?.Invoke();
                })
                .setEaseOutCubic()
                .uniqueId;
        }
    }

    private void HandlePan()
    {
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
            m_Camera.transform.hasChanged = true;
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
        float zoom = m_Camera.fieldOfView * pinchRatio;
        this.zoom = zoom;
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
        if (allowCancel)
            onChangeZoom += _OnChangeZoom;

        LeanTween.cancel(m_ZoomTweenId);

        m_ZoomTweenId = LeanTween.value(m_Camera.fieldOfView, clamp ? Mathf.Clamp(zoom, m_MinZoom, m_MaxZoom) : zoom, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Camera.fieldOfView = t;
                m_CenterPoint.hasChanged = true;
            })
            .setOnComplete(() => 
            {
                onChangeZoom?.Invoke();
            })
            .uniqueId;
    }

    public void SetPosition(Vector3 pos, float time, bool allowCancel)
    {
        if (allowCancel)
            onChangePosition += _OnChangePosition;

        LeanTween.cancel(m_MoveTweenId);

        bool previousValue = controlEnabled;
        controlEnabled = false;

        m_MoveTweenId = LeanTween.move(m_CenterPoint.gameObject, pos, time)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                controlEnabled = previousValue;
            })
            .setOnUpdate((float t) =>
            {
                m_CenterPoint.hasChanged = true;
            })
            .setOnComplete(() =>
            {
                onChangePosition?.Invoke();
            })
            .uniqueId;
    }

    private void _OnChangeZoom()
    {
        LeanTween.cancel(m_ZoomTweenId);
        onChangeZoom -= _OnChangeZoom;
    }

    private void _OnChangePosition()
    {
        LeanTween.cancel(m_MoveTweenId);
        onChangePosition -= _OnChangePosition;
    }
}
