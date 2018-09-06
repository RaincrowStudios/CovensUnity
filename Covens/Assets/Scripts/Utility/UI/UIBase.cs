using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A base class. It shows and closes
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    [Header("Behavior")]
    public bool m_DisableOnAwake = true;
    public GameObject m_Target;

    [Header("Sound")]
    public string m_ShowSound = "ShowUI";
    public string m_CloseSound = "CloseUI";


    protected bool m_IsVisible = false;
    protected bool m_IsAnimating = false;
    protected RectTransform m_TargetRectTransform;
    



    public bool IsVisible
    {
        get {
            if (m_Target)
                return m_Target.activeSelf;
            return m_IsVisible;
        }
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
        if(m_DisableOnAwake && m_Target)
            m_Target.SetActive(false);
    }

    public virtual void Show()
    {
        if(m_Target)
            m_Target.SetActive(true);
        m_IsVisible = true;
        DoShowAnimation();
        SoundManager.PlayRandomPitch(m_ShowSound);
    }
    public virtual void DoShowAnimation()
    {
        m_IsAnimating = true;
    }
    public virtual void OnShowFinish()
    {
        m_IsAnimating = false;
    }


    public virtual void Close()
    {
        DoCloseAnimation();
        if (IsVisible)
            SoundManager.PlayRandomPitch(m_CloseSound);
    }
    public virtual void DoCloseAnimation()
    {
        m_IsAnimating = true;
    }
    public virtual void OnCloseFinish()
    {
        m_IsAnimating = false;
        m_IsVisible = false;
        if(m_Target)
            m_Target.SetActive(false);
    }


    public void Hide()
    {
        SetActive(false);
    }
    public void SetActive(bool bActive)
    {
        if (m_Target)
            m_Target.SetActive(bActive);
    }


    #region inspector calls

    [ContextMenu("Show")]
    protected void ShowIt()
    {
        Show();
    }
    [ContextMenu("Close")]
    protected void CloseIt()
    {
        Close();
    }
    [ContextMenu("Activate")]
    protected void ActivateIt()
    {
        SetActive(true);
    }
    [ContextMenu("Hide")]
    protected void HideIt()
    {
        Hide();
    }
    #endregion
}