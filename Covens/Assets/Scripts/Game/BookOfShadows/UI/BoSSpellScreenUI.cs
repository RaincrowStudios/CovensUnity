using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSSpellScreenUI : UIBaseAnimated
{
    public HorizontalLayoutGroup m_pNavigator;
    public string m_sNavMarkPrefab;
    public Button m_pCloseButton;
    public Button m_pSignatureButton;

    public void SetupUI(BookOfShadows_Display pData, int iIndex, BoSManagerUI pParent)
    {
        Color pCurrentMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 1.0f);
        Color pNearMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 0.43137255f);
        Color pOtherMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 0.17647f);

        if (iIndex == 0)
            m_pNavigator.GetComponentInChildren<Image>().color = pNearMarkColor;
        else
            m_pNavigator.GetComponentInChildren<Image>().color = pOtherMarkColor;

        for (int i = 0; i < pData.spells.Count; i++)
        {
            GameObject pNavMark = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sNavMarkPrefab), m_pNavigator.transform) as GameObject;

            if ((i == (iIndex - 1)) || (i == (iIndex + 1)))
                pNavMark.GetComponent<Image>().color = pNearMarkColor;
            else
            {
                if (i == iIndex)
                    pNavMark.GetComponent<Image>().color = pCurrentMarkColor;
                else
                    pNavMark.GetComponent<Image>().color = pOtherMarkColor;
            }
            pNavMark.transform.localScale = Vector3.one;
        }

        m_pCloseButton.onClick.AddListener(delegate{ pParent.Close(); });
        m_pSignatureButton.onClick.AddListener(delegate { pParent.ShowSignatureUI(); });
    }
	
 
}
