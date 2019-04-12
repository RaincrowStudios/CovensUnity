using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMaps
    {
        Transform transform { get; }

        Camera camera { get; }

        /// <summary>
        /// Current zoom
        /// </summary>
        float zoom { get; }
        float normalizedZoom { get; }

        /// <summary>
        /// Coordinates of the center point of the map
        /// </summary>
        Vector2 position { get; set; }

        /// <summary>
        /// Current GPS coordinates.
        /// </summary>
        Vector2 physicalPosition { get; }
        
        bool allowControl { get; set; }

        void SetPosition(double lng, double lat);
        void GetPosition(out double lng, out double lat);

        Vector3 GetWorldPosition();
        Vector3 GetWorldPosition(double lng, double lat);

        /// <summary>
        /// Adds a new 3D marker on the map.
        /// </summary>
        IMarker AddMarker(Vector2 position, GameObject prefab);
        void RemoveMarker(IMarker marker);

        Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2);
        double DistanceBetweenPointsD(Vector2 point1, Vector2 point2);

        void InitMap();
        void InitMap(double longitude, double latitude, float zoom, System.Action callback, bool animate);

        void HideMap(bool hide);

        System.Action OnChangePosition { get; set; }
        System.Action OnChangeZoom { get; set; }
    }
}