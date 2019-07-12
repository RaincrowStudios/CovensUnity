using UnityEngine;
using Raincrow.Rewards;

public static class OnCharacterLocationReward
{
    public static void HandleEvent(WSData data)
    {
        //inform character that data.locationName has rewarded them data.reward of gold
        Debug.Log(data.json);
        RewardData locationReward = JsonUtility.FromJson<RewardData>(data.json);
        PlayerDataManager.playerData.gold += locationReward.gold;
        PlayerDataManager.playerData.silver += locationReward.silver;
        PlayerManagerUI.Instance.UpdateDrachs();
        if (DownloadedAssets.localizedText.ContainsKey("pop_reward_notification"))
            PlayerNotificationManager.Instance.ShowNotificationPOP(DownloadedAssets.localizedText["pop_reward_notification"].Replace("{{gold}}", locationReward.gold.ToString()).Replace("{{silver}}", locationReward.silver.ToString()));
        else
            PlayerNotificationManager.Instance.ShowNotificationPOP($"Your Places of Power rewarded you <color=#FF9F00><b>{locationReward.gold}</b> Gold</color> and <color=#FFFFFF><b>{locationReward.silver}</b> Silver</color>.");
        //UILocationRewards.Instance.Show(locationReward, data.location);
        Debug.Log("onCharacterLocationReward");
    }
}
