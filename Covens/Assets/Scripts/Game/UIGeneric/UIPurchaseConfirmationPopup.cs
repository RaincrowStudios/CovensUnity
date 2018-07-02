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


    public event Action OnClickBuyWithSilverEvent;
    public event Action OnClickBuyWithGoldEvent;


    public void Setup(string sTitle, string sDescription, Sprite pSprite, long lGoldPrice, long lSilverPrice)
    {
        OnClickBuyWithSilverEvent = null;
        OnClickBuyWithGoldEvent = null;

        // set properties
        m_ItemImage.sprite = pSprite;
        m_txtTitle.text = sTitle;
        m_txtDescription.text = sDescription;

        // setting the price
        m_GoldButton.SetActive(lGoldPrice > 0);
        m_txtGoldPrice.text = lGoldPrice.ToString();
        m_SilverButton.SetActive(lSilverPrice > 0);
        m_txtSilverPrice.text = lSilverPrice.ToString();
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
}