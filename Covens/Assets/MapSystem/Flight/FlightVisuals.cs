using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Raincrow.Maps;

public class FlightVisuals : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private Transform m_FlyFxObj;
    [SerializeField] private SpriteRenderer m_PlayerPortrait;

    [SerializeField] private Transform Particles;
    [SerializeField] private GameObject FlyFX;
    [SerializeField] private Transform attackRing;
    //public GameObject UIFlyGlow;
    private IMaps map;
    private float[] m_Multipliers = new float[]
    {
        5000,
        5000,
        5000,
        4000,
        3000,
        2048,
        1000,
        560,
        450,
        250,
        128,
        40,
        40,
        8,
        3,
        2,
        2,
        2
    };

    private static FlightVisuals m_Instance;
    public static FlightVisuals Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<FlightVisuals>("FlightVisuals"));
            return m_Instance;
        }
    }

    private Vector3 m_LastMapPosition;

    private void OnMapPan()
    {
        Vector3 newPos = MapsAPI.Instance.GetWorldPosition();
        Vector3 delta = (newPos - m_LastMapPosition);

        m_LastMapPosition = newPos;

        delta = new Vector3(delta.x, delta.z) / m_Multipliers[(int)MapsAPI.Instance.zoom];
        m_FlyFxObj.transform.position += delta * 0.1f;
    }


    private void OnMapZoom()
    {
        float zoom = map.normalizedZoom;
        if (zoom > .689f && zoom < .855f)
        {
            attackRing.gameObject.SetActive(true);
            float multiplier = 0;

            if (zoom < .7166f)
            {
                multiplier = MapUtils.scale(.84f, 1.3f, .689f, .7166f, map.normalizedZoom);
            }
            else if (zoom < .75464f)
            {
                multiplier = MapUtils.scale(1.3f, 2.65f, .7166f, .75464f, map.normalizedZoom);
            }
            else if (zoom < .8f)
            {
                multiplier = MapUtils.scale(2.65f, 6.5f, .75464f, .8f, map.normalizedZoom);
            }
            else
            {
                multiplier = MapUtils.scale(6.5f, 20.3536f, .8f, .855f, map.normalizedZoom);
            }

            attackRing.transform.localScale = Vector3.one * multiplier;
        }
        else
        {
            attackRing.gameObject.SetActive(false);
        }
    }

    private void OnMoveFloatingOrigin()
    {
        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();
    }

    [ContextMenu("Start flight")]
    public void StartFlight()
    {
        map = MapsAPI.Instance;
        //     IconFXColor();
        MapsAPI.Instance.OnChangePosition += OnMapPan;
        MapsAPI.Instance.OnMoveOriginPoint += OnMoveFloatingOrigin;
        MapsAPI.Instance.OnChangeZoom += OnMapZoom;


        //Debug.Log ("StartFlight");

        m_LastMapPosition = MapsAPI.Instance.GetWorldPosition();

        gameObject.SetActive(true);
        LeanTween.scale(FlyFX, Vector3.one, 0.3f);
        if (PlayerManager.marker != null)
            (PlayerManager.marker as WitchMarker).GetPortrait(spr => m_PlayerPortrait.sprite = spr);
        if (DeathState.Instance != null)
            DeathState.Instance.FlightGlowOn();
    }

    [ContextMenu("End flight")]
    public void EndFlight()
    {
        MapsAPI.Instance.OnChangePosition -= OnMapPan;
        MapsAPI.Instance.OnMoveOriginPoint -= OnMoveFloatingOrigin;
        MapsAPI.Instance.OnChangeZoom -= OnMapZoom;
        //Debug.Log ("EndFlight");
        if (DeathState.Instance != null)
            DeathState.Instance.FlightGlowOff();
        LeanTween.scale(FlyFX, Vector3.zero, 0.6f).setOnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}