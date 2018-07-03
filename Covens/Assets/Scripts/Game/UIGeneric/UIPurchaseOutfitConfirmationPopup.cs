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


    public event Action<UIPurchaseOutfitConfirmationPopup> OnClickBuyWithSilverEvent;
    public event Action<UIPurchaseOutfitConfirmationPopup> OnClickBuyWithGoldEvent;


    private bool m_bCanBuyWithGold = false;
    private bool m_bCanBuyWithSilver = false;
    private string m_sItemName;

    public void Setup(string sItemName, string sTitle, Sprite pSprite, long lGoldPrice, long lSilverPrice)
    {
        OnClickBuyWithSilverEvent = null;
        OnClickBuyWithGoldEvent = null;

        // set properties
        m_sItemName = sItemName;
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
            m_bCanBuyWithGold = lGold >= lGoldPrice;
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
            m_bCanBuyWithSilver = lSilver >= lSilverPrice;
        }
    }

    public void OnClickBuyWithSilver()
    {
        if (!m_bCanBuyWithSilver)
        {
            UIPurchaseNotification.ShowNoSilver(m_sItemName);
            return;
        }
        if (OnClickBuyWithSilverEvent != null)
            OnClickBuyWithSilverEvent(this);
    }
    public void OnClickBuyWithGold()
    {
        if (!m_bCanBuyWithGold)
        {
            UIPurchaseNotification.ShowNoGold(m_sItemName);
            return;
        }
        if (OnClickBuyWithGoldEvent != null)
            OnClickBuyWithGoldEvent(this);
    }
    public void OnClickBuyMoreSilver()
    {
        StoreView pStore = UIManager.Get<StoreView>();
        pStore.ShowTabSilver();
        Close();
    }
}