using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreView : UIBaseAnimated
{
    public UIBase m_ViewGear;
    public UIBase m_ViewSilver;
    public UIBase m_ViewElixirs;
    public UIBase m_ViewWheel;

    public UIBase m_LastView;



    public void ShowTabGear()
    {
        ShowTab(m_ViewGear);
    }
    public void ShowTabSilver()
    {
        ShowTab(m_ViewSilver);
    }
    public void ShowTabElixirs()
    {
        ShowTab(m_ViewElixirs);
    }
    public void ShowTabWheel()
    {
        ShowTab(m_ViewWheel);
    }

    public override void Show()
    {
        base.Show();
        ShowTabGear();
    }

    public void ShowTab(UIBase pBase)
    {
        if (m_LastView != null && m_LastView.IsVisible)
            m_LastView.Close();
        pBase.Show();
        m_LastView = pBase;
    }

}