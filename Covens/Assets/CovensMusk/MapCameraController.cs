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

    [Header("Transition")]
    [SerializeField] private float m_TransitionTime = 1f;
    [SerializeField] private LeanTweenType m_TweenType;
    [SerializeField] private float m_CameraRot = 25f;
    [SerializeField] private float m_MinVig = .9f;
    [SerializeField] private float m_MaxVig = .55f;
    [SerializeField] private float m_MinVSoft = .2f;
    [SerializeField] private float m_MaxVSoft = .44f;
    [SerializeField] private float m_FXTimeIn = .5f;
    [SerializeField] private float m_FXTimeOut = 1f;

    [Header("FlyOut")]
    [SerializeField] private float m_FlyOutTime = 1f;
    [SerializeField] private AnimationCurve m_FlyOutCurve;



    public static float screenAdjust { get { return 720f / Screen.height; } }
    //public static MapCameraController Instance { get; private set; }

    private Vector2 m_LastDragPosition;
    private LeanFinger m_LastDragFinger;
    private System.Action m_OnUserPan;
    private System.Action m_OnUserPinch;
    private System.Action m_OnUserTwist;

    public new Camera camera { get { return m_Camera; } }

    public bool controlEnabled { get; private set; }
    public bool zoomEnabled { get; private set; }
    public bool panEnabled { get; private set; }
    public bool twistEnabled { get; private set; }

    public float maxFOV { get { return m_MaxFOV; } }
    public float minFOV { get { return m_MinFOV; } }
    public float minAngle { get { return m_MinAngle; } }
    public float maxAngle { get { return m_MaxAngle; } }
    public float fov { get { return m_Camera.fieldOfView; } }

    public float streetLevelNormalizedZoom { get; private set; }

    public Transform CenterPoint { get { return m_CenterPoint; } }
    public Transform RotationPivot { get { return m_AnglePivot; } }
    public float maxDistanceFromCenter { get { return m_MaxDistanceFromCenter; } }

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
        //Instance = this;
        LeanTouch.OnFingerUp += OnFingerUp;
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

    public void OnLandZoomIn(Material material)
    {
        EnableControl(false);

        LeanTween.value(0, .4f, m_FXTimeIn).setEase(m_TweenType).setOnUpdate((float v) =>
          {
              material.SetFloat("_LuminosityAmount", v);
          }).setOnComplete(() =>
          {
              LeanTween.value(.4f, 0, m_FXTimeOut).setEase(m_TweenType).setOnUpdate((float v) =>
               {
                   material.SetFloat("_LuminosityAmount", v);
               });
          });

        LeanTween.value(m_MinVig, m_MaxVig, m_FXTimeIn).setEase(m_TweenType).setOnUpdate((float v) =>
        {
            material.SetFloat("_VRadius", v);
        }).setOnComplete(() =>
        {
            LeanTween.value(m_MaxVig, m_MinVig, m_FXTimeOut).setEase(m_TweenType).setOnUpdate((float v) =>
           {
               material.SetFloat("_VRadius", v);
           });
        });

        LeanTween.value(m_MinVSoft, m_MaxVSoft, m_FXTimeIn).setEase(m_TweenType).setOnUpdate((float v) =>
       {
           material.SetFloat("_VSoft", v);
       }).setOnComplete(() =>
       {
           LeanTween.value(m_MaxVSoft, m_MinVSoft, m_FXTimeOut).setEase(m_TweenType).setOnUpdate((float v) =>
           {
               material.SetFloat("_VSoft", v);
           });
       });

        LeanTween.value(m_Camera.transform.localPosition.z, -800, m_TransitionTime).setOnUpdate((float v) =>
        {
            m_Camera.transform.localPosition = new Vector3(0, 0, v);
            onChangeRotation?.Invoke();
            onChangeZoom?.Invoke();
            onUpdate?.Invoke(false, true, true);
        }).setEase(m_TweenType).setOnComplete(() =>
        {
            EnableControl(true);
        });

        LeanTween.value(90, 25, m_TransitionTime).setOnUpdate((float v) =>
        {
            m_AnglePivot.localEulerAngles = new Vector3(v, 0, 0);
        }).setEase(m_TweenType);

        LeanTween.value(30, 13, m_TransitionTime).setOnUpdate((float v) =>
        {
            m_Camera.fieldOfView = v;
        }).setEase(m_TweenType);

        LeanTween.value(0, m_CameraRot, m_TransitionTime).setOnUpdate((float v) =>
           {
               m_CenterPoint.localEulerAngles = new Vector3(0, v, 0);
               m_CurrentTwist = v;
               m_TargetTwist = v;
           }).setEase(m_TweenType);
        LeanTween.value(0, 1, m_TransitionTime).setOnUpdate((float v) =>
        {
            streetLevelNormalizedZoom = v;
        }).setEase(m_TweenType);
        LeanTween.value(m_MuskMapWrapper.normalizedZoom, 1, m_TransitionTime).setOnUpdate((float v) =>
      {
          m_MuskMapWrapper.SetZoom(v);
      }).setEase(m_TweenType);
    }

    public void OnFlyButton(System.Action onComplete)
    {
        // if (m_MuskMapWrapper.normalizedZoom < .5f)
        //     return;
        EnableControl(false);
        Debug.Log("Flyinggg");
        LeanTween.value(m_MuskMapWrapper.normalizedZoom, .5f, m_FlyOutTime).setOnUpdate((float v) =>
        {
            m_MuskMapWrapper.SetZoom(v);
            onChangeZoom?.Invoke();
            onUpdate?.Invoke(false, true, false);
            m_MuskMapWrapper.refreshMap = true;
        }).setEase(m_FlyOutCurve).setOnComplete(() => { controlEnabled = true; onComplete(); });
    }

    public void OnLandButton(bool getMarkers = false)
    {
        if (m_StreetLevel)
            return;
        EnableControl(false);
        Debug.Log("Landing");
        LeanTween.value(m_MuskMapWrapper.normalizedZoom, .91f, m_FlyOutTime).setOnUpdate((float v) =>
        {
            m_MuskMapWrapper.SetZoom(v);
            onChangeZoom?.Invoke();
            onUpdate?.Invoke(false, true, false);
            m_MuskMapWrapper.refreshMap = true;
        }).setEase(m_FlyOutCurve).setOnComplete(() =>
        {
            controlEnabled = true;
            if (getMarkers)
            {
                MarkerManagerAPI.GetMarkers(true, true, () => { },
                true,
                false);
            }
        });
    }

    private void Update()
    {
        m_PositionChanged = m_ZoomChanged = m_RotationChanged = false;

        if (controlEnabled)
        {
            HandlePan();
            HandlePinch();
            HandleTwist();
        }

        streetLevelNormalizedZoom = Mathf.Clamp((1 - m_MuskMapWrapper.normalizedZoom) / 0.1f, 0, 1);
        bool streetLevel = m_MuskMapWrapper.streetLevel;

        if (m_StreetLevel != streetLevel)
        {
            m_StreetLevel = streetLevel;

            if (streetLevel)
            {
                m_PositionDelta = Vector3.zero;
                m_Camera.farClipPlane = 10000;

                double lng, lat;
                m_MuskMapWrapper.GetCoordinates(out lng, out lat);
                m_MuskMapWrapper.SetPosition(lng, lat);
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
        if (m_PositionDelta.magnitude > 1)
        {
            Vector3 newDelta = Vector3.Lerp(m_PositionDelta, Vector3.zero, Time.deltaTime * 10 / m_DragInertia);
            m_CenterPoint.position = ClampPosition(m_CenterPoint.position + (m_PositionDelta - newDelta));
            m_PositionDelta = newDelta;
            m_PositionChanged = true;
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
        {
            m_MuskMapWrapper.refreshMap = true;
            onUpdate?.Invoke(m_PositionChanged, m_ZoomChanged, m_RotationChanged);
        }
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

            Vector2 delta = (screenPoint - m_LastDragPosition)
                * m_DragSensivity
                * (720f / Screen.height)
                * (fov / m_MinFOV)
                * m_DragInertia
                * (Mathf.Abs(m_Camera.transform.localPosition.z) / m_DistanceFromGround)
                * m_MuskMapWrapper.cameraDat.panSensivity;

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

        Vector2 delta = (screenPoint - lastScreenPoint)
            * m_DragSensivity
            * (720f / Screen.height)
            * (fov / m_MinFOV)
            * (Mathf.Abs(m_Camera.transform.localPosition.z) / m_DistanceFromGround)
            * m_MuskMapWrapper.cameraDat.panSensivity;

        if (delta.magnitude > 0)
        {
            m_PositionDelta = Vector3.zero;
            Vector3 worldDelta = -m_CenterPoint.forward * delta.y * (m_MaxAngle / m_AnglePivot.eulerAngles.x) - m_CenterPoint.right * delta.x;
            m_CenterPoint.position = ClampPosition(m_CenterPoint.position + worldDelta);

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
            m_OnUserPinch?.Invoke();
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

        float newValue = m_TargetTwist + LeanGesture.GetTwistDegrees(fingers) * m_RotateSensivity;
        if (m_TargetTwist != newValue)
        {
            m_TargetTwist = newValue;
            m_OnUserTwist?.Invoke();
        }
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        if (m_StreetLevel)
        {
            Vector3 dir = (position - m_MuskMapWrapper.transform.position);
            if (dir.magnitude > m_MaxDistanceFromCenter)
                position = m_MuskMapWrapper.transform.position + dir.normalized * m_MaxDistanceFromCenter;
        }
        else
        {
            double lng, lat;
            m_MuskMapWrapper.GetCoordinates(out lng, out lat);
            Rect bounds = new Rect(position.x - 1, position.z - 1, position.x + 1, position.z + 1);

            if (lng < -150)
                bounds.x = m_MuskMapWrapper.topLeftBorder.x;
            else if (lng > 150)
                bounds.width = m_MuskMapWrapper.botRightBorder.x;
            if (lat < -60)
                bounds.y = m_MuskMapWrapper.botRightBorder.z;
            else if (lat > 60)
                bounds.height = m_MuskMapWrapper.topLeftBorder.z;

            //Debug.Log(lng + ", " + lat);

            position.x = Mathf.Clamp(position.x, bounds.x, bounds.width);
            position.z = Mathf.Clamp(position.z, bounds.y, bounds.height);
        }

        return position;
    }

    public void EnableControl(bool enable)
    {
        if (enable)
        {

        }
        else
        {
            m_PositionDelta = Vector3.zero; //stops inertia
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

    private int m_MoveTweenId;
    public void AnimatePosition(Vector3 pos, float time, bool allowCancel)
    {
        LeanTween.cancel(m_MoveTweenId, true);

        System.Action cancelAction = () => { };
        if (allowCancel)
            cancelAction = () => LeanTween.cancel(m_MoveTweenId, true);
        m_OnUserPan += cancelAction;

        m_MoveTweenId = LeanTween.move(m_CenterPoint.gameObject, pos, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                onChangePosition?.Invoke();
                onUpdate?.Invoke(true, false, false);
                m_MuskMapWrapper.refreshMap = true;
            })
            .setOnComplete(() =>
            {
                m_OnUserPan -= cancelAction;
            })
            .uniqueId;
    }

    private int m_ZoomTweenId;
    public void AnimateZoom(float normalizedZoom, float time, bool allowCancel)
    {
        LeanTween.cancel(m_ZoomTweenId, true);

        System.Action cancelAction = () => { };
        if (allowCancel)
            cancelAction = () => LeanTween.cancel(m_ZoomTweenId, true);
        m_OnUserPinch += cancelAction;

        m_ZoomTweenId = LeanTween.value(this.m_MuskMapWrapper.normalizedZoom, normalizedZoom, time)
           .setEaseOutCubic()
           .setOnUpdate((float t) =>
           {
               m_MuskMapWrapper.SetZoom(t);
               onChangeZoom?.Invoke();
               onUpdate?.Invoke(false, true, false);
           })
           .setOnComplete(() =>
           {
               m_OnUserPinch -= cancelAction;
           })
           .uniqueId;
    }

    private int m_TwistTweenId;
    public void AnimateRotation(float targetAngle, float time, bool allowCancel, System.Action onComplete)
    {
        LeanTween.cancel(m_TwistTweenId, true);

        System.Action cancelAction = () => { };
        if (allowCancel) cancelAction = () => LeanTween.cancel(m_TwistTweenId, true);
        m_OnUserTwist += cancelAction;

        Quaternion currentRotation;
        Quaternion startRotation = m_CenterPoint.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
        float lastAngle = m_CenterPoint.eulerAngles.y;
        float deltaAngle;

        m_TwistTweenId = LeanTween.value(0, 1, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                //update actual object rotation
                currentRotation = Quaternion.Lerp(startRotation, targetRotation, t);
                m_CenterPoint.rotation = currentRotation;

                //update stored angle
                deltaAngle = m_CenterPoint.eulerAngles.y - lastAngle;
                m_CurrentTwist = m_TargetTwist = m_TargetTwist + deltaAngle;
                lastAngle = m_CenterPoint.eulerAngles.y;

                //trigger events
                onChangeRotation?.Invoke();
                onUpdate?.Invoke(false, false, true); ;
            })
            .setOnComplete(() =>
            {
                m_OnUserTwist -= cancelAction;
                onComplete?.Invoke();
            })
            .uniqueId;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_StreetLevel)
        {
            Gizmos.DrawWireSphere(m_MuskMapWrapper.transform.position, m_MaxDistanceFromCenter);
        }
    }

    [Header("debug")]
    [SerializeField] private float m_Debug_TargetZoom = 1;
    [ContextMenu("animate zoom")]
    private void Debug_Zoom()
    {
        AnimateZoom(m_Debug_TargetZoom, 1f, false);
    }

    [ContextMenu("Toggle control")]
    private void ToggleControl()
    {
        EnableControl(!controlEnabled);
    }
}
