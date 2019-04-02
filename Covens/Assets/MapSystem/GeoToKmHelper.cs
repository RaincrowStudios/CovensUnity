using Mapbox.Unity.Map;
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

    public static GeoToKmHelper Instance { get; private set; }

    private StreetMapWrapper m_Map;

    private void Awake()
    {
        Instance = this;
        m_Map = FindObjectOfType<StreetMapWrapper>();
    }

    private void OnDrawGizmos()
    {
        if (m_ReferencePoint == null)
            return;

        Gizmos.DrawLine(transform.position, m_ReferencePoint.position);
        
        Vector2 center = m_Map.WorldToCoords(transform.position);
        Vector2 target = m_Map.WorldToCoords(m_ReferencePoint.position);

        Vector2 dist = MapsAPI.Instance.DistanceBetweenPoints(center, target);
        m_Distance_x = dist.x;
        m_Distance_y = dist.y;

        m_DistanceD = MapsAPI.Instance.DistanceBetweenPointsD(center, target);
    }

    public static float OneKmInWorldspace
    {
        get
        {
            return Vector3.Distance(Instance.transform.position, Instance.m_OneKm.position);
        }
    }
}
