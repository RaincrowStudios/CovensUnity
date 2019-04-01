using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererBasedDome : MonoBehaviour
{
    [SerializeField, HideInInspector] private LineRenderer m_LineRenderer;
    [SerializeField] private int m_Points = 20;
    [SerializeField] private float m_Radius = 1275;

    private void OnValidate()
    {
        if (m_LineRenderer == null)
            m_LineRenderer = GetComponent<LineRenderer>();
    }


    [ContextMenu("Setup dome")]
    public void SetupDome()
    {
        float spacing = (360f / m_Points) * Mathf.Deg2Rad;
        Vector3[] points = new Vector3[m_Points];

        float angle = 0;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].x = (m_Radius * Mathf.Cos(angle));
            points[i].y = 30;
            points[i].z = (m_Radius * Mathf.Sin(angle));
            angle += spacing;
        }

        m_LineRenderer.positionCount = points.Length;
        m_LineRenderer.SetPositions(points);
    }
}
