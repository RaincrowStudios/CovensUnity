using System;
using UnityEngine;

public class MapFlightTransition : MonoBehaviour
{
    public static MapFlightTransition Instance { get; set; }
    private MapCameraController m_CameraControl;
    private Material m_Material;
    [SerializeField] private RadialBlur m_RadialBlur;
    void Awake()
    {
        Instance = this;
        m_CameraControl = GetComponent<MapCameraController>();
        m_Material = m_RadialBlur.material;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
        // m_CameraControl.onExitStreetLevel += TransitionOut;

    }



    // private void TransitionOut()
    // {

    // }
    public void FlyOut()
    {
        m_CameraControl.OnFlyButton();
    }
    void TransitionIn()
    {
        if (PlayerManagerUI.Instance == null)
            return;

        SoundManagerOneShot.Instance.PlayLandFX(.78f);
        PlayerManagerUI.Instance.LandFX.SetActive(true);
        m_CameraControl.OnLandZoomIn(m_Material);
    }
}