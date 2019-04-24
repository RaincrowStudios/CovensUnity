using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraUtils : MonoBehaviour
{
    private static MapCameraUtils m_Instance;

    [SerializeField] private MapCameraController m_Controller;
    [SerializeField] private CovensMuskMap m_Map;
    
    private int m_HightlighTweenId;
    public static int markersLayer;
    public static int highlightsLayer;

    private void Awake()
    {
        m_Instance = this;

        markersLayer = LayerMask.NameToLayer("MapMarkers");
        highlightsLayer = LayerMask.NameToLayer("HighlightMarker");
    }

    public static void FocusOnPosition(Vector3 worldPosition, bool clampZoom, float zoom, bool allowCancel)
    {
        //m_Instance.m_Controller.SetPosition(worldPosition, 0.6f, allowCancel);
        //m_Instance.m_Controller.SetZoom(zoom, clampZoom, 0.6f, allowCancel);
    }

    public static void FocusOnTarget(IMarker marker)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.right * 19.1266f + m_Instance.m_Controller.CenterPoint.forward * 19.5f,
            false,
            9,
            false
        );

        marker.EnableAvatar();
    }

    public static void FocusOnTargetCenter(IMarker marker)
    {
        if (marker == null)
            return;

        if (marker.gameObject == null)
            return;

        FocusOnPosition(
            marker.gameObject.transform.position + m_Instance.m_Controller.CenterPoint.forward * 10.5f,
            false,
            9,
            false
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
}
