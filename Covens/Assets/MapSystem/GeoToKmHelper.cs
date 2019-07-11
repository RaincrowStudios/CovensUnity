using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoToKmHelper : MonoBehaviour
{
    [Header("presets")]
    [SerializeField] private Transform m_OneKm;

    [Header("debug")]
    [SerializeField] private Transform m_ReferencePoint;
    [SerializeField] private double m_Distance_x;
    [SerializeField] private double m_Distance_y;
    [SerializeField] private double m_DistanceD;
        
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;

        if (m_ReferencePoint == null)
            return;

        Gizmos.DrawLine(transform.position, m_ReferencePoint.position);

        double lng, lat;
        Vector2 center, target;

        MapsAPI.Instance.GetPosition(out lng, out lat);
        center = new Vector2((float)lng, (float)lat);

        MapsAPI.Instance.GetPosition(m_ReferencePoint.position, out lng, out lat);
        target = new Vector2((float)lng, (float)lat);

        Vector2 dist = MapsAPI.Instance.DistanceBetweenPoints(center, target);
        m_Distance_x = dist.x;
        m_Distance_y = dist.y;

        m_DistanceD = MapsAPI.Instance.DistanceBetweenPointsD(center, target);
    }

    public float OneKmInWorldspace
    {
        get
        {
            return Vector3.Distance(transform.position, m_OneKm.position);
        }
    }
}
