using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldMapWrapper : MonoBehaviour
{
    [SerializeField] private SpriteMapsController m_Map;
    [SerializeField] private Camera m_Camera;

    private int m_TweenId;

    public float zoom
    {
        get { return m_Map.zoom; }
        set { m_Map.zoom = zoom; }
    }

    public bool allowZoom
    {
        get { return true; }
        set { }
    }

    public bool allowControl
    {
        get { return true; }
        set { }
    }

    public Camera camera { get { return m_Camera; } }
    
    public Action OnChangePosition { get; set; }
    public Action OnChangeZoom { get; set; }
    public Action OnMapUpdated { get; set; }

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
        m_TweenId = LeanTween.value(m_Map.zoom, 10, 2f)
            .setEaseInOutCubic()
            .setDelay(0.5f)
            .setOnUpdate((float t) =>
            {
                m_Map.zoom = t;
            })
            .uniqueId;
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
}
