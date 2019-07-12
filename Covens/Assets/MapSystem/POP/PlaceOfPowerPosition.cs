using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPowerPosition : MonoBehaviour
{
    public IMarker marker { get; set; }
    
    public void AddMarker(IMarker marker)
    {
        this.marker = marker;

        if (marker.type == MarkerSpawner.MarkerType.CHARACTER)
            (marker as WitchMarker).RemoveImmunityFX();

        marker.SetWorldPosition(transform.position);
        marker.gameObject.SetActive(true);
        marker.SetAlpha(0);
        MarkerSpawner.UpdateMarker(marker, false, true, MarkerSpawner.m_MarkerScale);
        marker.SetAlpha(1, 1f);
    }
}
