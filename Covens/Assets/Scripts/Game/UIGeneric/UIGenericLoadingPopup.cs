using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGenericLoadingPopup : UIBaseAnimated
{
    static UIGenericLoadingPopup Instance;


    private void Awake()
    {
        Instance = this;
        m_Target.SetActive(false);
    }


    public static void ShowLoading(string sTitle = null)
    {
        // show it
        Instance.Show();
    }

    public static void CloseLoading()
    {
        Instance.Close();
    }
}
