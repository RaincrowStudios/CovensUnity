using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPOPBattle : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_PanelRect;

    [SerializeField] private Button m_LeaveButton;

    private int m_TweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_PanelRect.anchoredPosition = new Vector2(0, -m_PanelRect.sizeDelta.y);

        m_LeaveButton.onClick.AddListener(OnClickLeave);
    }

    public void Open()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_Canvas.enabled = true;
                m_InputRaycaster.enabled = true;
            })
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.anchoredPosition = new Vector2(0, Mathf.Lerp(m_PanelRect.sizeDelta.y, 0, t));
            })
            .uniqueId;
    }

    public void Close()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.anchoredPosition = new Vector2(0, Mathf.Lerp(m_PanelRect.sizeDelta.y, 0, t));
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_InputRaycaster.enabled = false;
            })
            .uniqueId;
    }

    private void OnClickLeave()
    {
        PlaceOfPower.LeavePoP();
    }
}
