using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A basic animated by Tween UI. It can be inherited to whatever you want to change the animation
/// </summary>
public abstract class UIBaseAnimated : MonoBehaviour
{
    public GameObject m_Target;
    public float m_AnimationTime = 0.4f;

    protected bool m_IsVisible = false;
    protected bool m_IsAnimating = false;
    private RectTransform m_TargetRectTransform;



    public RectTransform TargetTransform
    {
        get {
            if(m_TargetRectTransform == null)
            {
                m_TargetRectTransform = m_Target.transform as RectTransform;
            }
            return m_TargetRectTransform;
        }
    }


    private void Reset()
    {
        m_Target = gameObject;
    }

    [ContextMenu("Show")]
    public virtual void Show()
    {
        m_Target.SetActive(true);
        m_IsVisible = true;
        DoShowAnimation();
    }
    public virtual void DoShowAnimation()
    {
        m_IsAnimating = true;
        TargetTransform.localScale = Vector2.zero;
        var pDesc = LeanTween.scale(TargetTransform, Vector2.one, m_AnimationTime);
        pDesc.setEase(LeanTweenType.easeOutBack);
        pDesc.setOnComplete(OnShowFinish);
    }
    public virtual void OnShowFinish()
    {
        m_IsAnimating = false;
    }


    [ContextMenu("Close")]
    public virtual void Close()
    {
        DoCloseAnimation();
    }
    public virtual void DoCloseAnimation()
    {
        m_IsAnimating = true;
        var pDesc = LeanTween.scale(TargetTransform, Vector2.zero, m_AnimationTime);
        pDesc.setEase(LeanTweenType.easeInBack);
        pDesc.setOnComplete(OnCloseFinish);
    }
    public virtual void OnCloseFinish()
    {
        m_IsAnimating = false;
        m_IsVisible = false;
        m_Target.SetActive(false);
    }


    public void Hide()
    {
        SetActive(false);
    }
    public void SetActive(bool bActive)
    {
        m_Target.SetActive(bActive);
    }
}