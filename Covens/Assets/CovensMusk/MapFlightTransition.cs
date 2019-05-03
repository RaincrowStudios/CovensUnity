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
        m_Material = m_RadialBlur.material;
        m_CameraControl.onEnterStreetLevel += TransitionIn;
        // m_CameraControl.onExitStreetLevel += TransitionOut;
    }

    public void RecallHome()
    {
        if (map == null)
        {
            map = MapsAPI.Instance;
        }

        Debug.Log("RECALLING");
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

        map.SetPosition(map.physicalPosition.x, map.physicalPosition.y);
        LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        {
            m_CameraControl.SetZoomRecall(.89f);
            LeanTween.alphaCanvas(CG, 0, .3f).setOnComplete(() =>
           {
               CG.gameObject.SetActive(false);
               m_CameraControl.OnLandButton(true);
               PlayerManagerUI.Instance.home();
           });

        });

        // m_CameraControl.OnFlyButton(() => LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        // {

        // }));
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
        if (map == null)
        {
            map = MapsAPI.Instance;
        }
        if (map.streetLevel)
        {
            m_CameraControl.OnFlyButton(() => { });
        }
        else if (!map.streetLevel)
        {
            m_CameraControl.OnLandButton();
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