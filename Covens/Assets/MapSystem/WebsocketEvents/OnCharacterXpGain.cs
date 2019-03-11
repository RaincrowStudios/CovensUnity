using UnityEngine;
using System.Collections;

public static class OnCharacterXpGain 
{
    public static void HandleEvent(WSData data)
    {
        PlayerDataManager.playerData.xp = data.newXp;
        PlayerManagerUI.Instance.setupXP();
    }
}
