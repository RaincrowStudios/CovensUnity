using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapboxBuildingScaler : MonoBehaviour
{
    [SerializeField] private Transform m_BuildingsParent;
    [SerializeField] private MapCameraController m_MapController;

    private float m_Zoom;

    private void Update()
    {
        if (m_MapController.normalizedZoom != m_Zoom)
        {
            m_Zoom = m_MapController.normalizedZoom;
            m_BuildingsParent.localScale = new Vector3(
                m_BuildingsParent.localScale.x,
                m_BuildingsParent.localScale.x * LeanTween.easeOutQuint(0, 1, 1 - m_Zoom) + 0.001f,
                m_BuildingsParent.localScale.z
            );
        }
    }
}
