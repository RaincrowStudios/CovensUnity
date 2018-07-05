using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGearsView : UIBaseAnimated
{

    [Header("Item view")]
    public StoreItemView m_ItemView;
    public Button m_ButtonItemView;
    public Text m_TextItemView;

    [Header("Style view")]
    public StoreStyleView m_StyleView;
    public Button m_ButtonStyleView;
    public Text m_TextStyleView;

    [Header("Button behavior")]
    public Color m_ColorEnabled;
    public Color m_ColorDisabled;

    private UIBase m_CurrentUI = null;

    public override void Show()
    {
        Invoke("ShowScheduled", .4f);
    }
    void ShowScheduled()
    {
        base.Show();
        m_ItemView.Setup();
        m_StyleView.Setup();

        ShowItems();
    }

    void SetEnabledButton(bool bEnabled, Text m_Text)
    {
        m_Text.color = bEnabled ? m_ColorEnabled : m_ColorDisabled;
    }

    public void ShowItems()
    {
        if (m_CurrentUI == m_ItemView)
            return;
        m_StyleView.Close();
        m_ItemView.Show();
        SetEnabledButton(false, m_TextStyleView);
        SetEnabledButton(true, m_TextItemView);
        m_CurrentUI = m_ItemView;
    }
    public void ShowStyles()
    {
        if (m_CurrentUI == m_StyleView)
            return;
        m_ItemView.Close();
        m_StyleView.Show();
        SetEnabledButton(true, m_TextStyleView);
        SetEnabledButton(false, m_TextItemView);
        m_CurrentUI = m_StyleView;
    }
}
