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
    private bool m_MapInitialized = false;

    private Vector3 m_InitialCameraPosition;
    private Quaternion m_InitialCameraRotation;
    private Quaternion m_InitialCenterRotation;

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

    public Camera camera { get { return m_Controller.camera; } }

    public Action OnChangePosition { get; set; }
    public Action OnChangeZoom { get; set; }

    private void Awake()
    {
        m_InitialCameraPosition = m_Controller.camera.transform.position;
        m_InitialCameraRotation = m_Controller.camera.transform.rotation;
        m_InitialCenterRotation = m_Controller.CenterPoint.rotation;

        m_Controller.EnableControl(false);
    }
    
    public void Show(double longidute, double latitude)
    {
        gameObject.SetActive(true);
        m_Controller.camera.gameObject.SetActive(true);
        if (m_MapInitialized)
        {
            m_Map.UpdateMap(new Mapbox.Utils.Vector2d(latitude, longidute), 16.8f);
        }
        else
        {
            m_Map.Initialize(new Mapbox.Utils.Vector2d(latitude, longidute), 16.8f);
            m_MapInitialized = true;
        }
        m_Controller.EnableControl(true);

        LeanTween.cancel(m_TweenId);

        //set zoom  to max
        m_Controller.zoom = m_Controller.maxZoom;

        //reset to initial local positions/rotations
        m_Controller.CenterPoint.localPosition = Vector3.zero;
        m_Controller.CenterPoint.rotation = m_InitialCenterRotation;
        m_Controller.camera.transform.position = m_InitialCameraPosition;
        m_Controller.camera.transform.rotation = m_InitialCameraRotation;

        //tween zoom out
        m_Controller.onChangeZoom += OnChangeZoomWhileAnimating;
        m_TweenId = LeanTween.value(m_Controller.zoom, m_Controller.minZoom + (m_Controller.maxZoom - m_Controller.minZoom) * 0.5f, 2f)
            .setEaseOutCubic()
            .setDelay(0.5f)
            .setOnUpdate((float t) =>
            {
                m_Controller.camera.fieldOfView = t;
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

    private void OnChangeZoomWhileAnimating()
    {
        LeanTween.cancel(m_TweenId);
        m_Controller.onChangeZoom -= OnChangeZoomWhileAnimating;
    }

    public void EnableBuildings(bool enable)
    {
        m_Map.VectorData.FindFeatureSubLayerWithName("Building").SetActive(enable);
    }
}
