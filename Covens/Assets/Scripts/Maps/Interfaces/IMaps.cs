using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMaps
    {
        Transform transform { get; }
        Vector2 position { get; set; }
        Vector2 physicalPosition { get; set; }
        void SetPosition(double lng, double lat);
        void GetPosition(out double lng, out double lat);
        void SetPositionAndZoom(double lng, double lat, int zoom = 0);
        IMarker AddMarker(Vector2 position, GameObject prefab);
        void RemoveMarker(IMarker marker);
        double DistanceBetweenPointsD(Vector2 point1, Vector2 point2);

        System.Action OnChangePosition { get; set; }
        System.Action OnMapUpdated { get; set; }
    }
}