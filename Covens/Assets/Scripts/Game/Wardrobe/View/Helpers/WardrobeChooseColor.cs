using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// shooses between 3 colored items
/// </summary>
public class WardrobeChooseColor : UIBaseAnimated
{
    

    [Header("Item Buttons")]
    public GameObject m_ChooseColorRoot;
    public WardrobeItemButton m_WhiteButton;
    public WardrobeItemButton m_GreyButton;
    public WardrobeItemButton m_ShadowButton;

    public event Action<WardrobeItemButton> OnClickEvent;
    private CharacterView m_Character;


    protected WardrobeController Controller
    {
        get { return WardrobeController.Instance; }
    }
    public CharacterView CharacterView
    {
        get { return m_Character; }
        set { m_Character = value; }
    }
    protected CharacterControllers CharacterController
    {
        get { return m_Character.m_Controller; }
    }


    

    public void Show(WardrobeView.GroupedWardrobeItemModel pGroupedItem)
    {
        OnClickEvent = null;
        int i = 0;
        foreach (var pItem in pGroupedItem.m_Items)
        {
            WardrobeItemButton pButton = null;
            switch (pItem.AlignmentEnum)
            {
                case EnumAlignment.White: pButton = m_WhiteButton; break;
                case EnumAlignment.Gray: pButton = m_GreyButton; break;
                case EnumAlignment.Shadow: pButton = m_ShadowButton; break;
            }
            if (pButton != null)
            {
                pButton.Setup(pItem);
                pButton.OnClickEvent += Button_OnClickEvent;
                pButton.SetEquipped(CharacterController.IsEquipped(pItem));
            }
        }
        base.Show();
    }

    private void Button_OnClickEvent(WardrobeItemButton obj)
    {
        if (OnClickEvent != null)
            OnClickEvent(obj);
    }
}