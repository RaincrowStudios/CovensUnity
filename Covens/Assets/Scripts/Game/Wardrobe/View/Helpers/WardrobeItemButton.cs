using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class WardrobeItemButton : MonoBehaviour
{
    public event Action<WardrobeItemButton> OnClickEvent;


    public Text title;
	public Image icon;
    public SelectableItem m_SelectableItem;
    public GameObject m_goNewFlag;

    [Header("Color icons")]
    public GameObject m_WhiteIcon;
    public GameObject m_GrayIcon;
    public GameObject m_ShadowIcon;

    private WardrobeView.GroupedWardrobeItemModel m_pWardrobeGroupedItemModel;
    private WardrobeItemModel m_pWardrobeItemModel;
    private WardrobeView m_pWardrobeView;
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


    public void Setup(WardrobeView.GroupedWardrobeItemModel pWardrobeItemModel, WardrobeView pWardrobeView, Sprite pDefault)
    {
        m_bHasGroups = true;
        m_bIsGrouped = pWardrobeItemModel.m_Items.Count > 1;
        SetEquipped(false);
        OnClickEvent = null;
        m_pWardrobeGroupedItemModel = pWardrobeItemModel;
        m_pWardrobeItemModel = null;
        m_pWardrobeView = pWardrobeView;

        title.text = m_pWardrobeGroupedItemModel.First.DisplayName;
        Sprite pSprt = ItemDB.Instance.GetTexturePreview(m_pWardrobeGroupedItemModel.First);
        icon.sprite = pSprt != null ? pSprt : pDefault;

        m_WhiteIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.White));
        m_GrayIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.Gray));
        m_ShadowIcon.SetActive(pWardrobeItemModel.HasAlignment(EnumAlignment.Shadow));
    }

    public void Setup(WardrobeItemModel pWardrobeItemModel, WardrobeView pWardrobeView, Sprite pDefault)
    {
        m_bHasGroups = false;
        m_bIsGrouped = false;
        SetEquipped(false);
        OnClickEvent = null;
        m_pWardrobeItemModel = pWardrobeItemModel;
        m_pWardrobeGroupedItemModel = null;
        m_pWardrobeView = pWardrobeView;

        if(title)
            title.text = pWardrobeItemModel.DisplayName;
        if (icon)
        {
            Sprite pSprt = ItemDB.Instance.GetTexturePreview(pWardrobeItemModel);
            icon.sprite = pSprt != null ? pSprt : pDefault;
        }

        if(m_WhiteIcon) m_WhiteIcon.SetActive(false);
        if(m_GrayIcon)  m_GrayIcon.SetActive(false);
        if(m_ShadowIcon)    m_ShadowIcon.SetActive(false);
    }


    public void OnClickItem()
    {
        if (OnClickEvent != null)
            OnClickEvent(this);
    }

    public void SetEquipped(bool bEquipped)
    {
        m_SelectableItem.SetSelected(bEquipped);
    }
}

