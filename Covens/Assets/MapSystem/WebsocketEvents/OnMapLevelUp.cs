using UnityEngine;
using System.Collections;

public static class OnMapLevelUp
{
    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;
        if (data.instance == player.instance)
        {
            player.xpToLevelUp = data.xpToLevelUp;
            player.level = data.newLevel;
            player.baseEnergy = data.newBaseEnergy;
            PlayerManagerUI.Instance.playerlevelUp();
            PlayerManagerUI.Instance.UpdateEnergy();
            SoundManagerOneShot.Instance.PlayLevel();
        }
        if (data.instance == MarkerSpawner.instanceID)
        {
            MarkerSpawner.SelectedMarker.level = data.newLevel;
            if (MapSelection.currentView == CurrentView.MapView)
            {
                //ShowSelectionCard.Instance.ChangeLevel();
            }
            else
            {
                IsoTokenSetup.Instance.ChangeLevel();
            }
        }
    }
}
