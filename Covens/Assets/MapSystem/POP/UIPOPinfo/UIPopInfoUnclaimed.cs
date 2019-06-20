using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPopInfoUnclaimed : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Reward;
    [SerializeField] private TextMeshProUGUI m_EnterText;

    [SerializeField] private Button m_EnterBtn;
    [SerializeField] private Button m_OfferBtn;
    [SerializeField] private Button m_CloseBtn;

    private int m_TweenId;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        m_CanvasGroup.gameObject.SetActive(false);
        m_CanvasGroup.alpha = 0;
    }

    public void SetupListeners(UnityAction onEnter, UnityAction onOffer, UnityAction onClose)
    {
        m_EnterBtn.onClick.RemoveAllListeners();
        m_OfferBtn.onClick.RemoveAllListeners();
        m_CloseBtn.onClick.RemoveAllListeners();

        m_EnterBtn.onClick.AddListener(onEnter);
        m_OfferBtn.onClick.AddListener(onOffer);
        m_CloseBtn.onClick.AddListener(onClose);
    }

    public void Show(IMarker marker, Token data)
    {
        IsOpen = true;

        m_Title.text = LocalizeLookUp.GetText("pop_title");
        m_Reward.text = "";

        m_EnterBtn.interactable = false;

        LeanTween.cancel(m_TweenId);
        m_CanvasGroup.gameObject.SetActive(true);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).setEaseOutCubic().uniqueId;
    }

    public void SetupDetails(LocationMarkerDetail data)
    {
        if (!string.IsNullOrEmpty(data.displayName))
            m_Title.text = data.displayName;

        if (data.rewardOn != 0)
            m_Reward.text = LocalizeLookUp.GetText("pop_treasure_time").Replace("{{time}}", Utilities.GetSummonTime(data.rewardOn));
        else
            m_Reward.text = "";

        m_EnterBtn.interactable = data.full == false;

        if (data.full)
            m_EnterText.text = "The Place of Power is full";
        else
            m_EnterText.text = "Enter this Place of Power";
    }

    public void Close(System.Action onComplete = null)
    {
        IsOpen = false;

        if (m_CanvasGroup.gameObject.activeSelf == false)
        {
            onComplete?.Invoke();
            return;
        }

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_CanvasGroup.gameObject.SetActive(false);
                onComplete?.Invoke();
            })
            .uniqueId;
    }
}
