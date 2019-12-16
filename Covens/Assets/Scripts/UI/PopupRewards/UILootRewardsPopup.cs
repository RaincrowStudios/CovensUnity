using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

public class UILootRewardsPopup : UIRewardsPopup
{
    [Space, Header("UILootRewardsPopup")]
    [SerializeField] private Image m_Portrait;
    [SerializeField] private TextMeshProUGUI m_BossName;
    [SerializeField] private TextMeshProUGUI m_ChestTier;

    public static UILootRewardsPopup Instantiate()
    {
        return Instantiate(Resources.Load<UILootRewardsPopup>("UI/LootRewardsPopup"));
    }

    public void Show(CollectLootHandler.EventData data)
    {
        DownloadedAssets.GetSprite(data.bossId + "_portrait", m_Portrait);
        m_BossName.text = LocalizeLookUp.GetSpiritName(data.bossId);
        m_ChestTier.text = LocalizeLookUp.GetText(data.type);

        base.Show(
            ingredients: data.collectibles,
            cosmetics: data.cosmetics,
            gold: data.gold,
            silver: data.silver,
            xp: data.xp,
            power: 0,
            resilience: 0
        );
    }
}
