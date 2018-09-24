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
    public float m_AnimationDelay = 0f;
	public CanvasGroup CG;
	public LeanTweenType tweenType = LeanTweenType.easeInOutSine;

    protected LTDescr m_pCurrentAnimation;

    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
        if (m_pCurrentAnimation != null)
            LeanTween.cancel(TargetTransform);
        TargetTransform.localScale = Vector2.zero;

		var pDesc = LeanTween.scale(TargetTransform, Vector2.one, m_AnimationTime).setDelay(m_AnimationDelay);
		
		pDesc.setEase(tweenType);

		LeanTween.value(0, 1, m_AnimationTime).setOnUpdate((float fValue) =>
			{
				CG.alpha = fValue;
			});

        pDesc.setOnComplete(OnShowFinish);
        m_pCurrentAnimation = pDesc;
    }
    public override void OnShowFinish()
    {
        base.OnShowFinish();
        m_IsAnimating = false;
    }


    public override void DoCloseAnimation()
    {
        base.DoCloseAnimation();
        if (m_pCurrentAnimation != null)
            LeanTween.cancel(TargetTransform);

        var pDesc = LeanTween.scale(TargetTransform, Vector2.zero, m_AnimationTime*.75f);
		pDesc.setEase(tweenType);

		LeanTween.value(1, 0,m_AnimationTime*.5f).setOnUpdate((float fValue) =>
			{
				CG.alpha = fValue;
			});

        pDesc.setOnComplete(OnCloseFinish);
        m_pCurrentAnimation = pDesc;
    }
}