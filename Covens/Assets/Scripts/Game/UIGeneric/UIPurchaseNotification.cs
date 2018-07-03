using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPurchaseNotification : UIBaseAnimated
{
    public Text m_txtText;
    public float m_CloseTime = 3;

    public void Setup(string sText)
    {
        m_txtText.text = sText;
        CancelInvoke("Close");
        Invoke("Close", m_CloseTime);
    }


    public static void Show(string sText)
    {
        UIPurchaseNotification pUi = UIManager.Show<UIPurchaseNotification>();
        pUi.Setup(sText);
    }

    public static void ShowNoSilver(string sItemName)
    {
        string sSilver = Oktagon.Localization.Lokaki.GetText("General_Silver");
        string sText = Oktagon.Localization.Lokaki.GetText("Store_NoCoinToUnlock");
        sText = sText.Replace("<coin>", sSilver).Replace("<item>", sItemName);
        Show(sText);
    }
    public static void ShowNoGold(string sItemName)
    {
        string sSilver = Oktagon.Localization.Lokaki.GetText("General_Gold");
        string sText = Oktagon.Localization.Lokaki.GetText("Store_NoCoinToUnlock");
        sText = sText.Replace("<coin>", sSilver).Replace("<item>", sItemName);
        Show(sText);
    }

}