using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIGlobalErrorPopup : MonoBehaviour
{
    private static UIGlobalErrorPopup m_pInstance;

    [SerializeField] private TeamConfirmPopUp m_pPopup;
    
    private void Awake()
    {
        if(m_pInstance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        m_pInstance = this;
        m_pPopup.onClose = () => { gameObject.SetActive(false); };
        gameObject.SetActive(false);
    }

    public static void ShowPopUp(Action confirmAction, Action cancelAction, string txt)
    {
        m_pInstance.gameObject.SetActive(true);
        m_pInstance.m_pPopup.ShowPopUp(confirmAction, cancelAction, txt);
    }

    public static void ShowPopUp(Action cancelAction, string txt)
    {
        m_pInstance.gameObject.SetActive(true);
        m_pInstance.m_pPopup.ShowPopUp(cancelAction, txt);
    }

    public static void Close()
    {
        m_pInstance.m_pPopup.Close();
    }
    
    public static void Error(string err)
    {
        m_pInstance.m_pPopup.Error(err);
    }

    public static void ShowError(Action confirmAction, Action cancelAction, string txt, string confirmTxt = "Yes", string cancelTxt = "No")
    {
        ShowPopUp(confirmAction, cancelAction, txt);
        Error(txt);
        m_pInstance.m_pPopup.confirm.GetComponentInChildren<Text>().text = confirmTxt;
        m_pInstance.m_pPopup.cancel.GetComponentInChildren<Text>().text = cancelTxt;
    }

    public static void ShowError(Action cancelAction, string txt, string cancelTxt = "Ok")
    {
        ShowPopUp(cancelAction, txt);
        Error(txt);
        m_pInstance.m_pPopup.confirm.GetComponentInChildren<Text>().text = cancelTxt;
    }
}