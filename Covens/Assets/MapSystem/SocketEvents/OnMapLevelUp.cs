using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public static class OnMapLevelUp
{
    public static void HandleEvent(WSData data)
    {
        PlayerDataDetail player = PlayerDataManager.playerData;
        if (data.instance == player.instance)
        {
            if (data.newLevel == 3)
                AppsFlyerAPI.ReachedLevelThree();
            player.xpToLevelUp = data.xpToLevelUp;
            player.level = data.newLevel;
            player.baseEnergy = data.newBaseEnergy;
            PlayerManagerUI.Instance.playerlevelUp();
            PlayerManagerUI.Instance.UpdateEnergy();
            UILevelUp.Instance.Show();
        }
        else
        {
            IMarker marker = MarkerManager.GetMarker(data.instance);

            if (marker != null)
            {
                WitchToken token = marker.customData as WitchToken;
                if (token != null)
                {
                    token.level = data.newLevel;
                }
            }
        }

        //if (data.instance == MarkerSpawner.instanceID)
        //{
        //    MarkerSpawner.SelectedMarker.level = data.newLevel;
        //    if (MapSelection.currentView == CurrentView.MapView)
        //    {
        //        //ShowSelectionCard.Instance.ChangeLevel();
        //    }
        //    else
        //    {
        //        IsoTokenSetup.Instance.ChangeLevel();
        //    }
        //}
    }
}
