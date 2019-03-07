using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIInfoPanel : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;

    private int m_TweenId;

    public bool IsShowing { get { return m_InputRaycaster.enabled; } }

    protected virtual void Awake()
    {
        m_Panel.anchoredPosition = new Vector2(m_Panel.sizeDelta.x, 0);
        m_CanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
    }

    /// <summary>
    /// Animate and enable the canvas and panel
    /// </summary>
    public virtual void ReOpen()
    {
        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;

        //animate
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_Panel.anchoredPosition = new Vector2((1 - t) * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    /// <summary>
    /// Animate and disable the canvas and panel
    /// </summary>
    public void Close()
    {
        if (m_InputRaycaster.enabled == false)
            return;

        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_Panel.anchoredPosition = new Vector2(t * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = 1 - t;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }
}
