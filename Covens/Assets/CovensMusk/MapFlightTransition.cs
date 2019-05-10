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
    // bool hasPlayed = false;
    void Awake()
    {
        Instance = this;
        m_CameraControl = GetComponent<MapCameraController>();
        m_Material = m_RadialBlur.materialInstance;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
    }

    public void RecallHome()
    {
        if (map == null)
        {
            map = MapsAPI.Instance;
        }

        //   Debug.Log("RECALLING");
        // return;
        //Check if at same pos
        // double ln = 0;
        // double lt = 0;
        // map.GetPosition(out ln, out lt);
        // if (Math.Round(ln, 6) == Math.Round(map.physicalPosition.x, 6) && Math.Round(map.physicalPosition.y, 6) == Math.Round(lt, 6))
        if (!PlayerManager.inSpiritForm)
        {
            PlayerManager.Instance.atLocationUIShow();
            return;
        }
        CG.gameObject.SetActive(true);
        CG.alpha = 0;
        // hasPlayed = false;
        LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        {
            map.SetPosition(map.physicalPosition.x, map.physicalPosition.y);
            m_CameraControl.SetZoomRecall(.89f);

            //m_CameraControl.OnLandButton(true);
            m_CameraControl.AnimateZoom(1f, 0.3f, false, m_CameraControl.m_FlyOutCurve);
            MarkerManagerAPI.GetMarkers(true, false, null, false, false);

            LeanTween.alphaCanvas(CG, 0, .3f).setOnComplete(() =>
            {
                CG.gameObject.SetActive(false);
            });

        });

        // m_CameraControl.OnFlyButton(() => LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        // {

        // }));
    }

    void Start()
    {

    }

    private bool m_IsLandingAnim;
    private bool m_IsFlyingAnim;
    public void FlyOut()
    {
        Debug.Log("fly");
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