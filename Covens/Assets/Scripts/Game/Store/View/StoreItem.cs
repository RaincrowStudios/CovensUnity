using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    public string ID = "";

    [Header("Roots")]
    public GameObject RootPrice;
    public GameObject RootAmount;
    public GameObject RootType;
    public GameObject RootButton;
    public GameObject RootPriceTag;
    public GameObject RootDiscount;
    public GameObject RootPriceGold;
    public GameObject RootPriceSilver;
    public GameObject RootPriceOr;

    [Header("Components")]
    public Image m_sptIcon;
    public Text m_txtTitle;
    public Text m_txtGoldPrice;
    public Text m_txtSilverPrice;
    public Text m_txtAmount;
    public Text m_txtType;
    public Text m_txtBuy;
    public Text m_txtIAPValue;
    public Text m_txtIAPPrice;
    public Text m_txtIAPExtra;



    public void Setup(StoreItemModel pItem)
    {
        Utilities.SetActiveList(false, RootAmount, RootButton, RootDiscount, RootPriceTag, RootType);
        if (pItem == null)
            return;
        ID = pItem.ID;
        // setup price values
        RootPrice.SetActive(true);
        bool bHasGoldPrice = pItem.GoldPrice > 0;
        bool bHasSilverPrice = pItem.SilverPrice > 0;
        RootPriceGold.SetActive(bHasGoldPrice);
        RootPriceSilver.SetActive(bHasSilverPrice);
        RootPriceOr.SetActive(bHasGoldPrice && bHasSilverPrice);
        m_txtGoldPrice.text = pItem.GoldPrice.ToString();
        m_txtSilverPrice.text = pItem.SilverPrice.ToString();

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
                RootType.SetActive(true);
                m_txtType.text = pItem.Type;
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






    public void OnClickTry()
    {

    }
    public void OnClickBuy()
    {

    }


}