using Mapbox.Utils;
using Raincrow.Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }

    [SerializeField] public StreetMapWrapper m_StreetMap;
    [SerializeField] public WorldMapWrapper m_WorldMap;

    [Header("canvas")]
    [SerializeField] private CanvasGroup m_OverlayUI;
    
    private int m_OverlayTweenId;

    public bool isWorld { get { return m_WorldMap.gameObject.activeSelf; } }
    public bool isStreet { get { return m_StreetMap.gameObject.activeSelf; } }

    public Action OnChangePosition { get; set; }
    public Action OnChangeZoom { get; set; }
    public Action OnMapUpdated { get; set; }

    /// <summary>
    /// returns the coordinates the camera is currently focused at
    /// </summary>
    public Vector2 position
    {
        get
        {
            double lng, lat;

            if (isWorld)
                m_WorldMap.GetPosition(out lng, out lat);
            else
                m_StreetMap.GetPosition(out lng, out lat);

            return new Vector2((float)lng, (float)lat);
        }
        set
        {
            if (isStreet)
                m_StreetMap.SetPosition(value.x, value.y);
        }
    }

    public float zoom
    {
        get
        {
            if (isWorld)
                return m_WorldMap.zoom;
            else
                return m_WorldMap.zoom;
        }
        set
        {
            if (isWorld)
                m_WorldMap.zoom = value;
            else
                m_StreetMap.zoom = value;
        }
    }
    
    public bool allowControl
    {
        get
        {
            if (isWorld)
                return m_WorldMap.allowControl;
            else
                return m_StreetMap.allowControl;
        }
        set
        {
            if (isWorld)
                m_WorldMap.allowControl = value;
            else
                m_StreetMap.allowControl = value;
        }
    }


    private void Awake()
    {
        Instance = this;

        m_OverlayUI.alpha = 0;
        m_OverlayUI.gameObject.SetActive(false);

        m_WorldMap.Initialize();
        m_WorldMap.Hide(0);
        m_StreetMap.Hide();
    }

    /// <summary>
    /// Initialize the map at the stree level in the given coordinates
    /// </summary>
    public void InitMap(double longitude, double latitude)
    {
        if (isWorld)
            m_WorldMap.Hide(0);
        m_StreetMap.Show(longitude, latitude);
    }

    /// <summary>
    /// Smoothly transitions from world map to street map.
    /// </summary>
    public void ShowStreetMap(double longitude, double latitude, Action callback)
    {
        //if (!isStreet)
        //{
            LeanTween.cancel(m_OverlayTweenId);

            //show the black overlay
            m_OverlayUI.blocksRaycasts = true;
            m_OverlayUI.gameObject.SetActive(true);

            //hide the worldmap
            m_WorldMap.Hide();

            m_OverlayTweenId = LeanTween.value(m_OverlayUI.alpha, 1, 0.5f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_OverlayUI.alpha = t;
                })
                .setOnComplete(() =>
                {
                    //init the streetmap
                    m_StreetMap.Show(longitude, latitude);
                    m_OverlayUI.blocksRaycasts = false;

                    //hide the overlay after few seconds
                    m_OverlayTweenId = LeanTween.value(m_OverlayUI.alpha, 0, 1f)
                        .setEaseOutCubic()
                        .setDelay(0.5f)
                        .setOnStart(() =>
                        {

                            callback?.Invoke();
                        })
                        .setOnUpdate((float t) =>
                        {
                            m_OverlayUI.alpha = t;
                        })
                        .setOnComplete(() =>
                        {
                            m_OverlayUI.gameObject.SetActive(false);
                        })
                        .uniqueId;
                })
                .uniqueId;
        //}
        //else
        //{
        //    Debug.LogError("Already showing street map");
        //}
    }

    /// <summary>
    /// Smoothly transitions from street map to worldmap.
    /// </summary>
    public void ShowWorldMap(double longitude, double latitude, Action callback)
    {
        if (!isWorld)
        {
            LeanTween.cancel(m_OverlayTweenId);

            //show the black overlay
            m_OverlayUI.blocksRaycasts = true;
            m_OverlayUI.gameObject.SetActive(true);

            LeanTween.value(m_StreetMap.zoom, m_StreetMap.maxZoom, 0.5f)
                .setOnUpdate((float t) =>
                {
                    m_StreetMap.zoom = t;
                });

            m_OverlayTweenId = LeanTween.value(m_OverlayUI.alpha, 1, 0.5f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_OverlayUI.alpha = t;
                })
                .setOnComplete(() =>
                {
                    m_StreetMap.Hide();
                    m_WorldMap.Show(longitude, latitude);
                    callback?.Invoke();
                    m_OverlayUI.blocksRaycasts = false;

                    m_OverlayTweenId = LeanTween.value(m_OverlayUI.alpha, 0, 0.5f)
                        .setEaseOutCubic()
                        .setOnUpdate((float t) =>
                        {
                            m_OverlayUI.alpha = t;
                        })
                        .setOnComplete(() =>
                        {
                            m_OverlayUI.gameObject.SetActive(false);
                        })
                        .uniqueId;
                })
                .uniqueId;
        }
        else
        {
            Debug.LogError("Already showing world map");
        }
    }
    
    /// <summary>
    /// Instantly hides both maps.
    /// </summary>
    public void HideMap()
    {
        if (isWorld)
            m_WorldMap.Hide(0);
        if (isStreet)
            m_StreetMap.Hide();
    }

    /// <summary>
    /// Returns the scene position for the given coordinates.
    /// </summary>
    public Vector3 CoordsToWorldPosition(double longitude, double latitude)
    {
        if (isWorld)
            return m_WorldMap.CoordsToWorldPosition(longitude, latitude);
        else
            return m_StreetMap.CoordsToWorldPosition(longitude, latitude);
    }

    /// <summary>
    /// Returns the geo coordinates for the given scene position.
    /// </summary>
    public Vector2 WorldToCoords(Vector3 position)
    {
        if (isWorld)
            return m_WorldMap.WorldToCoords(position);
        else
            return m_StreetMap.WorldToCoords(position);
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetPosition(out double longitude, out double latitude)
    {
        if (isWorld)
            m_WorldMap.GetPosition(out longitude, out latitude);
        else
            m_StreetMap.GetPosition(out longitude, out latitude);
    }

    public void SetPosition(double longitude, double latitude)
    {
        if (isWorld)
            m_WorldMap.SetPosition(longitude, latitude);
        else
            m_StreetMap.SetPosition(longitude, latitude);
    }

    public Camera camera
    {
        get {
            if (isWorld)
                return m_WorldMap.camera;
            else
                return m_StreetMap.camera;
        }
    }
}
