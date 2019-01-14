using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public class OMOnlineMaps : IMaps
    {
        private OnlineMaps m_OnlineMaps;
        private Dictionary<IMarker, OnlineMapsMarker3D> m_Markers = new Dictionary<IMarker, OnlineMapsMarker3D>();

        public Transform transform { get { return m_OnlineMaps.transform; } }

        public Vector2 position
        {
            get { return m_OnlineMaps.position; }
            set { m_OnlineMaps.position = value; }
        }

        public Vector2 physicalPosition
        {
            get { return OnlineMapsLocationService.instance.position; }
            set { OnlineMapsLocationService.instance.position = value; }
        }

        public IMarker AddMarker(Vector2 position, GameObject prefab)
        {
            OnlineMapsMarker3D marker = OnlineMapsControlBase3D.instance.AddMarker3D(position, prefab);
            OMMarker3D omMarker = new OMMarker3D(marker);
            m_Markers.Add(omMarker, marker);
            return omMarker;
        }

        public double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
        {
            return OnlineMapsUtils.DistanceBetweenPointsD(point1, point2);
        }

        public void GetPosition(out double lng, out double lat)
        {
            m_OnlineMaps.GetPosition(out lng, out lat);
        }

        public void RemoveMarker(IMarker marker)
        {
            OnlineMapsMarker3D marker3D = m_Markers[marker];
            m_Markers.Remove(marker);
            OnlineMapsControlBase3D.instance.RemoveMarker3D(marker3D);
        }

        public void SetPosition(double lng, double lat)
        {
            m_OnlineMaps.SetPosition(lng, lat);
        }

        public void SetPositionAndZoom(double lng, double lat, int zoom = 0)
        {
            m_OnlineMaps.SetPositionAndZoom(lng, lat, zoom);
        }



        public Action OnChangePosition
        {
            get { return m_OnlineMaps.OnChangePosition; }
            set { m_OnlineMaps.OnChangePosition = value; }
        }

        public Action OnMapUpdated
        {
            get { return m_OnlineMaps.OnMapUpdated; }
            set { m_OnlineMaps.OnMapUpdated = value; }
        }
    }
}