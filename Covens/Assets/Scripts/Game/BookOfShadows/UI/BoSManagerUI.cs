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
    public GameObject m_pLoadingUI;

    public BookOfShadows_Display DisplayData
    {
        get { return m_pDisplayData; }
        set { m_pDisplayData = value; }
    }

    public BoSSignatureScreenUI m_pSignatureUI;
    //private int m_iCurPageIndex = 0;

    public override void Show()
    {
        m_pLoadingUI.SetActive(true);
        BookOfShadowsAPI.Display(SetupDataUI, OnError);

        m_pContentGrid.cellSize = m_pMainCanvasRect.sizeDelta;
        m_pHorizontalbar.value = 0;

        base.Show();
    }

    public void SetupDataUI(BookOfShadows_Display pResponse)
    {
        string sInvisibilitySpellID = "spell_invisibility";

        DisplayData = pResponse;

        m_pContentGrid.GetComponentInChildren<BoSLandingScreenUI>().SetupUI(DisplayData);

        int iInvisibilityIndex = pResponse.spells.FindIndex(x => x.id.Equals(sInvisibilitySpellID));
        BoS_Spell pInvisibilitySpell = pResponse.spells[iInvisibilityIndex];
        pResponse.spells[iInvisibilityIndex] = pResponse.spells[0];
        pResponse.spells[0] = pInvisibilitySpell;

        for (int i = 0; i < pResponse.spells.Count; i++)
        {
            GameObject pSpellUI = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSpellPrefabName),m_pContentGrid.transform) as GameObject;
            pSpellUI.name = "SpellPage" + i.ToString();
            pSpellUI.transform.localScale = Vector3.one;
            pSpellUI.GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, i, this);
        }

        m_pHorizontalbar.numberOfSteps = pResponse.spells.Count + 1;
        m_pHorizontalbar.value = 0.0f;

        m_pLoadingUI.SetActive(false);
    }

    public override void Close()
    {
        BoSSpellScreenUI[] pAllChildUI = m_pContentGrid.GetComponentsInChildren<BoSSpellScreenUI>();

        for (int i = 0; i < pAllChildUI.Length; i++)
            GameObject.Destroy(pAllChildUI[i].gameObject);

        m_pContentGrid.GetComponentInChildren<BoSLandingScreenUI>().ResetUI();

        base.Close();
    }

    public void OnError(string sResponse)
    {
        Debug.Log("Getting Book of Shadows Data: Faliure");
        Debug.Log("Message: " + sResponse);
    }

    public void GoToPage(int iIndex)
    {
        StartCoroutine(ChangeToPage(iIndex));
    }

    private IEnumerator ChangeToPage(int iIndex)
    {
        yield return null;

        float fNewValue = 0.0f;
        if (m_pHorizontalbar.numberOfSteps > 1)  
            fNewValue = iIndex * ((1.0f / (m_pHorizontalbar.numberOfSteps - 1)) + 0.01f);

        m_pHorizontalbar.value = fNewValue;
        Canvas.ForceUpdateCanvases();

        //m_iCurPageIndex = iIndex;
    }

    public void ShowSignatureUI(BoS_Spell pCurrentSpell, List<Bos_Signature_Spell_Data> pCurrentSignatures, BoSSpellScreenUI pCurrentSpellUI)
    {
        m_pSignatureUI.Show();
        m_pSignatureUI.GetComponent<BoSSignatureScreenUI>().SetupUI(pCurrentSpell, pCurrentSignatures, this, pCurrentSpellUI);
    }

    private void OnDisable()
    {
        m_pLoadingUI.SetActive(false);
    }

   /* private void GetCurrentPage()
    {
        if (m_pHorizontalbar.numberOfSteps > 1)
            m_iCurPageIndex = Mathf.FloorToInt( m_pHorizontalbar.value / (1.0f / (m_pHorizontalbar.numberOfSteps - 1)));
        else
            m_iCurPageIndex = 0;
    }

    private void Update()
    {
        GetCurrentPage();
    }*/
}
