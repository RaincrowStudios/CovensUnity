using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIInfoPanel : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] protected GraphicRaycaster m_InputRaycaster;
    [SerializeField] protected CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;


    private Vector3 m_PreviousMapPosition;
    protected Vector3 previousMapPosition
    {
        get
        {
            if (BanishManager.isBind)
                return Vector3.zero;
            return m_PreviousMapPosition;
        }
        set
        {
            m_PreviousMapPosition = value;
        }
    }

    protected int m_TweenId;

    protected bool m_IsShowing;

    protected virtual void Awake()
    {
        if (m_Panel)
            m_Panel.anchoredPosition = new Vector2(m_Panel.sizeDelta.x, 0);

        m_CanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
    }

    protected virtual void Show()
    {
        m_IsShowing = true;
        ReOpen();
    }

    public virtual void Close()
    {
        m_IsShowing = false;
        Hide();
    }

    /// <summary>
    /// Animate and enable the canvas and panel
    /// </summary>
    public virtual void ReOpen()
    {
        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;
        ReOpenAnimation();
    }

    protected virtual void ReOpenAnimation()
    {
        System.Action<float> onUpdate;
        if (m_Panel)
            onUpdate = t =>
            {
                m_Panel.anchoredPosition = new Vector2((1 - t) * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = t;
            };
        else
            onUpdate = t =>
            {
                m_CanvasGroup.alpha = t;
            };

        //animate
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate(onUpdate)
            .setEaseOutCubic()
            .uniqueId;
    }

    /// <summary>
    /// Animate and disable the canvas and panel
    /// </summary>
    public virtual void Hide()
    {
        System.Action<float> onUpdate;
        if (m_Panel)
            onUpdate = t =>
            {
                m_Panel.anchoredPosition = new Vector2(t * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = 1 - t;
            };
        else
            onUpdate = t =>
            {
                m_CanvasGroup.alpha = 1 - t;
            };

        m_InputRaycaster.enabled = false;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate(onUpdate)
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }
}
