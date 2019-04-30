using System;
using Raincrow.Maps;
using UnityEngine;

public class MapFlightTransition : MonoBehaviour
{
    public static MapFlightTransition Instance { get; set; }
    private MapCameraController m_CameraControl;
    private IMaps map;
    private Material m_Material;
    [SerializeField] private RadialBlur m_RadialBlur;
    bool Canfly = true;
    void Awake()
    {
        Instance = this;
        m_CameraControl = GetComponent<MapCameraController>();
        m_Material = m_RadialBlur.material;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
        // m_CameraControl.onExitStreetLevel += TransitionOut;
    }



    void Start()
    {

    }
    // private void TransitionOut()
    // {

    // }
    public void FlyOut()
    {
        Debug.Log("fly");

        if (Canfly)
        {
            m_CameraControl.OnFlyButton();
            Canfly = false;
        }
        else if (!Canfly && !map.streetLevel)
        {
            m_CameraControl.OnLandButton();
            Canfly = true;
        }
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