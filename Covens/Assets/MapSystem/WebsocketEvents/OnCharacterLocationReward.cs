using UnityEngine;
using Raincrow.Rewards;

public static class OnCharacterLocationReward
{
    public static void HandleEvent(WSData data)
    {
        //inform character that data.locationName has rewarded them data.reward of gold
        RewardData locationReward = JsonUtility.FromJson<RewardData>(data.json);
        PlayerDataManager.playerData.gold += locationReward.gold;
        PlayerDataManager.playerData.silver += locationReward.silver;
        UILocationRewards.Instance.Show(locationReward, data.location);
    }
}
