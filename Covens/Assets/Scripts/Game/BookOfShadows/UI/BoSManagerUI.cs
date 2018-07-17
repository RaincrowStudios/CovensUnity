using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSManagerUI : UIBaseAnimated
{
    public GridLayoutGroup m_pContentGrid;
    public RectTransform m_pMainCanvasRect;
    public Scrollbar m_pHorizontalbar;

    private BookOfShadows_Display m_pDisplayData;
    public string m_sSpellPrefabName;

    public BookOfShadows_Display DisplayData
    {
        get { return m_pDisplayData; }
        set { m_pDisplayData = value; }
    }

    private bool m_bIsInit = false;
    public BoSSignatureScreenUI m_pSignatureUI;

    public override void Show()
    {
        if (!m_bIsInit)
            BookOfShadowsAPI.Display(SetupDataUI, OnError);

        m_pContentGrid.cellSize = m_pMainCanvasRect.sizeDelta;
        m_pHorizontalbar.value = 0;
        base.Show();
    }

    public void SetupDataUI(BookOfShadows_Display pResponse)
    {
        DisplayData = pResponse;

        m_pContentGrid.GetComponentInChildren<BoSLandingScreenUI>().SetupUI(DisplayData);

        for (int i = 0; i < pResponse.spells.Count; i++)
        {
            GameObject pSpellUI = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSpellPrefabName),m_pContentGrid.transform) as GameObject;
            pSpellUI.name = "SpellPage" + i.ToString();
            pSpellUI.transform.localScale = Vector3.one;
            pSpellUI.GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, i, this);
        }

        m_pHorizontalbar.numberOfSteps = pResponse.spells.Count + 1;
        m_pHorizontalbar.value = 0;

        m_bIsInit = true;
    }

    public void OnError(string sResponse)
    {
        Debug.Log("Getting Book of Shadows Data: Faliure");
        Debug.Log("Message: " + sResponse);
    }

    /*public void ReloadUI()
    {
        for (int i = 0; i < m_pContentGrid.GetComponentsInChildren<GameObject>().Length; i++)
        {
            if (i == 0)
                m_pContentGrid.GetComponentsInChildren<GameObject>()[i].GetComponent<BoSLandingScreenUI>().SetupUI(DisplayData);
            else
                m_pContentGrid.GetComponentsInChildren<GameObject>()[i].GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, i - 1, this);
        }
    }*/

    public void ShowSignatureUI() { m_pSignatureUI.Show(); }
}
