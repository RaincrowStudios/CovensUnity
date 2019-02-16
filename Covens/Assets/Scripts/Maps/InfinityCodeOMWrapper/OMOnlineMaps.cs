using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public class OMOnlineMaps : IMaps
    {
        private Dictionary<IMarker, OnlineMapsMarker3D> m_Markers = new Dictionary<IMarker, OnlineMapsMarker3D>();
        private OnlineMaps m_OnlineMaps;
        private OnlineMaps onlineMaps
        {
            get
            {
                if (m_OnlineMaps == null)
                    m_OnlineMaps = OnlineMaps.instance;
                return m_OnlineMaps;
            }
        }

        public Transform transform { get { return onlineMaps.transform; } }

        public Vector2 position
        {
            get { return onlineMaps.position; }
            set { onlineMaps.position = value; }
        }

        public int zoom
        {
            get { return onlineMaps.zoom; }
            set { onlineMaps.zoom = value; }
        }

        public Vector2 physicalPosition
        {
            get { return OnlineMapsLocationService.instance.position; }
            //set { OnlineMapsLocationService.instance.position = value; }
        }

        public IMarker AddMarker(Vector2 position, GameObject prefab)
        {
            OnlineMapsMarker3D marker = OnlineMapsControlBase3D.instance.AddMarker3D(position, prefab);
            OMMarker3D omMarker = new OMMarker3D(marker);
            m_Markers.Add(omMarker, marker);
            return omMarker;
        }

        public Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2)
        {
            return OnlineMapsUtils.DistanceBetweenPoints(point1, point2);
        }

        public double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
        {
            return OnlineMapsUtils.DistanceBetweenPointsD(point1, point2);
        }

        public void GetPosition(out double lng, out double lat)
        {
            onlineMaps.GetPosition(out lng, out lat);
        }

        public void RemoveMarker(IMarker marker)
        {
            if (marker == null)
                return;

            OnlineMapsMarker3D marker3D = m_Markers[marker];
            m_Markers.Remove(marker);
            OnlineMapsControlBase3D.instance.RemoveMarker3D(marker3D);
        }

        public void SetPosition(double lng, double lat)
        {
            onlineMaps.SetPosition(lng, lat);
        }

        public void SetPositionAndZoom(double lng, double lat, int zoom = 0)
        {
            onlineMaps.SetPositionAndZoom(lng, lat, zoom);
        }

        public void RedrawImmediately()
        {
            onlineMaps.RedrawImmediately();
        }

        public void ClearAllCaches()
        {
            OnlineMapsCache.instance.ClearAllCaches();
        }

        public Action OnChangePosition
        {
            get { return onlineMaps.OnChangePosition; }
            set { onlineMaps.OnChangePosition = value; }
        }

        public Action OnChangeZoom
        {
            get { return onlineMaps.OnChangeZoom; }
            set { onlineMaps.OnChangeZoom = value; }
        }

        public Action OnMapUpdated
        {
            get { return onlineMaps.OnMapUpdated; }
            set { onlineMaps.OnMapUpdated = value; }
        }

        public Vector2 topLeftPosition
        {
           get { return onlineMaps.topLeftPosition; }
        }

        public Vector2 bottomRightPosition
        {
            get { return onlineMaps.bottomRightPosition; }
        }

        public Vector2 tilesetSize
        {
            get { return onlineMaps.tilesetSize; }
        }

        public string customProviderURL
        {
            get { return onlineMaps.customProviderURL; }
            set { onlineMaps.customProviderURL = value; }
        }

        public bool allowZoom
        {
            get { return OnlineMapsTileSetControl.instance.allowZoom; }
            set { OnlineMapsTileSetControl.instance.allowZoom = value; }
        }

        public bool allowUserControl
        {
            get { return OnlineMapsTileSetControl.instance.allowUserControl; }
            set { OnlineMapsTileSetControl.instance.allowUserControl = value; }
        }
        public bool allowCameraControl
        {
            get { return OnlineMapsTileSetControl.instance.allowCameraControl; }
            set { OnlineMapsTileSetControl.instance.allowCameraControl = value; }
        }

        public void HideMap(bool hide)
        {
            transform.GetComponent<MeshRenderer>().enabled = !hide;
        }
    }
}