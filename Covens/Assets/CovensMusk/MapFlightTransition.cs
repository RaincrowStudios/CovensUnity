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

        //Check if at same pos
        double ln = 0;
        double lt = 0;
        map.GetPosition(out ln, out lt);
        if (Math.Round(ln, 6) == Math.Round(map.physicalPosition.x, 6) && Math.Round(map.physicalPosition.y, 6) == Math.Round(lt, 6)) return;
        CG.gameObject.SetActive(true);
        CG.alpha = 0;
        m_CameraControl.OnFlyButton(() => LeanTween.alphaCanvas(CG, 1, .3f).setOnComplete(() =>
        {
            map.SetPosition(map.physicalPosition.x, map.physicalPosition.y);
            LeanTween.alphaCanvas(CG, 0, .3f).setOnComplete(() =>
            {
                CG.gameObject.SetActive(false);
                Canfly = false;
                m_CameraControl.OnLandButton(true);
            });
        }));
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
        if (Canfly)
        {
            m_CameraControl.OnFlyButton(() => { });
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