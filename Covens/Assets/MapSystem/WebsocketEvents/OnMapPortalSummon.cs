using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapPortalSummon
{
    public static event System.Action<string> OnPortalSummoned;

    public static void HandleEvent(WSData data)
    {
        IMarker marker = MarkerSpawner.GetMarker(data.instance);
        MarkerSpawner.DeleteMarker(data.instance);

        OnPortalSummoned?.Invoke(data.instance);

        //if (MarkerSpawner.instanceID == data.instance && MapSelection.currentView == CurrentView.IsoView)
        //{
        //    IsoPortalUI.instance.Summoned();
        //    MovementManager.Instance.RemoveMarkerIso(data.instance);
        //}
        //else
        //{
        //    if (MapSelection.currentView == CurrentView.IsoView || MapSelection.currentView == CurrentView.TransitionView)
        //        MovementManager.Instance.RemoveMarkerIso(data.instance);
        //    else
        //        MovementManager.Instance.RemoveMarker(data.instance);
        //}
    }
}
