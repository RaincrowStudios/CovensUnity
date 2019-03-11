using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class UIGlobalErrorPopup : MonoBehaviour
{
    private static UIGlobalErrorPopup m_Instance;

    [SerializeField] private TeamConfirmPopUp m_Popup;
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    
    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        m_Instance = this;
        m_Popup.onClose = () => 
        {
            m_Canvas.enabled = false;
            m_InputRaycaster.enabled = false;
        };
    }

    public static void ShowPopUp(Action confirmAction, Action cancelAction, string txt)
    {
        m_Instance.m_Canvas.enabled = true;
        m_Instance.m_InputRaycaster.enabled = true;

        m_Instance.m_Popup.ShowPopUp(confirmAction, cancelAction, txt);
    }

    public static void ShowPopUp(Action cancelAction, string txt)
    {
        m_Instance.m_Canvas.enabled = true;
        m_Instance.m_InputRaycaster.enabled = true;

        m_Instance.m_Popup.ShowPopUp(cancelAction, txt);
    }

    public static void Close()
    {
        m_Instance.m_InputRaycaster.enabled = false;

        m_Instance.m_Popup.Close();
    }
    
    public static void Error(string err)
    {
        m_Instance.m_Popup.Error(err);
    }

    public static void ShowError(Action confirmAction, Action cancelAction, string txt, string confirmTxt = "Yes", string cancelTxt = "No")
    {
        ShowPopUp(confirmAction, cancelAction, txt);
        Error(txt);
        m_Instance.m_Popup.confirm.GetComponentInChildren<TextMeshProUGUI>().text = confirmTxt;
        m_Instance.m_Popup.cancel.GetComponentInChildren<TextMeshProUGUI>().text = cancelTxt;
    }

    public static void ShowError(Action cancelAction, string txt, string cancelTxt = "Ok")
    {
        ShowPopUp(cancelAction, txt);
        Error(txt);
        m_Instance.m_Popup.confirm.GetComponentInChildren<TextMeshProUGUI>().text = cancelTxt;
    }
}