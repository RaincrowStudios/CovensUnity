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
    //public static int markersLayer;
    //public static int highlightsLayer;

    private void Awake()
    {
        m_Instance = this;

        //markersLayer = LayerMask.NameToLayer("MapMarkers");
        //highlightsLayer = LayerMask.NameToLayer("HighlightMarker");
    }

    public static void FocusOnPosition(Vector3 worldPosition, float normalizedZoom, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
        m_Instance.m_Controller.AnimateZoom(normalizedZoom, time, allowCancel);
    }

    public static void FocusOnPosition(Vector3 worldPosition, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
    }

    public static void FocusOnPosition(float longitude, float latitude, bool allowCancel, float time = 1f)
    {
        m_Instance.m_Controller.AnimatePosition(new Vector2(longitude, latitude), time, allowCancel);
    }

    public static void FocusOnMarker(Vector3 position, float time = 1f)
    {
        FocusOnPosition(
            position + m_Instance.m_Controller.CenterPoint.right * m_Instance.m_TargetFocusOffset.x + m_Instance.m_Controller.CenterPoint.forward * m_Instance.m_TargetFocusOffset.y,
            1,
            false,
            time
        );
    }

    public static void FocusOnTargetCenter(IMarker marker, float time = 1)
    {
        if (marker == null)
            return;

        if (marker.GameObject == null)
            return;

        FocusOnPosition(
            marker.GameObject.transform.position + m_Instance.m_Controller.CenterPoint.forward * 10.5f,
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
        LeanTween.cancel(m_ResetTweenId);
        LeanTween.cancel(m_ShakeTweenId);

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

    public static void SetRotation(float eulerAngle, float time, bool allowCancel, System.Action onComplete)
    {
        m_Instance.m_Controller.AnimateRotation(eulerAngle, time, allowCancel, onComplete);
    }


    /// <summary>
    /// Use this for short (street level) distances
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="time"></param>
    /// <param name="allowCancel"></param>
    public static void SetPosition(Vector3 worldPosition, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimatePosition(worldPosition, time, allowCancel);
    }

    /// <summary>
    /// Use this for long (world level) distances.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="time"></param>
    /// <param name="allowCancel"></param>
    public static void SetPosition(Vector2 coordinates, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimatePosition(coordinates, time, allowCancel);
    }

    public static void SetZoom(float normalizedZoom, float time, bool allowCancel)
    {
        m_Instance.m_Controller.AnimateZoom(normalizedZoom, time, allowCancel);
    }

    public static void SetCameraRotation(Vector3 euler, float time, System.Action onComplete)
    {
        m_Instance.m_Controller.AnimateCamRotation(euler, time, onComplete);
    }

    public static void SetExtraFOV(float value)
    {
        m_Instance.m_Controller.ExtraFOV = value;
    }

    // public static void POPEnterAnimation()
    // {
    //     m_Instance.m_Controller.PlaceOfPowerEnter();
    // }

    // public static void POPExitAnimation()
    // {
    //     m_Instance.m_Controller.PlaceOfPowerExit();
    // }
}
