using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CovenTitleEditItem : MonoBehaviour
{
    public GameObject m_CurrentTitle;
    public Text m_Text;
    public CovenController.CovenTitle m_eTitle;

    public void Show(CovenController.CovenTitle eTitle, bool bIsCurrent)
    {
        m_CurrentTitle.SetActive(bIsCurrent);
        m_Text.text = eTitle.ToString();
        m_eTitle = eTitle;
    }

}