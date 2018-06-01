using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oktagon.Localization;


public class UIGenericInputPopup : UIBaseAnimated
{
    public static UIGenericInputPopup Instance;

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

    [Header("Tip components")]
    public GameObject m_TipsRoot;
    public SimpleObjectPool m_TipsPool;
    public GameObject m_LoadingIcon;

    private float m_fDelayToTip = 1;
    private Coroutine m_TipCoroutine;

    private void Awake()
    {
        Instance = this;
        m_Target.SetActive(false);
        m_TipsPool.Setup();
    }

    #region Show

    public static UIGenericInputPopup ShowPopup(string sTitle, string sStartString, Action<string> pOnClickConfirm, Action<string> pOnClickNo)
    {
        return Show(sTitle, sStartString, Lokaki.GetText("General_Ok"), Lokaki.GetText("General_No"), pOnClickConfirm, pOnClickNo, null);
    }
    public static UIGenericInputPopup ShowPopupLocalized(string sTitle, string sStartString, Action<string> pOnClickConfirm, Action<string> pOnClickNo)
    {
        return ShowPopup(Lokaki.GetText(sTitle), sStartString, pOnClickConfirm, pOnClickNo);
    }
    public static UIGenericInputPopup ShowPopup(string sTitle, string sStartString, Action<string> pOnClickConfirm, Action<string> pOnClickNo, Action<string> pOnValueChanged)
    {
        return Show(sTitle, sStartString, Lokaki.GetText("General_Ok"), Lokaki.GetText("General_No"), pOnClickConfirm, pOnClickNo, pOnValueChanged);
    }

    public static UIGenericInputPopup Show(string sTitle, string sStartString, string sOkText, string sNoText, Action<string> pOnClickConfirm, Action<string> pOnClickNo, Action<string> pOnValueChanged)
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

        // loading icon
        Instance.m_LoadingIcon.SetActive(false);

        // show it
        Instance.Show();
        return Instance;
    }

    #endregion


    public override void Show()
    {
        base.Show();
        m_TipsPool.DespawnAll();
        SetLoading(false);
        StartCoroutine(RefreshLayer());
    }


    public void SetInputChangedCallback(Action<string> pOnValueChanged, float fTipDelay = 1)
    {
        m_pOnValueChanged = pOnValueChanged;
        m_fDelayToTip = fTipDelay;
    }

    public void SetLoading(bool bIsLoading)
    {
        if(m_LoadingIcon != null)
            m_LoadingIcon.SetActive(bIsLoading);
    }


    #region button callbacks

    public void OnValueChanged()
    {
        if (m_pOnValueChanged != null && !string.IsNullOrEmpty(m_Input.text))
        {
            if (m_TipCoroutine != null)
                StopCoroutine(m_TipCoroutine);
            m_TipCoroutine = StartCoroutine(TipNotifyDelay());
        }
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


    IEnumerator TipNotifyDelay()
    {
        yield return new WaitForSeconds(m_fDelayToTip);
        if (m_pOnValueChanged != null)
            m_pOnValueChanged(m_Input.text);
    }
    public void OnClickTipButton(Text pText)
    {
        m_Input.text = pText.text;
    }

    public void SetTipList(string[] vTips)
    {
        m_TipsPool.DespawnAll();
        for (int i = 0; i < vTips.Length; i++)
        {
            GameObject pGO = m_TipsPool.Spawn();
            Text pTxt = pGO.GetComponentInChildren<Text>();
            if(pTxt != null)
            {
                pTxt.text = vTips[i];
            }
        }
        StartCoroutine(RefreshLayer());
    }

    IEnumerator RefreshLayer()
    {
        yield return null;
        m_TipsRoot.SetActive(false);
        m_TipsRoot.SetActive(true);
    }


}