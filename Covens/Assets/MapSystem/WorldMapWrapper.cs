using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldMapWrapper : MonoBehaviour
{
    [SerializeField] private SpriteMapsController m_Map;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_FlyFX;
    [SerializeField] private SpriteRenderer m_FlyIcon;

    private int m_TweenId;

    public float zoom
    {
        get { return m_Map.zoom; }
        set { m_Map.zoom = zoom; }
    }

    public bool allowControl
    {
        get { return true; }
        set { }
    }

    public Camera camera { get { return m_Camera; } }
    
    public Action OnChangePosition
    {
        get { return m_Map.onChangePosition; }
        set { m_Map.onChangePosition = value; }
    }
    public Action OnChangeZoom
    {
        get { return m_Map.onChangeZoom; }
        set { m_Map.onChangeZoom = value; }
    }

    private void Awake()
    {
        EnableFlyFX(false);
    }

    public void Initialize()
    {
        m_Map.Initialize();
    }

    public void Show(double longitude, double latitude)
    {
        gameObject.SetActive(true);

        LeanTween.cancel(m_TweenId);

        //set zoom  to max
        m_Map.zoom = m_Map.m_MinZoom;

        //focus on the given coords
        m_Camera.transform.position = m_Map.ClampCameraY(m_Map.GetWorldPosition((float)longitude, (float)latitude));

        //tween zoom out
        m_Map.onChangeZoom += OnMapUpdate;
        m_Map.onChangePosition += OnMapUpdate;
        m_TweenId = LeanTween.value(m_Map.m_MinZoom, m_Map.m_MinZoom + (m_Map.m_MaxZoom - m_Map.m_MinZoom) * 0.0f, 2f)
            .setEaseInOutCubic()
            .setDelay(0.5f)
            .setOnStart(() =>
            {
                m_Map.onChangePosition?.Invoke();
            })
            .setOnUpdate((float t) =>
            {
                m_Map.m_Camera.orthographicSize = t;
            })
            .uniqueId;

        EnableFlyFX(true);
    }

    public void Hide(float duration = 1f)
    {
        if(duration == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (gameObject.activeSelf == false)
            return;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_Map.zoom, m_Map.m_MinZoom, duration)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Map.zoom = t;
            })
            .setOnComplete(()=>
            {
                gameObject.SetActive(false);
            })
            .uniqueId;

        EnableFlyFX(false);
    }

    /// <summary>
    /// return the coordinates of the camera center
    /// </summary>
    public void GetPosition(out double longitude, out double latitude)
    {
        Vector2 coords = m_Map.GetCoordinatesFromWorldPosition(m_Camera.transform.position.x, m_Camera.transform.position.y);
        longitude = coords.x;
        latitude = coords.y;
    }

    /// <summary>
    /// set the camera to the given coordinates
    /// </summary>
    public void SetPosition(double longitude, double latitude)
    {
        m_Map.ClampCameraY(m_Map.GetWorldPosition((float)longitude, (float)latitude));
    }


    public Vector3 CoordsToWorldPosition(double longitude, double latitude)
    {
        return m_Map.GetWorldPosition((float)longitude, (float)latitude);
    }
    public Vector2 WorldToCoords(Vector3 position)
    {
        return m_Map.GetCoordinatesFromWorldPosition(position.x, position.y);
    }


    private void OnMapUpdate()
    {
        LeanTween.cancel(m_TweenId);
        m_Map.onChangeZoom -= OnMapUpdate;
        m_Map.onChangePosition -= OnMapUpdate;
    }

    private void EnableFlyFX(bool enable)
    {
        if (enable)
        {
            m_FlyIcon.sprite = PlayerManager.witchMarker.portrait;
        }

        m_FlyFX.SetActive(enable);
    }
}
