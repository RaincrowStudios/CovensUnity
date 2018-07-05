using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// with this button we have 2 situations
/// 1. wardrobe item selling setup
/// 2. generic item selling setup
/// </summary>
public class StoreItem : MonoBehaviour
{
    public string ID = "";

    [Header("Roots")]
    public GameObject RootPrice;
    public GameObject RootAmount;
    public GameObject RootDescription;
    public GameObject RootButton;
    public GameObject RootPriceTag;
    public GameObject RootDiscount;
    public GameObject RootPriceGold;
    public GameObject RootPriceSilver;
    public GameObject RootPriceOr;
    public GameObject RootUnlocked;

    [Header("Components")]
    public Image m_sptIcon;
    public Text m_txtTitle;
    public Text m_txtGoldPrice;
    public Text m_txtSilverPrice;
    public Text m_txtAmount;
    public Text m_txtDescription;
    public Text m_txtBuy;
    public Text m_txtIAPValue;
    public Text m_txtIAPPrice;
    public Text m_txtIAPExtra;

    private StoreItemModel m_pItem;
    private WardrobeItemModel m_pItemWardrobe;
    public event Action<StoreItem, bool> OnClickBuyEvent;
    public event Action<StoreItem> OnClickTryEvent;

    private bool m_bUnlocked = false;

    public StoreItemModel ItemStore
    {
        get { return m_pItem; }
    }
    public WardrobeItemModel ItemWardrobe
    {
        get { return m_pItemWardrobe; }
    }


    public void Setup(WardrobeItemModel pItem, bool bUnlocked)
    {
        m_bUnlocked = bUnlocked;
        m_pItemWardrobe = pItem;
        OnClickBuyEvent = null;
        OnClickTryEvent = null;
        Utilities.SetActiveList(false, RootAmount,  RootDiscount, RootPriceTag, RootDescription);
        Utilities.SetActiveList(!bUnlocked, RootButton);

        // setup price values
        SetPrice(pItem.GoldPrice, pItem.SilverPrice);

        // setup others
        if(m_sptIcon != null)
            m_sptIcon.sprite = ItemDB.Instance.GetTexturePreview(pItem);
        if(m_txtTitle != null)
            m_txtTitle.text = pItem.DisplayName;
        if(m_txtGoldPrice != null)
            m_txtGoldPrice.text = pItem.GoldPrice.ToString();
        if(m_txtSilverPrice != null)
            m_txtSilverPrice.text = pItem.SilverPrice.ToString();
        if(RootUnlocked)
            RootUnlocked.SetActive(bUnlocked);
    }

    public void Setup(StoreItemModel pItem)
    {
        m_bUnlocked = false;
        m_pItem = pItem;
        OnClickBuyEvent = null;
        OnClickTryEvent = null;
        Utilities.SetActiveList(false, RootAmount, RootButton, RootDiscount, RootPriceTag, RootDescription, RootUnlocked);
        if (pItem == null)
            return;

        ID = pItem.ID;

        // setup price values
        SetPrice(pItem.GoldPrice, pItem.SilverPrice);

        // setup icon
        m_sptIcon.sprite = SpriteResources.GetSprite(pItem.Icon);

        // naming
        m_txtBuy.text = "Buy";
        m_txtTitle.text = pItem.DisplayName;

        // specific changes
        switch (pItem.StoreTypeEnum)
        {
            case EnumStoreType.Aptitude:
                RootButton.SetActive(true);
                break;
            case EnumStoreType.Bundles:
                RootButton.SetActive(true);
                break;
            case EnumStoreType.Energy:
                RootButton.SetActive(true);
                RootAmount.SetActive(true);
                m_txtAmount.text = string.Format("x{0}", pItem.Amount.ToString());
                break;
            case EnumStoreType.Experience:
                RootButton.SetActive(true);
                RootDescription.SetActive(true);
                m_txtDescription.text = pItem.DisplaySubDescription;
                break;
            case EnumStoreType.IAP:
                RootPrice.SetActive(false);
                RootButton.SetActive(false);
                RootPriceTag.SetActive(true);
                m_txtIAPValue.text = pItem.Value.ToString();
                //m_txtIAPValue.text = pItem.Iap.ToString();
                break;
        }
    }



    void SetPrice(long lGoldPrice, long lSilverPrice)
    {
        if (RootPrice == null)
            return;
        RootPrice.SetActive(true);
        bool bHasGoldPrice = lGoldPrice > 0;
        bool bHasSilverPrice = lSilverPrice > 0;
        RootPriceGold.SetActive(bHasGoldPrice);
        RootPriceSilver.SetActive(bHasSilverPrice);
        RootPriceOr.SetActive(bHasGoldPrice && bHasSilverPrice);
        m_txtGoldPrice.text = lGoldPrice.ToString();
        m_txtSilverPrice.text = lSilverPrice.ToString();
    }




    public void OnClickTry()
    {
        if (OnClickTryEvent != null)
            OnClickTryEvent(this);
    }
    public void OnClickBuy()
    {
        if (OnClickBuyEvent != null)
            OnClickBuyEvent(this, RootUnlocked.activeSelf);
    }


}