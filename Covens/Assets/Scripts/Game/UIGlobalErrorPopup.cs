using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIGlobalErrorPopup : MonoBehaviour
{
    private static UIGlobalErrorPopup m_pInstance;

    [SerializeField] private TeamConfirmPopUp m_pPopup;
    
    private void Awake()
    {
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
}
