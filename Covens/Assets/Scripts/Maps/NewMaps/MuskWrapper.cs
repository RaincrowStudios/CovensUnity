using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public class MuskWrapper : IMaps
    {
        private HashSet<MuskMarker> m_Markers = new HashSet<MuskMarker>();
        private Vector2 m_LastPosition = new Vector2();

        public Camera camera { get { return MapCameraController.Instance.camera; } }

        public Transform mapCenter { get { return MapCameraController.Instance.CenterPoint; } }

        public Transform trackedContainer { get { return CovensMuskMap.Instance.itemContainer; } }

        public bool streetLevel { get { return CovensMuskMap.Instance.streetLevel; } }

        public Bounds visibleBounds { get { return CovensMuskMap.Instance.cameraBounds; } }
        
        public Vector2 position
        {
            get
            {
                double lng, lat;
                CovensMuskMap.Instance.GetCoordinates(out lng, out lat);
                return new Vector2((float)lng, (float)lat);
            }
            set
            {
                SetPosition(value.x, value.y);
            }
        }

        public Vector2 physicalPosition
        {
            get { return new Vector2(GetGPS.longitude, GetGPS.latitude); }
        }

        public IMarker AddMarker(Vector2 position, GameObject prefab)
        {
            GameObject markerInstance = GameObject.Instantiate(prefab);
            MuskMarker marker = markerInstance.GetComponent<MuskMarker>();
            marker.transform.SetParent(CovensMuskMap.Instance.itemContainer);

            if (marker == null)
                marker = markerInstance.AddComponent<MuskMarker>();

            marker.position = position;
            m_Markers.Add(marker);

            return marker;
        }

        public void RemoveMarker(IMarker marker)
        {
            if (marker == null)
                return;

            MuskMarker _marker = marker as MuskMarker;
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

        public Vector3 GetWorldPosition()
        {
            return CovensMuskMap.Instance.GetWorldPosition();
        }

        public Vector3 GetWorldPosition(double lng, double lat)
        {
            return CovensMuskMap.Instance.GetWorldPosition(lng, lat);
        }

        public void GetPosition(out double lng, out double lat)
        {
            CovensMuskMap.Instance.GetCoordinates(out lng, out lat);
        }

        public void GetPosition(Vector3 worldPos, out double lng, out double lat)
        {
            CovensMuskMap.Instance.GetCoordinates(worldPos, out lng, out lat);
        }

        public void SetPosition(double lng, double lat)
        {
            m_LastPosition = new Vector2((float)lng, (float)lat);
            CovensMuskMap.Instance.SetPosition(lng, lat);
        }

        public void ClearAllCaches()
        {
            Debug.LogError("TODO: ClearAllCaches");
        }

        public bool allowControl
        {
            get { return MapCameraController.Instance.controlEnabled; }
            set { MapCameraController.Instance.EnableControl(value); }
        }

        public float zoom
        {
            get { return CovensMuskMap.Instance.normalizedZoom; }
        }
        public float normalizedZoom
        {
            get { return CovensMuskMap.Instance.normalizedZoom; }
        }
        public float streetLevelNormalizedZoom
        {
            get { return MapCameraController.Instance.streetLevelNormalizedZoom; }
        }

        public Action OnChangePosition
        {
            get { return MapCameraController.Instance.onChangePosition; }
            set { MapCameraController.Instance.onChangePosition = value; }
        }

        public Action OnChangeZoom
        {
            get { return MapCameraController.Instance.onChangeZoom; }
            set { MapCameraController.Instance.onChangeZoom = value; }
        }

        public Action OnChangeRotation
        {
            get { return MapCameraController.Instance.onChangeRotation; }
            set { MapCameraController.Instance.onChangeRotation = value; }
        }

        public Action<bool, bool, bool> OnCameraUpdate
        {
            get { return MapCameraController.Instance.onUpdate; }
            set { MapCameraController.Instance.onUpdate = value; }
        }

        public System.Action OnEnterStreetLevel
        {
            get { return MapCameraController.Instance.onEnterStreetLevel; }
            set { MapCameraController.Instance.onEnterStreetLevel = value; }
        }

        public System.Action OnExitStreetLevel
        {
            get { return MapCameraController.Instance.onExitStreetLevel; }
            set { MapCameraController.Instance.onExitStreetLevel = value; }
        }


        public void InitMap()
        {
            InitMap(GetGPS.longitude, GetGPS.latitude, 1, null, true);
        }

        public void InitMap(double longitude, double latitude, float zoom, System.Action callback, bool animate)
        {
            CovensMuskMap.Instance.InitMap(longitude, latitude, zoom, callback);
            MapCameraController.Instance.onUpdate?.Invoke(true, true, true);
        }

        //public void ShowWorldMap()
        //{
        //    Vector2 pos = physicalPosition;
        //    MapController.Instance.ShowWorldMap(pos.x, pos.y, null);
        //}

        //public void ShowStreetMap(double longitude, double latitude, Action callback, bool animate)
        //{
        //    MapController.Instance.ShowStreetMap(longitude, latitude, callback, animate);
        //}

        //public void ShowWorldMap(double longitude, double latitude)
        //{
        //    MapController.Instance.ShowWorldMap(longitude, latitude, null);
        //}

        public void HideMap(bool hide)
        {
            CovensMuskMap.Instance.HideMap(hide);
        }
    }
}
