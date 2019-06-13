using Raincrow.Rewards;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILocationRewards : MonoBehaviour
{
    private struct LocationRewardData
    {
        public string Location { get; set; }
        public RewardData RewardData { get; set; }
    }

    public static UILocationRewards Instance { get; private set; }

    [SerializeField] private CanvasGroup m_pContent;
    [SerializeField] private GameObject m_pGoldReward;
    [SerializeField] private Button m_pCloseButton;

    [Header("Content")]
    [SerializeField] private Text m_pTitleText;
    [SerializeField] private Text m_pDescriptionText;
    [SerializeField] private Text m_pGoldRewardText;

    private int m_iFadeTweenId;
    private int m_iScaleTweenId;
    private List<LocationRewardData> m_lScheduleRewards = new List<LocationRewardData>();

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
        m_pCloseButton.onClick.AddListener(OnClickClose);

        m_pContent.alpha = 0;
        m_pContent.gameObject.SetActive(false);
        m_pContent.interactable = false;
        m_pContent.transform.localScale = new Vector2(1, 0);
    }

    public void Show(RewardData rewardData, string location)
    {
        if (IsShowing)
        {
            //store the data to show it after closing the current reward popup

            LocationRewardData locationRewardData = new LocationRewardData
            {
                Location = location,
                RewardData = rewardData
            };

            m_lScheduleRewards.Add(locationRewardData);
            return;
        }

        IsShowing = true;
        m_pContent.gameObject.SetActive(true);

        m_pGoldReward.SetActive(rewardData.gold > 0);
        if (m_pGoldReward.activeSelf)
        {
            m_pGoldRewardText.text = rewardData.gold.ToString();
        }
        m_pTitleText.text = location;

        Fade(1f, 0.5f);
        Scale(Vector2.one, 0.25f, () => m_pContent.interactable = true);
    }

    public void Close()
    {
        m_pContent.interactable = false;

        Scale(new Vector2(1, 0), 0.5f);
        Fade(0, 0.25f,
            () => {
                IsShowing = false;
                m_pContent.gameObject.SetActive(false);
                if (m_lScheduleRewards.Count > 0)
                {
                    LocationRewardData locationRewardData = m_lScheduleRewards[0];
                    m_lScheduleRewards.RemoveAt(0);
                    Show(locationRewardData.RewardData, locationRewardData.Location);
                }
            }
        );
    }
    
    private void OnClickClose()
    {
        Close();
    }

    private void Scale(Vector2 scale, float time, System.Action onComplete = null, LeanTweenType easeType = LeanTweenType.easeOutCubic)
    {
        LeanTween.cancel(m_iScaleTweenId);
        m_iScaleTweenId = LeanTween.scale(m_pContent.gameObject, scale, time)
            .setEase(easeType)
            .setOnComplete(onComplete)
            .uniqueId;
    }

    private void Fade(float alpha, float time, System.Action onComplete = null, LeanTweenType easeType = LeanTweenType.easeOutCubic)
    {
        LeanTween.cancel(m_iFadeTweenId);
        m_iFadeTweenId = LeanTween.value(m_pContent.alpha, alpha, time)
            .setEase(easeType)
            .setOnUpdate((float t) => m_pContent.alpha = t)
            .setOnComplete(onComplete)
            .uniqueId;
    }
}
