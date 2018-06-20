using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class WardrobeItemButton : MonoBehaviour
{
	public Text title;
	public Image icon;
    public SelectableItem m_SelectableItem;

    private WardrobeItemModel m_pWardrobeItemModel;
    private WardrobeView m_pWardrobeView;
    public event Action<WardrobeItemButton> OnClickEvent;


    public WardrobeItemModel WardrobeItemModel
    {
        get
        {
            return m_pWardrobeItemModel;
        }
    }
    public bool IsEquipped
    {
        get { return m_SelectableItem.IsSelected; }
    }

    // Use this for initialization
    void Start ()
	{
		
	}

    public void Setup(WardrobeItemModel pWardrobeItemModel, WardrobeView pWardrobeView, Sprite pDefault)
    {
        SetEquipped(false);
        OnClickEvent = null;
        m_pWardrobeItemModel = pWardrobeItemModel;
        m_pWardrobeView = pWardrobeView;

        title.text = m_pWardrobeItemModel.DisplayName;
        Sprite pSprt = ItemDB.Instance.GetTexturePreview(m_pWardrobeItemModel);
        icon.sprite = pSprt != null ? pSprt : pDefault;
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

