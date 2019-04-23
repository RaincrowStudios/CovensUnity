using System;
using UnityEngine;

public class MapFlightTransition : MonoBehaviour
{
    private MapCameraController m_CameraControl;
    private Material m_Material;
    [SerializeField] private RadialBlur m_RadialBlur;
    void Awake()
    {

        m_CameraControl = GetComponent<MapCameraController>();
        m_Material = m_RadialBlur.material;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
        m_CameraControl.onExitStreetLevel += TransitionOut;
    }

    private void TransitionOut()
    {

    }

    void TransitionIn()
    {
        PlayerManagerUI.Instance.LandFX.SetActive(true);
        m_CameraControl.OnLandZoomIn(m_Material);
    }
}