using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraUtils : MonoBehaviour
{
    private static MapCameraUtils m_Instance;

    [SerializeField] private MapCameraController m_Controller;
    [SerializeField] private CovensMuskMap m_Map;

    [Header("Settings")]
    [SerializeField] private Vector2 m_TargetFocusOffset = new Vector2(19.1266f, 19.5f);
    
    private int m_HightlighTweenId;
    public static int markersLayer;
    public static int highlightsLayer;

    private void Awake()
    {
        m_Instance = this;

        markersLayer = LayerMask.NameToLayer("MapMarkers");
        highlightsLayer = LayerMask.NameToLayer("HighlightMarker");
    }

    public static void FocusOnPosition(Vector3 worldPosition, float zoom, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
        m_Instance.m_Controller.AnimateZoom(zoom, time, allowCancel);
    }

    public static void FocusOnTarget(IMarker marker, float time = 1f)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.right * m_Instance.m_TargetFocusOffset.x + m_Instance.m_Controller.CenterPoint.forward * m_Instance.m_TargetFocusOffset.y,
            1,
            false,
            time
        );

        marker.EnableAvatar();
    }

    public static void FocusOnTargetCenter(IMarker marker, float time = 1)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.forward * 10.5f,
            1f,
            false,
            time
        );
    }

    private static int m_ShakeTweenId;
    private static int m_ResetTweenId;

    public static void ShakeCamera(Vector3 axis, float amount, float periodTime, float duration)
    {
        StopCameraShake();
                
        LTDescr shake = LeanTween.rotateAroundLocal(
            m_Instance.m_Controller.camera.gameObject,
            axis,
            amount,
            periodTime)
        .setEase(LeanTweenType.easeShake)
        .setLoopClamp()
        .setRepeat(-1);

        m_ShakeTweenId = LeanTween.value(m_Instance.m_Controller.camera.gameObject, amount, 0, duration)
            .setOnUpdate((float t) =>
            {
                shake.setTo(axis * t);
            })
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.cancel(shake.uniqueId);
                m_ResetTweenId = LeanTween.rotateLocal(m_Instance.m_Controller.camera.gameObject, Vector3.zero, 1f).setEaseOutCubic().uniqueId;
            })
            .uniqueId;
    }

    public static void StopCameraShake()
    {
        LeanTween.cancel(m_ResetTweenId);
        LeanTween.cancel(m_ShakeTweenId, true);
    }
    
    public static void SetLayer(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
            SetLayer(child, layer);
    }

    public static void SetRotation(float eulerAngle, float  time, bool allowCancel, System.Action onComplete)
    {
        m_Instance.m_Controller.AnimateRotation(eulerAngle, time, allowCancel, onComplete);
    }

    public static void SetPosition(Vector3 worldPosition, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
    }
}
