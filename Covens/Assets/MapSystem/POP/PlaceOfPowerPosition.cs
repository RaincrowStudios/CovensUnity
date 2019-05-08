using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPowerPosition : MonoBehaviour
{
    public IMarker marker { get; private set; }

    private int m_TweenId;

    public void AddMarker(IMarker marker)
    {
        this.marker = marker;

        LeanTween.cancel(m_TweenId);

        marker.SetCharacterAlpha(0);
        m_TweenId = LeanTween.value(0, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                marker.SetAlpha(t);
            })
            .uniqueId;
    }

    public void RemoveMarker()
    {
        if (marker == null)
            return;

        IMarker aux = marker;
        marker = null;

        LeanTween.cancel(m_TweenId);

        m_TweenId = LeanTween.value(1, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                aux.SetAlpha(t);
            })
            .uniqueId;
    }
}
