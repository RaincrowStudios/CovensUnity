using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Maps
{
    public interface IMarker
    {
        Vector2 position { get; set; }
        float scale { get; set; }
        void SetRange(int min = int.MinValue, int max = int.MaxValue, int minLimit = 3, int maxLimit = 20);
        GameObject instance { get; set; }
        System.Action<IMarker> OnClick { get; set; }
    }
}
