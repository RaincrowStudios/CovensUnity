using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPurchaseConfirmationPopup : UIBaseAnimated
{
    public Image m_ItemImage;
    public Text m_txtTitle;
    public Text m_txtDescription;
    [Header("Price")]
    public GameObject m_GoldButton;
    public Text m_txtGoldPrice;
    public GameObject m_SilverButton;
    public Text m_txtSilverPrice;


    public event Action<UIPurchaseConfirmationPopup> OnClickBuyWithSilverEvent;
    public event Action<UIPurchaseConfirmationPopup> OnClickBuyWithGoldEvent;

    private string m_sItemName;
    private bool m_bCanBuyWithGold = false;
    private bool m_bCanBuyWithSilver = false;

    public void Setup(string sItemName, string sTitle, string sDescription, Sprite pSprite, long lGoldPrice, long lSilverPrice)
    {
        OnClickBuyWithSilverEvent = null;
        OnClickBuyWithGoldEvent = null;

        // set properties
        m_sItemName = sItemName;
        m_ItemImage.sprite = pSprite;
        m_txtTitle.text = sTitle;
        m_txtDescription.text = sDescription;

        // setting the price
        m_GoldButton.SetActive(lGoldPrice > 0);
        m_txtGoldPrice.text = lGoldPrice.ToString();
        m_SilverButton.SetActive(lSilverPrice > 0);
        m_txtSilverPrice.text = lSilverPrice.ToString();

        long lGold = PlayerDataManager.playerData.gold;
        long lSilver = PlayerDataManager.playerData.silver;
        m_bCanBuyWithGold = lGold >= lGoldPrice;
        m_bCanBuyWithSilver = lSilver >= lSilverPrice;
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
}