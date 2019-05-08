using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class UIPOPOptions : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_PanelRect;

    [SerializeField] private Button m_OfferingButton;
    [SerializeField] private Button m_ChallengeButton;
    [SerializeField] private Button m_CancelButton;

    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_OfferingText;
    [SerializeField] private TextMeshProUGUI m_ChallengeText;

    private int m_TweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_PanelRect.localPosition = new Vector2(0, -m_PanelRect.sizeDelta.y);
    }

    public void Show(MarkerDataDetail locationData, UnityAction onSelectOffering, UnityAction onSelectChallenge, UnityAction onSelectCancel)
    {
        m_CancelButton.onClick.RemoveAllListeners();
        m_ChallengeButton.onClick.RemoveAllListeners();
        m_OfferingButton.onClick.RemoveAllListeners();

        m_CancelButton.onClick.AddListener(onSelectCancel);
        m_ChallengeButton.onClick.AddListener(onSelectChallenge);
        m_OfferingButton.onClick.AddListener(onSelectOffering);

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.localPosition = new Vector2(0, Mathf.Lerp(-m_PanelRect.sizeDelta.y, 0, t));
            })
            .setEaseOutCubic()
            .uniqueId;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.localPosition = new Vector2(0, Mathf.Lerp(-m_PanelRect.sizeDelta.y, 0, t));
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_InputRaycaster.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }
}
