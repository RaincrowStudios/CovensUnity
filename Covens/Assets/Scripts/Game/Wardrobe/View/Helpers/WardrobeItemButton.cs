using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WardrobeItemButton : UIButton
{
    public event Action<WardrobeItemButton> OnClickEvent;


    public TextMeshProUGUI title;
	public Image icon;
    public SelectableItem m_SelectableItem;
    public GameObject m_goNewFlag;
    public GameObject m_goConflicts;
    public Sprite m_DefaultIcon;

    [Header("Color icons")]
    public GameObject m_WhiteIcon;
    public GameObject m_GrayIcon;
    public GameObject m_ShadowIcon;

    private WardrobeView.GroupedWardrobeItemModel m_pWardrobeGroupedItemModel;
    private WardrobeItemModel m_pWardrobeItemModel;
    private bool m_bIsGrouped = false;
    private bool m_bHasGroups = false;




    public WardrobeItemModel WardrobeItemModel
    {
        get
        {
            if (m_bHasGroups)
                return WardrobeGroupedItemModel.First;
            return m_pWardrobeItemModel;
        }
    }
    public WardrobeView.GroupedWardrobeItemModel WardrobeGroupedItemModel
    {
        get
        {
            return m_pWardrobeGroupedItemModel;
        }
    }
    public bool IsEquipped
    {
        get { return m_SelectableItem.IsSelected; }
    }
    public bool IsGrouped
    {
        get { return m_bIsGrouped; }
    }
    public bool HasGroups
    {
        get { return m_bHasGroups; }
    }


    public void SetupGroup(WardrobeView.GroupedWardrobeItemModel pWardrobeItemModel)
    {
        m_bHasGroups = true;
        m_bIsGrouped = pWardrobeItemModel.m_Items.Count > 1;
        SetEquipped(false);
        SetNew(false);
        SetConflicts(false);
        OnClickEvent = null;
        m_pWardrobeGroupedItemModel = pWardrobeItemModel;
        m_pWardrobeItemModel = null;
        

        title.text = m_pWardrobeGroupedItemModel.First.DisplayName;
        Sprite pSprt = ItemDB.Instance.GetTexturePreview(m_pWardrobeGroupedItemModel.First);
        icon.sprite = pSprt != null ? pSprt : m_DefaultIcon;

        m_WhiteIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.White));
        m_GrayIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.Gray));
        m_ShadowIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.Shadow));
    }

    public void Setup(WardrobeItemModel pWardrobeItemModel)
    {
        m_bHasGroups = false;
        m_bIsGrouped = false;
        SetNew(false);
        SetConflicts(false);
        SetEquipped(false);
        OnClickEvent = null;
        m_pWardrobeItemModel = pWardrobeItemModel;
        m_pWardrobeGroupedItemModel = null;

        if(title)
            title.text = pWardrobeItemModel.DisplayName;
        if (icon)
        {
            Sprite pSprt = ItemDB.Instance.GetTexturePreview(pWardrobeItemModel);
            icon.sprite = pSprt != null ? pSprt : m_DefaultIcon;
        }

        if(m_WhiteIcon) m_WhiteIcon.SetActive(false);
        if(m_GrayIcon)  m_GrayIcon.SetActive(false);
        if(m_ShadowIcon)    m_ShadowIcon.SetActive(false);
    }


    public override void OnClickButton()
    {
        base.OnClickButton();
        if (OnClickEvent != null)
            OnClickEvent(this);
    }

    public void SetEquipped(bool bEquipped)
    {
        m_SelectableItem.SetSelected(bEquipped);
    }

    public void SetNew(bool bVal)
    {
        if(m_goNewFlag)
        m_goNewFlag.SetActive(bVal);
    }
    public void SetConflicts(bool bVal)
    {
        if(m_goConflicts)
        m_goConflicts.SetActive(bVal);
    }

}

