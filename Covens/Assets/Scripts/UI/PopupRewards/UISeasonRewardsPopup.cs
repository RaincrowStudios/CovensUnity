using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

public class UISeasonRewardsPopup : UIRewardsPopup
{
    [Space, Header("UISeasonRewardsPopup")]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;

    public static bool IsOpen { get; private set; }

    public static UISeasonRewardsPopup Instantiate()
    {
        return Instantiate(Resources.Load<UISeasonRewardsPopup>("UI/SeasonRewardsPopup"));
    }

    public void Show(SeasonRewardHandler.EventData data)
    {
        //wait for previous reward popup to close before showing this
        if (IsOpen)
        {
            LeanTween.value(0, 0, 0).setDelay(0.1f).setOnComplete(() => Show(data));
            return;
        }

        IsOpen = true;

        int tribunal = PlayerDataManager.tribunal - 1;

        if (tribunal < 0)
            tribunal = 3;

        if (tribunal == 2)
            m_Subtitle.text = LocalizeLookUp.GetText("summer_tribunal_upper");
        else if (PlayerDataManager.tribunal == 1)
            m_Subtitle.text = LocalizeLookUp.GetText("spring_tribunal_upper");
        else if (PlayerDataManager.tribunal == 3)
            m_Subtitle.text = LocalizeLookUp.GetText("autumn_tribunal_upper");
        else
            m_Subtitle.text = LocalizeLookUp.GetText("winter_tribunal_upper");

        m_Title.text = LocalizeLookUp.GetText("ftf_reward_upper") + ": " + (data.coven ? LocalizeLookUp.GetText("leaderboard_top_coven") : LocalizeLookUp.GetText("leaderboard_top_player"));
        
        base.Show(
            ingredients: data.items,
            cosmetics: data.cosmetics,
            gold: data.gold,
            silver: data.silver,
            xp: 0,
            power: data.power,
            resilience: data.resilience
        );
    }

    protected override void OnClose()
    {
        IsOpen = false;
    }
}
