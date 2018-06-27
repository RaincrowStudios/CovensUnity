using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A basic animated by Tween UI. It can be inherited to whatever you want to change the animation
/// </summary>
public abstract class UIBaseAnimated : UIBase
{
    [Header("Animation")]
    public float m_AnimationTime = 0.4f;


    private LTDescr m_pcurrentAnimation;

    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
        if (m_pcurrentAnimation != null)
            LeanTween.cancel(TargetTransform);
        TargetTransform.localScale = Vector2.zero;

        var pDesc = LeanTween.scale(TargetTransform, Vector2.one, m_AnimationTime);
        pDesc.setEase(LeanTweenType.easeOutBack);
        pDesc.setOnComplete(OnShowFinish);
        m_pcurrentAnimation = pDesc;
    }
    public override void OnShowFinish()
    {
        base.OnShowFinish();
        m_IsAnimating = false;
    }


    public override void DoCloseAnimation()
    {
        base.DoCloseAnimation();
        if (m_pcurrentAnimation != null)
            LeanTween.cancel(TargetTransform);

        var pDesc = LeanTween.scale(TargetTransform, Vector2.zero, m_AnimationTime);
        pDesc.setEase(LeanTweenType.easeInBack);
        pDesc.setOnComplete(OnCloseFinish);
        m_pcurrentAnimation = pDesc;
    }
}