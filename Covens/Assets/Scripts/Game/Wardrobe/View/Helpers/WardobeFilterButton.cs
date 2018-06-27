using UnityEngine;
using System.Collections;
using System;

public class WardobeFilterButton : UIButton
{
	public EnumEquipmentSlot m_Slot;
    [EnumFlag("Category Flag")]
    public EnumWardrobeCategory m_Category;
    public SelectableItem m_SelectableItem;
    public string m_sSubtitleId;


    WardrobeView m_pWardrobeView;

    public string Subtitle
    {
        get
        {
            return Oktagon.Localization.Lokaki.GetText(m_sSubtitleId);
        }
    }
    void Start()
	{
        m_pWardrobeView = GameObject.FindObjectOfType<WardrobeView>();
        
    }
    
    private void OnEnable()
    {
        SetSelected(false);
    }

    public override void OnClickButton()
    {
        base.OnClickButton();
        m_pWardrobeView.OnClickFilterButton(this);
    }

    public void SetSelected(bool bSelected)
    {
        if(m_SelectableItem != null)
            m_SelectableItem.SetSelected(bSelected);
    }

}

