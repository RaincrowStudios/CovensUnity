using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFXTrail : MonoBehaviour
{
    [SerializeField] private SpriteMapsController m_Map;
    [SerializeField] private Camera m_FlyCam;

    //
    [SerializeField] private LeanScreenDepth m_ScreenDepth;
    private Vector3 m_LastDragPosition;
    private LeanFinger m_LastDragFinger;
    private int m_TweenId;

    private void OnEnable()
    {
        LeanTouch.OnFingerUp += OnFingerUp;
        LeanTouch.OnFingerDown += OnFingerDown;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUp -= OnFingerUp;
        LeanTouch.OnFingerDown -= OnFingerDown;
    }

    private void Update()
    {
        HandlePan();
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

        Vector3 pos = m_Map.ClampCameraY(transform.position - worldDelta * m_Map.m_DragSensivity);

        if (pos != transform.position)
        {
            transform.position = pos;
        }
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

            Vector3 dir = delta * m_Map.m_DragInertia * m_Map.m_DragSensivity;
            Vector3 pos = m_Map.ClampCameraY(transform.position - dir);

            m_TweenId = LeanTween.move(transform.gameObject, pos, 1f)
                .setEaseOutCubic()
                .uniqueId;
        }
    }
}
