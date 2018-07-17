using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSLandingScreenUI : UIBaseAnimated
{
    public UIBaseAnimated m_pSpellPage;

    public Text m_pCovenNameLabel;
    public Text m_pCovenTitleLabel;
    public Text m_pWorldRankLabel;
    public Text m_pDominionRankLabel;
    public Text m_pFavoriteSpellLabel;
    public Text m_pNemesisLabel;
    public Text m_pBenefactorLabel;
    public HorizontalLayoutGroup m_pNavigator;
    public string m_sNavMarkPrefab;

    public void OpenSpellPage()
    {
        m_pSpellPage.Show();
        //Close();
    }

    public void SetupUI(BookOfShadows_Display pData)
    {
        Debug.Log("Getting Book of Shadows Data: Success");
        Debug.Log("Coven Name: " + pData.covenName);

        if (string.IsNullOrEmpty(pData.covenName))
        {
            m_pCovenTitleLabel.text = "";
            m_pCovenNameLabel.text = "Not in Coven!";
        }
        else
        {
            m_pCovenTitleLabel.text = pData.covenTitle + " of coven";
            m_pCovenNameLabel.text = pData.covenName;
        }

        m_pWorldRankLabel.text = string.Format("Rank {0} in the",pData.worldRank);
        m_pDominionRankLabel.text = string.Format("Rank {0} in the dominion of",pData.dominionRank);
        m_pFavoriteSpellLabel.text = pData.favoriteSpell;
        m_pNemesisLabel.text = pData.nemesis;
        m_pBenefactorLabel.text = pData.benefactor;

        for (int i = 0; i < pData.spells.Count; i++)
        {
            GameObject pNavMark = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sNavMarkPrefab), m_pNavigator.transform) as GameObject;

            if (i == 0)
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.43137255f);
            else
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.17647f);

            pNavMark.transform.localScale = Vector3.one;
        }
    }

    public void OnError(string sResponse)
    {
        Debug.Log("Getting Book of Shadows Data: Faliure");
        Debug.Log("Message: " + sResponse);
    }
	
}
