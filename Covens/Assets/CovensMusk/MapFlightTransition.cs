using System;
using System.Collections;
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
    public CanvasGroup CG;

    void Awake()
    {
        Instance = this;
        m_CameraControl = GetComponent<MapCameraController>();
        m_Material = m_RadialBlur.materialInstance;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
    }


    public void RecallHome(bool force = false)
    {
        if (map == null)
        {
            map = MapsAPI.Instance;
        }

        CG.gameObject.SetActive(true);
        CG.alpha = 0;

        PlayerLandFX.PlayFlightAnim();

        bool wasAtStreetLevel = MapsAPI.Instance.streetLevel;

        LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        {
            map.SetPosition(GetGPS.longitude, GetGPS.latitude);

            m_CameraControl.AnimateZoom(1f, 0.3f, false, m_CameraControl.m_FlyOutCurve);

            //if the player was at street level, PlayerManager.OnLand will not trigger
            if (wasAtStreetLevel)
                MarkerManagerAPI.GetMarkers(true, false, PlayerLandFX.PlayLandingAnim, false, false);
            else
                PlayerLandFX.PlayLandingAnim();

            LeanTween.alphaCanvas(CG, 0, .3f).setOnComplete(() =>
            {
                CG.gameObject.SetActive(false);
                PlayerManagerUI.Instance.CheckPhysicalForm();
                MapCameraUtils.FocusOnPosition(PlayerManager.marker.GameObject.transform.position, true, 2f);
            });

        });
    }

    void Start()
    {

    }

    private bool m_IsLandingAnim;
    private bool m_IsFlyingAnim;
    public void FlyOut()
    {
        if (map == null)
        {
            map = MapsAPI.Instance;
        }
        if (map.streetLevel)
        {
            if (m_IsFlyingAnim == false)
            {
                m_IsFlyingAnim = true;
                m_CameraControl.OnFlyButton(() => m_IsFlyingAnim = false);
            }
        }
        else if (!map.streetLevel)
        {
            if (m_IsLandingAnim == false)
            {
                m_IsLandingAnim = true;
                m_CameraControl.OnLandButton(() => m_IsLandingAnim = false);
            }
        }
    }

    void TransitionIn()
    {
        if (PlayerManagerUI.Instance != null)
        {
            // if (PlayerManager.inSpiritForm && !hasPlayed)
            // {
            //     SoundManagerOneShot.Instance.PlaySpiritForm();
            //     hasPlayed = true;
            // }
            SoundManagerOneShot.Instance.PlayLandFX(.78f);
            PlayerManagerUI.Instance.LandFX.SetActive(true);
        }
        m_CameraControl.OnLandZoomIn(m_Material);
    }
}