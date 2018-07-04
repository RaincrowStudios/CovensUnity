using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGearsView : UIBaseAnimated
{

    [Header("Item view")]
    public StoreItemView m_ItemView;
    public Button m_ButtonItemView;

    [Header("Style view")]
    public StoreStyleView m_StyleView;
    public Button m_ButtonStyleView;

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



    public void ShowItems()
    {
        m_StyleView.Close();
        m_ItemView.Show();
    }
    public void ShowStyles()
    {
        m_ItemView.Close();
        m_StyleView.Show();
    }
}
