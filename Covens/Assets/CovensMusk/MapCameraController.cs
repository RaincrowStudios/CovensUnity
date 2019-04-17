using Lean.Touch;
//using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private CovensMuskMap m_MuskMapWrapper;
    [SerializeField] private Transform m_CenterPoint;
    [SerializeField] private Transform m_AnglePivot;

    [Header("Settings")]
    [SerializeField] private float m_MinFOV = 13f;
    [SerializeField] private float m_MaxFOV = 30f;
    [SerializeField] private float m_MinAngle = 30f;
    [SerializeField] private float m_MaxAngle = 50f;
    [SerializeField] private float m_DragInertia = 10f;

    [Header("LeanTouch")]
    [SerializeField] private LeanScreenDepth m_ScreenDepth;
    [SerializeField] private float m_DragSensivity = 1f;
    [SerializeField] private float m_ZoomSensivity = 0.1f;
    [SerializeField] private float m_RotateSensivity = 1f;
    [SerializeField] private float m_DistanceFromGround = 400f;


    public static float screenAdjust { get { return 720f / Screen.height; } }
    public static MapCameraController Instance { get; private set; }

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
    public bool twistEnabled { get; private set; }

    public float maxFOV { get { return m_MaxFOV; } }
    public float minFOV { get { return m_MinFOV; } }
    public float minAngle { get { return m_MinAngle; } }
    public float maxAngle { get { return m_MaxAngle; } }
    public float zoom { get { return m_Camera.fieldOfView; } }

    public float streetLevelNormalizedZoom { get; private set; }

    public Transform CenterPoint { get { return m_CenterPoint; } }
    public Transform RotationPivot { get { return m_AnglePivot; } }

    public System.Action onChangeZoom;
    public System.Action onChangePosition;
    public System.Action onChangeRotation;
    public System.Action<bool, bool, bool> onUpdate;
    public System.Action onEnterStreetLevel;
    public System.Action onExitStreetLevel;

    private bool m_PositionChanged;
    private bool m_ZoomChanged;
    private bool m_RotationChanged;

    private bool m_StreetLevel = false;

    private Vector3 m_PositionDelta;
    
    private float m_CurrentTwist;
    private float m_TargetTwist;

    private bool m_LerpZoom;
    private bool m_LerpAngle;
    private bool m_LerpTwist;

    private float m_MaxDistanceFromCenter = 1000;

    private void Awake()
    {
        Instance = this;
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;
        controlEnabled = true;
        panEnabled = true;
        zoomEnabled = true;
        twistEnabled = true;

        LoginAPIManager.OnCharacterInitialized += LoginAPIManager_OnCharacterInitialized;
    }

    private void LoginAPIManager_OnCharacterInitialized()
    {
       m_MaxDistanceFromCenter = PlayerDataManager.DisplayRadius * GeoToKmHelper.OneKmInWorldspace;
    }

    private void Update()
    {
        m_PositionChanged = m_ZoomChanged = m_RotationChanged = false;

        if (!controlEnabled)
            return;
        
        HandlePan();
        HandlePinch();
        HandleTwist();

        streetLevelNormalizedZoom = Mathf.Clamp((1 - m_MuskMapWrapper.normalizedZoom) / 0.1f, 0, 1);
        bool streetLevel = m_MuskMapWrapper.streetLevel;

        if (m_StreetLevel != streetLevel)
        {
            m_StreetLevel = streetLevel;

            if (streetLevel)
            {
                m_Camera.farClipPlane = 10000;
            }
            else
            {
                m_Camera.farClipPlane = 1e+15f;
                m_TargetTwist = 360 * Mathf.RoundToInt(m_TargetTwist / 360);
            }

            if (streetLevel)
                onEnterStreetLevel?.Invoke();
            else
                onExitStreetLevel?.Invoke();
        }

        //position innertia
        if (m_PositionDelta != Vector3.zero)
        {
            Vector3 newDelta = Vector3.Lerp(m_PositionDelta, Vector3.zero, Time.deltaTime);
            m_CenterPoint.position += (m_PositionDelta - newDelta);
            m_PositionDelta = newDelta;
            m_PositionChanged = true;
            m_MuskMapWrapper.refreshMap = true;
        }

        if (m_CurrentTwist != m_TargetTwist)
        {
            m_CurrentTwist = Mathf.Lerp(m_CurrentTwist, m_TargetTwist, Time.deltaTime * 10);
            m_CenterPoint.eulerAngles = new Vector3(0, m_CurrentTwist, 0);
            m_RotationChanged = true;
        }

        m_Camera.fieldOfView = Mathf.Lerp(m_MinFOV, m_MaxFOV, streetLevelNormalizedZoom);
        m_AnglePivot.localEulerAngles = new Vector3(Mathf.Lerp(m_MinAngle, m_MaxAngle, streetLevelNormalizedZoom), 0, 0);
        

        if (m_PositionChanged)
            onChangePosition?.Invoke();
        if (m_ZoomChanged)
            onChangeZoom?.Invoke();
        if (m_RotationChanged)
            onChangeRotation?.Invoke();

        if (m_PositionChanged || m_ZoomChanged || m_RotationChanged)
            onUpdate?.Invoke(m_PositionChanged, m_ZoomChanged, m_RotationChanged);
    }

    private void LateUpdate()
    {
        
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

            Vector2 delta = (screenPoint - m_LastDragPosition) * m_DragSensivity * (720f / Screen.height) * (zoom / m_MinFOV) * m_DragInertia * (Mathf.Abs(m_Camera.transform.localPosition.z) / m_DistanceFromGround);

            if (delta.magnitude > 10)
            {
                Vector3 worldDelta = -m_CenterPoint.forward * delta.y * (m_MaxAngle / m_AnglePivot.eulerAngles.x) - m_CenterPoint.right * delta.x;
                m_PositionDelta = worldDelta;
            }
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
                
        Vector2 delta = (screenPoint - lastScreenPoint) * m_DragSensivity * (720f/Screen.height) * (zoom / m_MinFOV) * (Mathf.Abs(m_Camera.transform.localPosition.z) / m_DistanceFromGround);

        if (delta.magnitude > 0)
        {
            m_PositionDelta = Vector3.zero;
            Vector3 worldDelta = -m_CenterPoint.forward * delta.y * (m_MaxAngle / m_AnglePivot.eulerAngles.x) - m_CenterPoint.right * delta.x;
            m_CenterPoint.position += worldDelta;
            m_PositionChanged = true;
            m_OnUserPan?.Invoke();
            m_MuskMapWrapper.refreshMap = true;
        }
    }

    private void HandlePinch()
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

        var pinchScale = LeanGesture.GetPinchScale(fingers, -0.2f);
        float zoomAmount = (m_MuskMapWrapper.normalizedZoom * pinchScale - m_MuskMapWrapper.normalizedZoom) * m_MuskMapWrapper.cameraDat.zoomSensivity * m_ZoomSensivity;
        float zoom = Mathf.Clamp(m_MuskMapWrapper.normalizedZoom + zoomAmount, 0.05f, 1);

        if (zoom != m_MuskMapWrapper.normalizedZoom)
        {
            m_MuskMapWrapper.SetZoom(zoom);
            m_ZoomChanged = true;
            m_OnUserZoom?.Invoke();
        }
    }

    private void HandleTwist()
    {
        if (!twistEnabled)
            return;

        var fingers = LeanSelectable.GetFingers(true, true, 2);

        if (fingers.Count != 2)
            return;

        if (!m_MuskMapWrapper.streetLevel)
            return;

        m_TargetTwist += LeanGesture.GetTwistDegrees(fingers) * m_RotateSensivity;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        //string debug =
        //    m_MuskMapWrapper.topLeftBorder.x + " : " + position.x + " : " + m_MuskMapWrapper.bottomRightBorder.x + "\n" +
        //    (position.x > m_MuskMapWrapper.topLeftBorder.x) + "&&" + (position.x < m_MuskMapWrapper.bottomRightBorder.x);
        //Debug.Log(debug);
        ////position.x = Mathf.Clamp(position.x,    m_MuskMapWrapper.topLeftBorder.x,       m_MuskMapWrapper.bottomRightBorder.x);
        ////position.z = Mathf.Clamp(position.z,    m_MuskMapWrapper.bottomRightBorder.z,   m_MuskMapWrapper.topLeftBorder.z);

        //position.z = Mathf.Clamp(position.z, m_MuskMapWrapper.bottomRightBorder.z, m_MuskMapWrapper.topLeftBorder.z);

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

    public void EnableZoom(bool enable)
    {
        zoomEnabled = enable;
    }

    public void EnableTwist(bool enable)
    {
        twistEnabled = enable;
    }

    //public void SetPosition(Vector3 pos, float time, bool allowCancel)
    //{
    //    LeanTween.cancel(m_MoveTweenId, true);
        
    //    System.Action _action = ()=> { };
    //    if (allowCancel)
    //        _action = () => LeanTween.cancel(m_MoveTweenId, true);
    //    m_OnUserPan += _action;

    //    m_MoveTweenId = LeanTween.move(m_CenterPoint.gameObject, pos, time)
    //        .setEaseOutCubic()
    //        .setOnUpdate((float t) =>
    //        {
    //            m_CenterPoint.hasChanged = true;
    //            onChangePosition?.Invoke();
    //        })
    //        .setOnComplete(() =>
    //        {
    //            m_OnUserPan -= _action;
    //        })
    //        .uniqueId;
    //}
}
