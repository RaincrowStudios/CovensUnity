using UnityEngine;
using System.Collections;

public static class OnMapDegreeChange
{
    public static void HandleEvent(WSData data)
    {
        if (data.instance == PlayerDataManager.playerData.instance)
        {
            PlayerDataManager.playerData.degree = data.newDegree;
            PlayerManagerUI.Instance.playerDegreeChanged();

            if (data.oldDegree < data.newDegree)
            {
                SoundManagerOneShot.Instance.PlayWhite();
            }
            else
            {
                SoundManagerOneShot.Instance.PlayShadow();
            }

        }
        if (data.instance == MarkerSpawner.instanceID)
        {
            MarkerSpawner.SelectedMarker.degree = data.newDegree;
            if (MapSelection.currentView == CurrentView.MapView)
            {
                //ShowSelectionCard.Instance.ChangeDegree();
            }
        }
    }
}
