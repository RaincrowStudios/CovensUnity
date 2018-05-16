using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGenericInputPopup : UIBaseAnimated
{
    static UIGenericInputPopup Instance;

    [Header("Head")]
    public Text m_Title;
    public InputField m_Input;
    [Header("Ok")]
    public GameObject m_btnOk;
    public Text m_txtOk;
    [Header("No")]
    public GameObject m_btnNo;
    public Text m_txtNo;
    private Action<string> m_pOnClickConfirm;
    private Action<string> m_pOnClickNo;
    private Action<string> m_pOnValueChanged;


    private void Awake()
    {
        Instance = this;
        m_Target.SetActive(false);
    }

    #region Show

    public static void ShowPopup(string sTitle, string sStartString, Action<string> pOnClickConfirm, Action<string> pOnClickNo)
    {
        Show(sTitle, sStartString, "Ok", "No", pOnClickConfirm, pOnClickNo, null);
    }
    public static void ShowPopup(string sTitle, string sStartString, Action<string> pOnClickConfirm, Action<string> pOnClickNo, Action<string> pOnValueChanged)
    {
        Show(sTitle, sStartString, "Ok", "No", pOnClickConfirm, pOnClickNo, pOnValueChanged);
    }

    public static void Show(string sTitle, string sStartString, string sOkText, string sNoText, Action<string> pOnClickConfirm, Action<string> pOnClickNo, Action<string> pOnValueChanged)
    {
        // setup header
        Instance.m_Title.text = sTitle;
        Instance.m_Input.text = sStartString;

        // setup buttons
        Instance.m_btnOk.SetActive(!string.IsNullOrEmpty(sOkText));
        Instance.m_txtOk.text = sOkText;
        Instance.m_btnNo.SetActive(!string.IsNullOrEmpty(sNoText));
        Instance.m_txtNo.text = sNoText;

        // callbacks
        Instance.m_pOnClickConfirm = pOnClickConfirm;
        Instance.m_pOnClickNo = pOnClickNo;
        Instance.m_pOnValueChanged = pOnValueChanged;

        // show it
        Instance.Show();
    }

    #endregion



    #region button callbacks

    public void OnValueChanged()
    {
        if (m_pOnValueChanged != null)
            m_pOnValueChanged(m_Input.text);
    }

    public void OnClickConfirm()
    {
        if (m_pOnClickConfirm != null)
            m_pOnClickConfirm(m_Input.text);
        Close();
    }
    public void OnClickNo()
    {
        if (m_pOnClickNo != null)
            m_pOnClickNo(m_Input.text);
        Close();
    }

    #endregion

}