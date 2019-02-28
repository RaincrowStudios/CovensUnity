using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMaps
    {
        Transform transform { get; }

        /// <summary>
        /// Current zoom
        /// </summary>
        float zoom { get; set; }

        /// <summary>
        /// Coordinates of the center point of the map
        /// </summary>
        Vector2 position { get; set; }

        /// <summary>
        /// Current GPS coordinates.
        /// </summary>
        Vector2 physicalPosition { get; }
        
        string customProviderURL { get; set; }

        bool allowUserControl { get; set; }
        bool allowCameraControl { get; set; }

        void SetPosition(double lng, double lat);
        void GetPosition(out double lng, out double lat);
        void SetPositionAndZoom(double lng, double lat);

        /// <summary>
        /// Adds a new 3D marker on the map.
        /// </summary>
        IMarker AddMarker(Vector2 position, GameObject prefab);
        void RemoveMarker(IMarker marker);

        Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2);
        double DistanceBetweenPointsD(Vector2 point1, Vector2 point2);
        


        /// <summary>
        /// Opent the street map at the current gps position
        /// </summary>
        void ShowStreetMap(System.Action callback);
        /// <summary>
        /// Opens the street map at the given geo coordinates
        /// </summary>
        void ShowStreetMap(double longitude, double latitude, System.Action callback);
        void ShowWorldMap();
        void ShowWorldMap(double longitude, double latitude);

        void HideMap();
        void InitMap();

        bool IsWorld { get; }
    }
}