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

        marker.SetWorldPosition(transform.position);
        marker.gameObject.SetActive(true);
        marker.SetAlpha(0);
        MarkerSpawner.UpdateMarker(marker, false, true, MarkerSpawner.m_MarkerScale);
        Debug.Log(marker.gameObject.name + " " + marker.gameObject.transform.localScale);
        marker.SetAlpha(1, 1f);
    }

    public void RemoveMarker()
    {
        if (marker == null)
            return;
        
        //disable interaction wit hit
        marker.interactable = false;

        //animate the marker, then actually despawn it 
        marker.SetAlpha(0, 0, () => MapsAPI.Instance.RemoveMarker(marker));

        marker = null;
    }
}
