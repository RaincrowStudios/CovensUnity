using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpiritBanished : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private Image m_SpiritArt;
    [SerializeField] private Button m_CloseButton;

    [Space()]
    [SerializeField] private RectTransform m_RewardLayout;
    [SerializeField] private TextMeshProUGUI m_SilverReward;
    [SerializeField] private TextMeshProUGUI m_ExpReward;

    private static UISpiritBanished m_Instance;
    public static UISpiritBanished Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpiritBanished>("UISpiritBanished"));
            return m_Instance;
        }
    }

    public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return string.IsNullOrEmpty(m_Instance.m_SpiritId) == false;
        }
    }

    private string m_SpiritId;
    private int m_TweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CloseButton.onClick.AddListener(Close);
        m_SpiritArt.color = new Color(0, 0, 0, 0);
        m_CanvasGroup.alpha = 0;

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        LeanTween.cancel(m_TweenId);
        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void Show(string spiritId, bool isInPop = false)
    {
        m_SpiritId = spiritId;
        m_CloseButton.interactable = false;

        float CanvasAlpha = 0.65f;
        if (isInPop)
            CanvasAlpha = 1f;

        DownloadedAssets.GetSprite(spiritId, (sprite) =>
        {
            if (spiritId != m_SpiritId)
                return;

            LeanTween.value(0, 0, 0).setDelay(0.2f).setOnComplete(() => { m_CloseButton.interactable = true; });

            SpiritData data = DownloadedAssets.GetSpirit(spiritId);
            long exp = PlayerDataManager.spiritRewardExp[data.tier - 1];
            int silver = PlayerDataManager.spiritRewardSilver[data.tier - 1];

            m_SpiritArt.overrideSprite = sprite;
            m_SpiritArt.color = new Color(1, 1, 1, 0.9f);
            m_ExpReward.text = $"+{exp} XP";
            m_SilverReward.text = $"+{silver} {LocalizeLookUp.GetText("store_silver")}";

            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.value(0, CanvasAlpha, 0.7f).setDelay(0.5f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_CanvasGroup.alpha = t;
                    if (isInPop == false)
                    {
                        m_Content.transform.localScale = new Vector3(t, t, t);
                    }
                    else
                    {
                        m_Content.transform.localScale = new Vector3(t * 0.8f, t * 0.8f, t * 0.8f);
                    }
                })
                .uniqueId;

            m_Canvas.enabled = true;
            m_InputRaycaster.enabled = true;
        });
    }

    public void Close()
    {
        m_SpiritId = null;
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0.65f, 0, 0.8f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_Content.transform.localScale = new Vector3(t, t, t);
            })
            .setOnComplete(() => { m_Canvas.enabled = false; })
            .uniqueId;
    }
}
