using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// stores the scroll view item data
/// </summary>
public class CovenScrollViewItem : MonoBehaviour
{
    public Text m_txtLevel;
    public Text m_txtName;
    public Text m_txtTitle;
    public Text m_txtStatus;
    public GameObject m_imgBackground;


    public void Setup(string sLevel, string sName, string sTitle, string sStatus, bool bUseBackground)
    {
        m_txtLevel.text = sLevel;
        m_txtName.text = sName;
        m_txtLevel.text = sTitle;
        m_txtStatus.text = sStatus;
        m_imgBackground.SetActive(bUseBackground);
    }

}