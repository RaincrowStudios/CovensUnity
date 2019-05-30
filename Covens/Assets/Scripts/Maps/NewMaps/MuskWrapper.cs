using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public class MuskWrapper : IMaps
    {
        private MapCameraController m_CamController;
        private CovensMuskMap m_Map;
        private BuildingIconManager m_Icons;

        public void InstantiateMap()
        {
            if (m_Map != null)
                return;

            m_Map = GameObject.FindObjectOfType<CovensMuskMap>();
            if (m_Map == null)
                m_Map = GameObject.Instantiate(Resources.Load<CovensMuskMap>("CovensMuskMap"));
            m_CamController = m_Map.GetComponentInChildren<MapCameraController>();
            m_Icons = m_Map.GetComponentInChildren<BuildingIconManager>();

            Initialize();
        }

        private void Initialize()
        {
            m_LastGPS = physicalPosition;
            PhysicalPositionHelper.Instance.OnPositionChange += OnPhysicalPositionChange;

            m_CamController.onUserPan += () => m_DidPanSinceLand = true;
            m_CamController.onEnterStreetLevel += () => m_DidPanSinceLand = false;
            m_CamController.disablePanning = () => (BanishManager.isBind || DeathState.IsDead);
            m_CamController.lockControls = () => (PlaceOfPower.IsInsideLocation);
        }
        
        private HashSet<MuskMarker> m_Markers = new HashSet<MuskMarker>();

        public Camera camera { get { return m_CamController.camera; } }

        public Transform mapCenter { get { return m_CamController.CenterPoint; } }

        public Transform trackedContainer { get { return m_Map.itemContainer; } }

        public bool streetLevel { get { return m_CamController.streetLevel; } }

        public bool IsPointInsideView(Vector3 point)
        {
            return m_Map.IsPointInsideView(point);
        }

        public Bounds coordinateBounds { get { return m_Map.coordsBounds; } }

        public Vector2 position
        {
            get
            {
                double lng, lat;
                m_Map.GetCoordinates(out lng, out lat);
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
            marker.transform.SetParent(m_Map.itemContainer);

            if (marker == null)
                marker = markerInstance.AddComponent<MuskMarker>();

            marker.coords = position;
            marker.transform.position = MapsAPI.Instance.GetWorldPosition(position.x, position.y);
            m_Markers.Add(marker);

            return marker;
        }

        public void RemoveMarker(IMarker marker)
        {
            if (marker == null)
                return;

            MuskMarker _marker = marker as MuskMarker;
            m_Markers.Remove(_marker);

            //Debug.LogError("destroying " + _marker.name);

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
            return m_Map.GetWorldPosition();
        }

        public Vector3 GetWorldPosition(double lng, double lat)
        {
            return m_Map.GetWorldPosition(lng, lat);
        }

        public void GetPosition(out double lng, out double lat)
        {
            m_Map.GetCoordinates(out lng, out lat);
        }

        public void GetPosition(Vector3 worldPos, out double lng, out double lat)
        {
            m_Map.GetCoordinates(worldPos, out lng, out lat);
        }

        public void SetPosition(double lng, double lat)
        {
            m_Map.SetPosition(lng, lat);
        }

        public void ClearAllCaches()
        {
            Debug.LogError("TODO: ClearAllCaches");
        }

        public bool allowControl
        {
            get { return m_CamController.controlEnabled; }
            set { m_CamController.EnableControl(value); }
        }

        public float zoom
        {
            get { return m_Map.zoom; }
        }
        public float normalizedZoom
        {
            get { return m_Map.normalizedZoom; }
        }
        public float streetLevelNormalizedZoom
        {
            get { return m_CamController.streetLevelNormalizedZoom; }
        }

        public Action OnChangePosition
        {
            get { return m_CamController.onChangePosition; }
            set { m_CamController.onChangePosition = value; }
        }

        public Action OnChangeZoom
        {
            get { return m_CamController.onChangeZoom; }
            set { m_CamController.onChangeZoom = value; }
        }

        public Action OnChangeRotation
        {
            get { return m_CamController.onChangeRotation; }
            set { m_CamController.onChangeRotation = value; }
        }

        public Action<bool, bool, bool> OnCameraUpdate
        {
            get { return m_CamController.onUpdate; }
            set { m_CamController.onUpdate = value; }
        }

        public System.Action OnEnterStreetLevel
        {
            get { return m_CamController.onEnterStreetLevel; }
            set { m_CamController.onEnterStreetLevel = value; }
        }

        public System.Action OnExitStreetLevel
        {
            get { return m_CamController.onExitStreetLevel; }
            set { m_CamController.onExitStreetLevel = value; }
        }

        public System.Action OnMoveOriginPoint
        {
            get { return m_Map.onMoveFloatingOrigin; }
            set { m_Map.onMoveFloatingOrigin = value; }
        }

        public void InitMap(double longitude, double latitude, float zoom, System.Action callback, bool animate)
        {
            m_Map.InitMap(longitude, latitude, zoom, callback);
            m_CamController.onUpdate?.Invoke(true, true, true);
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
            m_Map.HideMap(hide);
        }


        public void EnableBuildings(bool enable)
        {
            m_Map.EnableBuildings(enable);
        }

        public void EnableBuildingIcons(bool enable)
        {
            if (m_Icons != null && m_Icons.initialized)
                m_Icons.EnableIcons(enable);
        }

        public void ScaleBuildings(float scale)
        {
            m_Map.ScaleBuildings(scale);
        }

        private Vector2 m_LastGPS;
        private bool m_DidPanSinceLand;
        private int m_PlayerGPSTweenId;

        public void OnPhysicalPositionChange()
        {
            //dont update if the player is flying
            if (!streetLevel)
            {
                m_LastGPS = MapsAPI.Instance.physicalPosition;
                return;
            }

            if (PlayerManager.marker == null)
                return;

            //the player was in spirit form
            if (MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, m_LastGPS) > 0.05f)
            {
                m_LastGPS = physicalPosition;
                return;
            }

            //10 meters from the last distance
            if (MapsAPI.Instance.DistanceBetweenPointsD(physicalPosition, m_LastGPS) > 0.01f)
            {
                m_LastGPS = physicalPosition;
                Vector3 worldPos = GetWorldPosition(m_LastGPS.x, m_LastGPS.y);

                //move the map center to the new position
                m_Map.MoveFloatingOrigin(worldPos, false);

                worldPos = GetWorldPosition(m_LastGPS.x, m_LastGPS.y);

                //animate the marker to the new position
                LeanTween.cancel(m_PlayerGPSTweenId);
                m_PlayerGPSTweenId = LeanTween.move(PlayerManager.marker.gameObject, worldPos, 1f).setEaseOutCubic().uniqueId;

                //animate the camera to the new position
                if (m_DidPanSinceLand == false)
                    m_CamController.AnimatePosition(worldPos, 1f, true);
                else //clamp the position to make sure the camera doesnt go out of the display radius
                    m_CamController.CenterPoint.transform.position = m_CamController.ClampPosition(m_CamController.CenterPoint.transform.position);

                //request markers
                MarkerManagerAPI.GetMarkers(m_LastGPS.x, m_LastGPS.y, true, null, false, false, false);
            }
        }
    }
}
