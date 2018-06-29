using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreView : UIBaseAnimated
{
    public UIBase m_ViewGear;
    public StoreGenericView m_ViewGeneric;
    public UIBase m_ViewWheel;
    private UIBase m_LastView;

    [Header("Polish")]
    public GameObject m_Fortuna;

    [Header("Shop configs")]
    public EnumStoreType[] m_SilverFilter;
    public EnumStoreType[] m_ElixirFilter;


    public void ShowTabGear()
    {
        ShowTab(m_ViewGear);
    }
    public void ShowTabSilver()
    {
        m_ViewGeneric.SetupType(m_SilverFilter);
        ShowTab(m_ViewGeneric);
    }
    public void ShowTabElixirs()
    {
        m_ViewGeneric.SetupType(m_ElixirFilter);
        ShowTab(m_ViewGeneric);
    }
    public void ShowTabWheel()
    {
        ShowTab(m_ViewWheel);
    }

    public override void Show()
    {
        base.Show();
        ShowTabWheel();

        m_Fortuna.transform.localRotation = Quaternion.Euler(0, 0, -90);
        LeanTween.rotateLocal(m_Fortuna, new Vector3(0, 0, 0), .4f).setEase(LeanTweenType.easeOutBack).setDelay(.3f);
    }

    public override void Close()
    {
        base.Close();
        if (m_LastView != null && m_LastView.IsVisible)
            m_LastView.Close();
        m_LastView = null;
    }
    public override void OnCloseFinish()
    {
        base.OnCloseFinish();
        if(m_ViewGear != null) m_ViewGear.Hide();
        if (m_ViewGeneric != null) m_ViewGeneric.Hide();
        if (m_ViewWheel != null) m_ViewWheel.Hide();
        if (m_LastView != null) m_LastView.Hide();
    }

    public void ShowTab(UIBase pBase)
    {
        if (!IsVisible)
            Show();
        pBase.Show();
        if (m_LastView != null)
            m_LastView.Close();
        m_LastView = pBase;
    }

}