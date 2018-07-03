using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPurchaseSuccess : UIBaseAnimated
{
    public Image m_ItemImage;
    public Text m_txtTitle;

    public void Setup(string sTitle, Sprite pSprite)
    {
        if (m_ItemImage)
            m_ItemImage.sprite = pSprite;
        if (m_txtTitle)
            m_txtTitle.text = sTitle;
    }

}
