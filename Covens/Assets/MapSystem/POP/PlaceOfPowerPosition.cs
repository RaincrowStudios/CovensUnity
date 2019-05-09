using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPowerPosition : MonoBehaviour
{
    public IMarker marker { get; private set; }
    
    public void AddMarker(IMarker marker)
    {
        this.marker = marker;


        marker.SetAlpha(0);
        marker.SetAlpha(1, 1);
    }

    public void RemoveMarker()
    {
        if (marker == null)
            return;

        marker.SetAlpha(1);
        marker = null;

    }
}
