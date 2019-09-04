using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCenterPointer : MonoBehaviour
{
    [SerializeField] private Transform m_Pointer;
    [SerializeField] private SpriteRenderer m_PointerSprite;
    [SerializeField] private MapCameraController m_MapController;

    private void Start()
    {
        m_MapController.onUpdate += OnMapUpdate;
    }

    private void OnMapUpdate(bool position, bool zoom, bool rotation)
    {
        if (position || rotation)
        {
            m_Pointer.transform.LookAt(Vector3.zero);

            float distance = Vector3.Distance(m_Pointer.position, Vector3.zero);
            float maxDistance = m_MapController.MaxDistanceFromCenter;

            m_PointerSprite.color = new Color(1, 1, 1, distance / maxDistance);
        }
    }
}
