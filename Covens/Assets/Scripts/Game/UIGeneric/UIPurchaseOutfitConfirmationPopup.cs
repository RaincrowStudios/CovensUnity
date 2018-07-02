using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPurchaseOutfitConfirmationPopup : UIBaseAnimated
{
    public Image m_ItemImage;
    public Text m_txtTitle;
    public Text m_txtDescription;

    [Header("Gold")]
    public GameObject m_GoldRoot;
    public Text m_txtGoldPrice;
    public Text m_txtGoldCurrent;
    public Text m_txtGoldBalance;
    [Header("Silver")]
    public GameObject m_SilverRoot;
    public Text m_txtSilverPrice;
    public Text m_txtSilverCurrent;
    public Text m_txtSilverBalance;


    public event Action OnClickBuyWithSilverEvent;
    public event Action OnClickBuyWithGoldEvent;


    public void Setup(string sTitle, Sprite pSprite, long lGoldPrice, long lSilverPrice)
    {
        OnClickBuyWithSilverEvent = null;
        OnClickBuyWithGoldEvent = null;

        // set properties
        m_ItemImage.sprite = pSprite;
        m_txtTitle.text = sTitle;
        //m_txtDescription.text = sDescription;

        // setting gold
        m_GoldRoot.SetActive(lGoldPrice > 0);
        if (lGoldPrice > 0)
        {
            long lGold = PlayerDataManager.playerData.gold;
            long lBalance = lGold - lGoldPrice;
            lBalance = lBalance < 0 ? 0 : lBalance;
            bool bCanBuy = lGold > lGoldPrice;
            m_txtGoldPrice.text = lGoldPrice.ToString();
            m_txtGoldBalance.text = lBalance.ToString();
            m_txtGoldCurrent.text = lGold.ToString();
            m_txtGoldCurrent.color = bCanBuy ? Color.white : Color.red;
        }

        // setting silver
        m_SilverRoot.SetActive(lSilverPrice > 0);
        if (lSilverPrice > 0)
        {
            long lSilver = PlayerDataManager.playerData.silver;
            long lBalance = lSilver - lSilverPrice;
            lBalance = lBalance < 0 ? 0 : lBalance;
            bool bCanBuy = lSilver > lGoldPrice;
            m_txtSilverPrice.text = lSilverPrice.ToString();
            m_txtSilverBalance.text = lBalance.ToString();
            m_txtSilverCurrent.text = lSilver.ToString();
            m_txtSilverCurrent.color = bCanBuy ? Color.white : Color.red;
        }
    }

    public void OnClickBuyWithSilver()
    {
        if (OnClickBuyWithSilverEvent != null)
            OnClickBuyWithSilverEvent();
    }
    public void OnClickBuyWithGold()
    {
        if (OnClickBuyWithGoldEvent != null)
            OnClickBuyWithGoldEvent();
    }
    public void OnClickBuyMoreSilver()
    {
        StoreView pStore = UIManager.Get<StoreView>();
        pStore.ShowTabSilver();
        Close();
    }
}