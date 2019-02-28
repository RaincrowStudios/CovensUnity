using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public class NewMapsSystem : IMaps
    {
        public HashSet<NewMapsMarker> m_Markers = new HashSet<NewMapsMarker>();

        public Transform transform { get { return MapController.Instance.transform; } }
        private Vector2d m_LastPosition = new Vector2d();

        public Vector2 position
        {
            get { return MapController.Instance.position; }
            set { m_LastPosition = new Vector2d(value.y, value.x); MapController.Instance.position = value; }
        }

        public Vector2 physicalPosition
        {
            get { return new Vector2(GetGPS.longitude, GetGPS.latitude); }
        }

        public IMarker AddMarker(Vector2 position, GameObject prefab)
        {
            GameObject markerInstance = GameObject.Instantiate(prefab);
            NewMapsMarker marker = markerInstance.GetComponent<NewMapsMarker>();
            if (marker == null)
                marker = markerInstance.AddComponent<NewMapsMarker>();

            marker.position = position;
            m_Markers.Add(marker);

            return marker;
        }

        public void RemoveMarker(IMarker marker)
        {
            if (marker == null)
                return;

            NewMapsMarker _marker = marker as NewMapsMarker;
            m_Markers.Remove(_marker);
            GameObject.Destroy(_marker.gameObject);
        }

        public Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2)
        {
            double scfY = Math.Sin(point1.y * Mathf.Deg2Rad);
            double sctY = Math.Sin(point2.y * Mathf.Deg2Rad);
            double ccfY = Math.Cos(point1.y * Mathf.Deg2Rad);
            double cctY = Math.Cos(point2.y * Mathf.Deg2Rad);
            double cX = Math.Cos((point1.x - point2.x) * Mathf.Deg2Rad);
            double sizeX1 = Math.Abs(6371 * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(6371 * Math.Acos(sctY * sctY + cctY * cctY * cX));
            float sizeX = (float)((sizeX1 + sizeX2) / 2.0);
            float sizeY = (float)(6371 * Math.Acos(scfY * sctY + ccfY * cctY));
            if (float.IsNaN(sizeY)) sizeY = 0;
            return new Vector2(sizeX, sizeY);
        }

        public double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
        {
            double scfY = Math.Sin(point1.y * Mathf.Deg2Rad);
            double sctY = Math.Sin(point2.y * Mathf.Deg2Rad);
            double ccfY = Math.Cos(point1.y * Mathf.Deg2Rad);
            double cctY = Math.Cos(point2.y * Mathf.Deg2Rad);
            double cX = Math.Cos((point1.x - point2.x) * Mathf.Deg2Rad);
            double sizeX1 = Math.Abs(6371 * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(6371 * Math.Acos(sctY * sctY + cctY * cctY * cX));
            double sizeX = (sizeX1 + sizeX2) / 2.0;
            double sizeY = 6371 * Math.Acos(scfY * sctY + ccfY * cctY);
            if (double.IsNaN(sizeY)) sizeY = 0;
            return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
        }

        public void GetPosition(out double lng, out double lat)
        {
            MapController.Instance.GetPosition(out lng, out lat);
        }

        public void SetPosition(double lng, double lat)
        {
            m_LastPosition = new Vector2d(lat, lng);
            MapController.Instance.SetPosition(lng, lat);
        }

        public void SetPositionAndZoom(double lng, double lat)
        {
            MapController.Instance.SetPosition(lng, lat);
        }

        public void RedrawImmediately()
        {
            //onlineMaps.RedrawImmediately();
            Debug.LogError("TODO");
        }

        public void ClearAllCaches()
        {
            //OnlineMapsCache.instance.ClearAllCaches();
            Debug.LogError("TODO");
        }
        
        public string customProviderURL
        {
            get { Debug.LogError("TOREPLACE"); return ""; }
            set { Debug.LogError("TOREPLACE"); }
        }
        
        public bool allowUserControl
        {
            get { return MapController.Instance.allowControl; }
            set { MapController.Instance.allowControl = value; }
        }

        public bool allowCameraControl
        {
            get { return true; }
            set { }
        }

        public float zoom
        {
            get { return MapController.Instance.zoom; }
            set { MapController.Instance.zoom = value; }
        }

        public void InitMap()
        {
            MapController.Instance.InitMap(GetGPS.longitude, GetGPS.latitude);
        }

        public void ShowStreetMap(System.Action callback)
        {
            Vector2 pos = physicalPosition;
            MapController.Instance.ShowStreetMap(pos.x, pos.y, callback);
        }

        public void ShowWorldMap()
        {
            Vector2 pos = physicalPosition;
            MapController.Instance.ShowWorldMap(pos.x, pos.y, null);
        }

        public void ShowStreetMap(double longitude, double latitude, Action callback)
        {
            MapController.Instance.ShowStreetMap(longitude, latitude, callback);
        }

        public void ShowWorldMap(double longitude, double latitude)
        {
            MapController.Instance.ShowWorldMap(longitude, latitude, null);
        }

        public void HideMap()
        {
            MapController.Instance.HideMap();
        }

        public bool IsWorld { get { return MapController.Instance.isWorld; } }
    }

}
