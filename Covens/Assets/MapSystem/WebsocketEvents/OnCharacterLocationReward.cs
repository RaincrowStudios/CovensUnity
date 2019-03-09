using UnityEngine;
using System.Collections;

public static class OnCharacterLocationReward
{
    public static void HandleEvent(WSData data)
    {
        //inform character that data.locationName has rewarded them data.reward of gold
        PlayerDataManager.playerData.gold += data.reward;
        UILocationRewards.Instance.Show(data);
    }
}
