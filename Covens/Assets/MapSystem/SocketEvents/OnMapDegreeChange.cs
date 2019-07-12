using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapDegreeChange
{
    public static void HandleEvent(WSData data)
    {
        if (data.instance == PlayerDataManager.playerData.instance)
        {
            PlayerDataManager.playerData.degree = data.newDegree;
            PlayerManagerUI.Instance.playerDegreeChanged();
            UIDegreeChanged.Instance.Show(data.oldDegree, data.newDegree);
        }
        else
        {
            WitchMarker marker = MarkerManager.GetMarker(data.instance) as WitchMarker;
            if (marker != null)
            {
                Token token = marker.customData as Token;
                if (token != null)
                {
                    token.degree = data.newDegree;
                    marker.SetRingAmount();
                }
            }
        }
        //if (data.instance == MarkerSpawner.instanceID)
        //{
        //    MarkerSpawner.SelectedMarker.degree = data.newDegree;
        //    if (MapSelection.currentView == CurrentView.MapView)
        //    {
        //        //ShowSelectionCard.Instance.ChangeDegree();
        //    }
        //}
    }
}
