using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

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
    private int m_ScaleTweenId;
    private int m_ScaleTweenId2;
    private int m_ScaleTweenId3;


    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CloseButton.onClick.AddListener(Close);
        m_SpiritArt.color = new Color(0, 0, 0, 0);
        m_CanvasGroup.alpha = 0;
        m_Content.transform.localScale = Vector3.one * 0.25f;
        m_RewardLayout.transform.localScale = Vector3.one * 0.25f;
        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ScaleTweenId);
        LeanTween.cancel(m_ScaleTweenId2);
        LeanTween.cancel(m_ScaleTweenId3);


        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void Show(SpiritBanishedHandler.SpiritBanishedEvent data)
    {
        m_SpiritId = data.spirit;
        m_CloseButton.interactable = false;

        //wait for the marker to be despawned
        if (MarkerManager.GetMarker(data.id) != null)
        {
            LeanTween.value(0, 0, 0.2f).setOnComplete(() => Show(data));
            return;
        }

        //wait half second before actualy showing
        LeanTween.value(0, 0, 0.5f).setOnComplete(() =>
        {
            BackButtonListener.AddCloseAction(null);

            DownloadedAssets.GetSprite(data.spirit, (sprite) =>
            {
                BackButtonListener.RemoveCloseAction();

                if (data.spirit != m_SpiritId)
                    return;

                //wait 0.2s before enabling the close button
                LeanTween.value(0, 0, 0).setDelay(0.2f).setOnComplete(() =>
                {
                    BackButtonListener.RemoveCloseAction();
                    BackButtonListener.AddCloseAction(Close);
                    m_CloseButton.interactable = true;
                });

                //setup screen
                SpiritData spiritData = DownloadedAssets.GetSpirit(data.spirit);
                m_SpiritArt.overrideSprite = sprite;
                m_SpiritArt.color = new Color(1, 1, 1, 0.9f);
                m_ExpReward.text = $"+{data.xp} XP";
                m_SilverReward.text = $"+{data.silver} {LocalizeLookUp.GetText("store_silver")}";

                //animate
                LeanTween.cancel(m_TweenId);
                LeanTween.cancel(m_ScaleTweenId);
                LeanTween.cancel(m_ScaleTweenId2);
                LeanTween.cancel(m_ScaleTweenId3);
                m_ScaleTweenId = LeanTween.scale(m_Content.gameObject, Vector3.one * 0.6f, 0.2f).setEaseOutCubic().uniqueId;
                m_ScaleTweenId2 = LeanTween.scale(m_RewardLayout.gameObject, Vector3.one * 2.5f, 0.5f).setEaseInOutCubic().setOnComplete(() =>
                {
                    m_ScaleTweenId3 = LeanTween.scale(m_RewardLayout.gameObject, Vector3.one * 1.5f, 0.3f).uniqueId;
                }).uniqueId;
                m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0.65f, 0.5f).uniqueId;

                m_Canvas.enabled = true;
                m_InputRaycaster.enabled = true;

                BackButtonListener.AddCloseAction(Close);
            });
        });
    }

    public void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_SpiritId = null;
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ScaleTweenId);
        LeanTween.cancel(m_ScaleTweenId2);
        LeanTween.cancel(m_ScaleTweenId3);


        m_ScaleTweenId2 = LeanTween.scale(m_RewardLayout.gameObject, Vector3.one * 0.25f, 0.5f).setEaseInOutCubic().uniqueId;
        m_ScaleTweenId = LeanTween.scale(m_Content.gameObject, Vector3.one * 0.25f, 0.5f).setEaseOutCubic().uniqueId;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.25f).uniqueId;
    }
}
