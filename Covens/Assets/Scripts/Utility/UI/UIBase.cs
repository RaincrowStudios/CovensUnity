using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A base class. It shows and closes
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    public bool m_DisableOnAwake = true;
    public GameObject m_Target;

    protected bool m_IsVisible = false;
    protected bool m_IsAnimating = false;
    protected RectTransform m_TargetRectTransform;




    public bool IsVisible
    {
        get { return m_Target.activeSelf; }
    }
    public bool IsAnimating
    {
        get { return m_IsAnimating; }
    }
    public RectTransform TargetTransform
    {
        get
        {
            if (m_TargetRectTransform == null)
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
    protected virtual void Awake()
    {
        if(m_DisableOnAwake)
            m_Target.SetActive(false);
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