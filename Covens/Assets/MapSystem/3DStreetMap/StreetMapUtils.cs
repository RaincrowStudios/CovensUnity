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

        Vector3 targetPosition = marker.instance.transform.position + diff;
        LeanTween.move(
            m_Instance.m_Controller.CenterPoint.gameObject, 
            targetPosition,
            0.5f
        ).setEaseOutCubic();
        LeanTween.value(
            m_Instance.m_Controller.zoom,
            m_Instance.m_Controller.minZoom + (m_Instance.m_Controller.maxZoom - m_Instance.m_Controller.minZoom) * zoom,
            0.5f
        )
        .setOnUpdate((float t) => { m_Instance.m_Controller.camera.fieldOfView = t; })
        .setEaseOutCubic();
    }
}
