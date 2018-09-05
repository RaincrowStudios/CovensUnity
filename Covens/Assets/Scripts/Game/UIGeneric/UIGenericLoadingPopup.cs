using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGenericLoadingPopup : UIBaseAnimated
{
    static UIGenericLoadingPopup Instance;
    public Text m_txtDescription;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        m_Target.SetActive(false);
    }


    public static void ShowLoading(string sTitle = null)
    {
        // show it
        Instance.Show();
    }
    public static void SetTitle(string sTitle)
    {
        // show it
        if (Instance.m_txtDescription != null)
            Instance.m_txtDescription.text = sTitle;
    }

    public static void CloseLoading()
    {
        Instance.Close();
    }
}
