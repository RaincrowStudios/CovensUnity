using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oktagon.Localization;

/// <summary>
/// A generic popup that asks the player and expects Yes, No or Cancel state
/// </summary>
public class UIGenericPopup : UIBaseAnimated
{
    static UIGenericPopup Instance;


    [Header("Head")]
    public Text m_Title;
    public Text m_Description;

    [Header("Ok")]
    public GameObject m_btnOk;
    public Text m_txtOk;
    [Header("No")]
    public GameObject m_btnNo;
    public Text m_txtNo;
    [Header("Cancel")]
    public GameObject m_btnCancel;
//    public Text m_txtCancel;

    private Action m_pOnClickConfirm;
    private Action m_pOnClickNo;
    private Action m_pOnClickCancel;


    private void Awake()
    {
        Instance = this;
        m_Target.SetActive(false);
    }



    #region Show


    public static void ShowYesNoPopup(string sTitle, string sDescription, Action pOnClickConfirm, Action pOnClickNo)
    {
        Show(sTitle, sDescription, Lokaki.GetText("General_Ok"), Lokaki.GetText("General_No"), null, pOnClickConfirm, pOnClickNo, null);
    }
    public static void ShowYesNoPopupLocalized(string sTitle, string sDescription, Action pOnClickConfirm, Action pOnClickNo)
    {
        ShowYesNoPopup(Lokaki.GetText(sTitle), Lokaki.GetText(sDescription), pOnClickConfirm, pOnClickNo);
    }
    public static void ShowConfirmPopup(string sTitle, string sDescription, Action pOnClickConfirm)
    {
        Show(sTitle, sDescription, Lokaki.GetText("General_Ok"), null, null, pOnClickConfirm, null, null);
    }
    public static void ShowConfirmPopup(string sTitle, string sDescription, string sOkMessage, Action pOnClickConfirm)
    {
        Show(sTitle, sDescription, sOkMessage, null, null, pOnClickConfirm, null, null);
    }
    public static void ShowConfirmPopupLocalized(string sTitle, string sDescription, string sOkMessage, Action pOnClickConfirm)
    {
        Show(Lokaki.GetText(sTitle), Lokaki.GetText(sDescription), Lokaki.GetText(sOkMessage), null, null, pOnClickConfirm, null, null);
    }
    public static void ShowErrorPopupLocalized(string sDescription, Action pOnClickConfirm)
    {
        Show(
            Lokaki.GetText("General_Error"), 
            Lokaki.GetText("General_ErrorDescription").Replace("<error>", sDescription), 
            Lokaki.GetText("General_Ok"), 
            null, null, pOnClickConfirm, null, null);
    }
    public static void Show(string sTitle, string sDescription, string sOkText, string sNoText, string sCancelText, Action pOnClickConfirm, Action pOnClickNo, Action pOnClickCancel)
    {
        // setup header
        Instance.m_Title.text = sTitle;
        Instance.m_Description.text = sDescription;

        // setup buttons
        Instance.m_btnOk.SetActive(!string.IsNullOrEmpty(sOkText));
        Instance.m_txtOk.text = sOkText;
        Instance.m_btnNo.SetActive(!string.IsNullOrEmpty(sNoText));
        Instance.m_txtNo.text = sNoText;
        Instance.m_btnCancel.SetActive(!string.IsNullOrEmpty(sCancelText));
//        Instance.m_txtCancel.text = sCancelText;

        // callbacks
        Instance.m_pOnClickConfirm = pOnClickConfirm;
        Instance.m_pOnClickNo = pOnClickNo;
        Instance.m_pOnClickCancel = pOnClickCancel;

        // show it
        Instance.Show();
    }


    #endregion


    #region button callbacks

    public void OnClickConfirm()
    {
        if (m_pOnClickConfirm != null)
            m_pOnClickConfirm();
        Close();
    }
    public void OnClickNo()
    {
        if (m_pOnClickNo != null)
            m_pOnClickNo();
        Close();
    }
    public void OnClickCancel()
    {
        if (m_pOnClickCancel != null)
            m_pOnClickCancel();
        Close();
    }

    #endregion
}