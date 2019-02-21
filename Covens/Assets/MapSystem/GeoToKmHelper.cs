using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoToKmHelper : MonoBehaviour {

    [SerializeField] private Transform m_ReferencePoint;
    [SerializeField] private double m_Distance_x;
    [SerializeField] private double m_Distance_y;
    [SerializeField] private double m_DistanceD;
    //[SerializeField] private AbstractMap m_Map;

    //private const float m_LatDegToKm = 110.57f;
    //private const float m_LngDegToKm = 111.32f;

    private StreetMapWrapper m_Map;

    private void Awake()
    {
        m_Map = FindObjectOfType<StreetMapWrapper>();
    }

    private void Update()
    {
        if (m_ReferencePoint == null)
            return;

        Debug.DrawLine(transform.position, m_ReferencePoint.position);

        Vector2 center = m_Map.WorldToCoords(transform.position);
        Vector2 target = m_Map.WorldToCoords(m_ReferencePoint.position);

        Vector2 dist = MapsAPI.Instance.DistanceBetweenPoints(center, target);
        m_Distance_x = dist.x;
        m_Distance_y = dist.y;

        m_DistanceD = MapsAPI.Instance.DistanceBetweenPointsD(center, target);
    }
}
