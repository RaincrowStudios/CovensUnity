using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILocationRewards : MonoBehaviour
{
    public static UILocationRewards Instance { get; private set; }

    [SerializeField] private CanvasGroup m_pContent;
    [SerializeField] private GameObject m_pGoldReward;
    [SerializeField] private Button m_pCloseButton;

    [Header("Content")]
    [SerializeField] private TextMeshProUGUI m_pTitleText;
    [SerializeField] private TextMeshProUGUI m_pDescriptionText;
    [SerializeField] private TextMeshProUGUI m_pGoldRewardText;

    private int m_iFadeTweenId;
    private int m_iScaleTweenId;
    private List<WSData> m_lScheduleRewards = new List<WSData>();

    public bool isShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
        m_pCloseButton.onClick.AddListener(OnClickClose);

        m_pContent.alpha = 0;
        m_pContent.gameObject.SetActive(false);
        m_pContent.interactable = false;
        m_pContent.transform.localScale = new Vector2(1, 0);
    }

    public void Show(WSData data)
    {
        if (isShowing)
        {
            //store the data to show it after closing the current reward popup
            m_lScheduleRewards.Add(data);
            return;
        }

        isShowing = true;
        m_pContent.gameObject.SetActive(true);

        m_pGoldReward.SetActive(data.reward > 0);
        if (m_pGoldReward.activeSelf)
            m_pGoldRewardText.text = data.reward.ToString();
        m_pTitleText.text = data.location;

        Fade(1f, 0.5f);
        Scale(Vector2.one, 0.25f, () => m_pContent.interactable = true);
    }

    public void Close()
    {
        m_pContent.interactable = false;

        Scale(new Vector2(1, 0), 0.5f);
        Fade(0, 0.25f,
            () => {
                isShowing = false;
                m_pContent.gameObject.SetActive(false);
                if (m_lScheduleRewards.Count > 0)
                {
                    WSData pData = m_lScheduleRewards[0];
                    m_lScheduleRewards.RemoveAt(0);
                    Show(pData);
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
