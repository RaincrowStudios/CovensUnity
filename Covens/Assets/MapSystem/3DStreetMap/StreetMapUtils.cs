using Mapbox.Unity.Map;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetMapUtils : MonoBehaviour
{
    private static StreetMapUtils m_Instance;

    [SerializeField] private AbstractMap m_Map;
    [SerializeField] private MapCameraController m_Controller;

    private void Awake()
    {
        m_Instance = this;
    }

    public static void FocusOnPosition(Vector3 worldPosition, bool clampZoom, float zoom, bool allowCancel)
    {
        m_Instance.m_Controller.SetPosition(worldPosition, 0.6f, !allowCancel);
        m_Instance.m_Controller.SetZoom(zoom, clampZoom, 0.6f, !allowCancel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="marker">target's marker</param>
    /// <param name="offset">offset in viewport position</param>
    /// <param name="zoom">zoom percentage (0 = full zoomed in, 1 = zoomed out)</param>
    public static void FocusOnTarget(IMarker marker, Vector2 offset, float zoom)
    {
        Vector3 offsetPosition = m_Instance.m_Controller.camera.ViewportToWorldPoint(new Vector3(
            offset.x, 
            offset.y, 
            Vector3.Distance(m_Instance.m_Controller.CenterPoint.position, m_Instance.m_Controller.camera.transform.position))
        );
        offsetPosition.y = m_Instance.m_Controller.CenterPoint.position.y;
        Vector3 diff = m_Instance.m_Controller.CenterPoint.position - offsetPosition;

        FocusOnPosition(marker.gameObject.transform.position + diff, false, zoom, false);
    }

    /// <summary>
    /// return the current worldPosition the camera is focused at
    /// </summary>
    public static Vector3 CurrentPosition()
    {
        return m_Instance.m_Controller.CenterPoint.position;
    }

    private static int m_ShakeTweenId;
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
            .uniqueId;
    }

    public static void StopCameraShake()
    {
        LeanTween.cancel(m_ShakeTweenId);
    }
}
