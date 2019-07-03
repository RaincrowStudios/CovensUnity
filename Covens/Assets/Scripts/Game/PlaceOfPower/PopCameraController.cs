using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopCameraController : MonoBehaviour
    {
        [SerializeField] private Camera m_Camera;
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


        public float fov { get { return m_Camera.fieldOfView; } }

        public bool controlEnabled { get; private set; }
        public bool zoomEnabled { get; private set; }
        public bool panEnabled { get; set; }
        public bool twistEnabled { get; private set; }
        
        /// <summary>
        /// position, zoom, rotation
        /// </summary>
        public event System.Action<bool, bool, bool> onUpdate;
        public event System.Action onUserPan;
        public event System.Action onUserPinch;
        public event System.Action onUserTwist;

        private Vector2 m_LastDragPosition;
        private LeanFinger m_LastDragFinger;

        private Vector3 m_PositionDelta;
        private float m_CurrentZoom;
        private float m_TargetZoom = 1f;
        private float m_CurrentTwist;
        private float m_TargetTwist;

        private bool m_PositionChanged;
        private bool m_ZoomChanged;
        private bool m_RotationChanged;

        private Vector3 m_CenterPosition;
        private float m_BoundRadius;

        private void Awake()
        {
            //Instance = this;
            LeanTouch.OnFingerUp += OnFingerUp;
            controlEnabled = true;
            panEnabled = true;
            zoomEnabled = true;
            twistEnabled = true;
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
                    * m_DragInertia;

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
                * (fov / m_MinFOV);

            if (delta.magnitude > 1)
            {
                m_PositionDelta = Vector3.zero;
                Vector3 worldDelta = -m_CenterPoint.forward * delta.y * (m_MaxAngle / m_AnglePivot.eulerAngles.x) - m_CenterPoint.right * delta.x;
                m_CenterPoint.position = ClampPosition(m_CenterPoint.position + worldDelta);

                onUserPan?.Invoke();
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
            float zoomAmount = (m_TargetZoom * pinchScale - m_TargetZoom) * m_ZoomSensivity;
            float zoom = Mathf.Clamp(m_TargetZoom + zoomAmount, 0.05f, 1);

            if (zoom != m_TargetZoom)
            {
                m_TargetZoom = zoom;
                onUserPinch?.Invoke();
            }
        }

        private void HandleTwist()
        {
            if (!twistEnabled)
                return;

            var fingers = LeanSelectable.GetFingers(true, true, 2);

            if (fingers.Count != 2)
                return;
            
            float newValue = m_TargetTwist + LeanGesture.GetTwistDegrees(fingers) * m_RotateSensivity;
            if (m_TargetTwist != newValue)
            {
                m_TargetTwist = newValue;
                onUserTwist?.Invoke();
            }
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            if (Vector3.Distance(m_CenterPosition, position) > m_BoundRadius)
                position = m_CenterPosition + (position - m_CenterPosition).normalized * m_BoundRadius;

            return position;
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

            //position innertia
            if (m_PositionDelta.magnitude > 1)
            {
                Vector3 newDelta = Vector3.Lerp(m_PositionDelta, Vector3.zero, Time.deltaTime * 10 / m_DragInertia);
                m_CenterPoint.position = ClampPosition(m_CenterPoint.position + (m_PositionDelta - newDelta));
                m_PositionDelta = newDelta;
                m_PositionChanged = true;
            }

            //zoom
            if (m_TargetZoom != m_CurrentZoom)
            {
                m_CurrentZoom = Mathf.Approximately(m_TargetZoom, m_CurrentZoom) ? m_TargetZoom : Mathf.Lerp(m_CurrentZoom, m_TargetZoom, Time.deltaTime * 5);
                m_ZoomChanged = true;
            }

            //rotation
            if (m_CurrentTwist != m_TargetTwist)
            {
                m_CurrentTwist = Mathf.Approximately(m_CurrentTwist, m_TargetTwist) ? m_TargetTwist : Mathf.Lerp(m_CurrentTwist, m_TargetTwist, Time.deltaTime * 5);
                m_CenterPoint.eulerAngles = new Vector3(0, m_CurrentTwist, 0);
                m_RotationChanged = true;
            }

            m_Camera.fieldOfView = Mathf.Lerp(m_MinFOV, m_MaxFOV, m_CurrentZoom);
            m_AnglePivot.localEulerAngles = new Vector3(Mathf.Lerp(m_MinAngle, m_MaxAngle, m_CurrentZoom), 0, 0);


            //if (m_PositionChanged)
            //    onChangePosition?.Invoke();
            //if (m_ZoomChanged)
            //    onChangeZoom?.Invoke();
            //if (m_RotationChanged)
            //    onChangeRotation?.Invoke();

            if (m_PositionChanged || m_ZoomChanged || m_RotationChanged)
            {
                onUpdate?.Invoke(m_PositionChanged, m_ZoomChanged, m_RotationChanged);
            }
        }


        public void SetCameraBounds(Vector3 center, float radius)
        {
            m_CenterPosition = center;
            m_BoundRadius = radius;
        }
    }
}
