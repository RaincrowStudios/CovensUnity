using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWheelItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CanvasGroup m_pCanvasGroup;
    [SerializeField] private RectTransform m_pRectTransform;

    private int m_iFadeTweenId;
    private int m_iScaleTweenId;

    public System.Action onClick { get; set; }
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //onClick?.Invoke();
        if (onClick != null)
            onClick.Invoke();
    }

    public virtual void Setup(object data) { }
    
    public void FadeContent(float alpha, float duration, float delay = 0, LeanTweenType easeType = LeanTweenType.notUsed, System.Action onComplete = null)
    {
        LeanTween.cancel(m_iFadeTweenId);
        m_iFadeTweenId = LeanTween.value(m_pCanvasGroup.alpha, alpha, duration)
            .setOnUpdate((float value) =>
            {
                m_pCanvasGroup.alpha = value;
            })
            .setOnComplete(onComplete)
            .setEase(easeType)
            .setDelay(delay)
            .uniqueId;
    }

    public void ScaleContent(float scale, float duration, float delay = 0, LeanTweenType easeType = LeanTweenType.notUsed, System.Action onComplete = null)
    {
        LeanTween.cancel(m_iScaleTweenId);
        m_iScaleTweenId = LeanTween.value(m_pCanvasGroup.transform.localScale.x, scale, duration)
            .setOnUpdate((float value) =>
            {
                m_pCanvasGroup.transform.localScale = new Vector2(value, value);
            })
            .setOnComplete(onComplete)
            .setEase(easeType)
            .setDelay(0)
            .uniqueId;
    }

    public void SetPivot(float x)
    {
        m_pRectTransform.pivot = new Vector2(x, m_pRectTransform.pivot.y);
    }
}
