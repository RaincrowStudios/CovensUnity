using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSLandingScreenUI : UIBaseAnimated
{
    public UIBaseAnimated m_pSpellPage;


    public void OpenSpellPage()
    {
        m_pSpellPage.Show();
        //Close();
    }
	
}
