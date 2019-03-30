using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StreetMapWrapper : MonoBehaviour
{
    [SerializeField] private AbstractMap m_Map;
    [SerializeField] private MapCameraController m_Controller;
    [SerializeField] private Camera m_MarkerCamera; 
    private bool m_MapInitialized = false;

    private Quaternion m_InitialCenterRotation;
    private Quaternion m_InitialPivotRotation;

    private int m_TweenId;

    public Transform cameraCenter { get { return m_Controller.CenterPoint; } }
    public float zoom { get { return m_Controller.zoom; } set { m_Controller.zoom = value; } }
    public float minZoom { get { return m_Controller.minZoom; } }
    public float maxZoom { get { return m_Controller.maxZoom; } }
        
    public bool allowControl
    {
        get { return m_Controller.controlEnabled; }
        set { m_Controller.EnableControl(value); }
    }

    public new Camera camera { get { return m_Controller.camera; } }

    public Action OnChangePosition
    {
        get { return m_Controller.onChangePosition; }
        set { m_Controller.onChangePosition = value; }
    }
    public Action OnChangeZoom
    {
        get { return m_Controller.onChangeZoom; }
        set { m_Controller.onChangeZoom = value; }
    }

    private void Awake()
    {
        m_InitialCenterRotation = m_Controller.CenterPoint.rotation;
        m_InitialPivotRotation = m_Controller.RotationPivot.rotation;

        m_Controller.EnableControl(false);
    }
    
    public void Show(double longidute, double latitude)
    {
        gameObject.SetActive(true);
        m_Controller.camera.gameObject.SetActive(true);
        if (m_MapInitialized)
        {
            m_Map.UpdateMap(new Mapbox.Utils.Vector2d(latitude, longidute), 17.8f);
        }
        else
        {
            m_Map.Initialize(new Mapbox.Utils.Vector2d(latitude, longidute), 17.8f);
            m_MapInitialized = true;
        }
        m_Controller.EnableControl(true);

        LeanTween.cancel(m_TweenId);

        //set zoom  to max
        m_Controller.zoom = m_Controller.maxZoom;

        //reset to initial local positions/rotations
        m_Controller.CenterPoint.localPosition = Vector3.zero;
        m_Controller.CenterPoint.rotation = m_InitialCenterRotation;
        m_Controller.RotationPivot.rotation = m_InitialPivotRotation;

        //tween zoom out
        m_Controller.onChangeZoom += OnMapUpdate;
        m_Controller.onChangePosition += OnMapUpdate;
        m_TweenId = LeanTween.value(m_Controller.zoom, m_Controller.minZoom + (m_Controller.maxZoom - m_Controller.minZoom) * 0.2f, 2f)
            .setEaseOutCubic()
            .setDelay(0.5f)
            .setOnUpdate((float t) =>
            {
                m_Controller.SetZoom(t, false, true);
            })
            .setOnComplete(() =>
            {
                OnMapUpdate();
            })
            .uniqueId;
    }

    /// <summary>
    /// return the coordinates of the camera center
    /// </summary>
    public void GetPosition(out double longitude, out double latitude)
    {
        Vector2d coords = m_Map.WorldToGeoPosition(m_Controller.CenterPoint.position);
        longitude = coords.y;
        latitude = coords.x;
    }

    /// <summary>
    /// set the camera to the given coordinates
    /// </summary>
    public void SetPosition(double longitude, double latitude)
    {
        m_Controller.CenterPoint.position = m_Map.GeoToWorldPosition(new Vector2d(latitude, longitude));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        m_Controller.camera.gameObject.SetActive(false);
    }

    public Vector3 CoordsToWorldPosition(double longitude, double latitude)
    {
        return m_Map.GeoToWorldPosition(new Vector2d(latitude, longitude), false);
    }

    public Vector2 WorldToCoords(Vector3 position)
    {
        Vector2d result = m_Map.WorldToGeoPosition(position);
        return new Vector2((float)result.y, (float)result.x);
    }

    private void OnMapUpdate()
    {
        LeanTween.cancel(m_TweenId);
        m_Controller.onChangeZoom -= OnMapUpdate;
        m_Controller.onChangePosition -= OnMapUpdate;
    }

    public void EnableBuildings(bool enable)
    {
        m_Map.VectorData.FindFeatureSubLayerWithName("Building").SetActive(enable);
    }

    public void SetVisible(bool visible)
    {
        camera.enabled = visible;
        m_MarkerCamera.enabled = visible;
    }
}
