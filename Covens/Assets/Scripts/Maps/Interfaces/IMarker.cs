using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMarker
    {
        GameObject instance { get; set; }
        object customData { get; set; }
        Vector2 position { get; set; }
        float scale { get; set; }
        bool inMapView { get; }
        System.Action<IMarker> OnClick { get; set; }
        void SetRange(int min = int.MinValue, int max = int.MaxValue, int minLimit = 3, int maxLimit = 20);
        void SetPosition(double lng, double lat);
    }
}
