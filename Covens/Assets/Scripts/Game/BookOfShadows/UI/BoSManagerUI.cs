using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoSManagerUI : UIBaseAnimated, IBeginDragHandler, IEndDragHandler
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
    private Vector2 m_vDragInitMousePosition = Vector2.zero;
    private bool m_bOnDrag = false;
    private int m_iCurPageIndex = 0;
    private int m_iSteps = 0;
    private int m_iLastPageIndex = 0;

#region LERP
    private bool m_bLerpPage = false;
    private float m_fInitBarValueLerp = 0.0f;
    private float m_fCurrentTimer = 0.0f;
    private float m_fTotalTimer = 0.65f;
#endregion

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
            GameObject pSpellUI = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSpellPrefabName), m_pContentGrid.transform) as GameObject;
            pSpellUI.name = "SpellPage" + i.ToString();
            pSpellUI.transform.localScale = Vector3.one;
            pSpellUI.GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, i, this);
        }

        m_pHorizontalbar.numberOfSteps = 0;
        m_iSteps = pResponse.spells.Count + 1;
        m_pHorizontalbar.value = 0.0f;
        GoToPage(0);

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
        m_iLastPageIndex = iIndex;
        m_bLerpPage = true;
        m_fInitBarValueLerp = m_pHorizontalbar.value;
        m_fCurrentTimer = 0.0f;
        //StartCoroutine(ChangeToPage(iIndex));
    }

    /*private IEnumerator ChangeToPage(int iIndex)
    {
        yield return null;

        float fNewValue = 0.0f;
        if (m_iSteps > 1)
            fNewValue = GetValueByIndex(iIndex);

        m_pHorizontalbar.value = fNewValue;
        Canvas.ForceUpdateCanvases();

        m_iCurPageIndex = iIndex;
    }
    */

    private float GetValueByIndex(int iIndex)
    {
        float fResult = iIndex * ((1.0f / (m_iSteps - 1)));
        return fResult;
    }

    public void ShowSignatureUI(BoS_Spell pCurrentSpell, List<Bos_Signature_Data> pCurrentSignatures, BoSSpellScreenUI pCurrentSpellUI)
    {
        m_pSignatureUI.Show();
        m_pSignatureUI.GetComponent<BoSSignatureScreenUI>().SetupUI(pCurrentSpell, pCurrentSignatures, this, pCurrentSpellUI);
    }

    private void OnDisable()
    {
        m_pLoadingUI.SetActive(false);
    }
    
    private void GetCurrentPage(bool bRightToLeft)
    {
        if (m_iSteps > 1)
        {
            int iTargetIndex;

            float a1 = GetValueByIndex(m_iLastPageIndex);
  
            if (bRightToLeft)
                iTargetIndex = m_iLastPageIndex - 1;
            else
                iTargetIndex = m_iLastPageIndex + 1;
 
            float a2 = GetValueByIndex(iTargetIndex);
            float fDiff = Mathf.InverseLerp(a1, a2, m_pHorizontalbar.value);

            if (fDiff > 0.5f)
                m_iCurPageIndex = iTargetIndex;
            else
                m_iCurPageIndex = m_iLastPageIndex;
        }
        else
            m_iCurPageIndex = 0;
    }

    private int GetIndexPage()
    {
        float fStepsValue = GetStepValue();
        float fFactor = m_pHorizontalbar.value / fStepsValue;

        return Mathf.FloorToInt(fFactor);
    }

    public float GetStepValue() { return (1.0f / (m_iSteps - 1)); } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_vDragInitMousePosition = eventData.position;
        m_bOnDrag = true;
    }

    public void OnEndDrag(PointerEventData data)
    {
        VerifyEndDrag(m_vDragInitMousePosition, data.position);
    }

    public bool OnDrag() { return m_bOnDrag; }

    public void VerifyEndDrag(Vector2 vInitialPos, Vector2 vFinalPos)
    {
        if ((vFinalPos.x - vInitialPos.x) > 0.0f)
            GetCurrentPage(true);
        else
            GetCurrentPage(false);

        float fHorizontalDelta = Mathf.Abs(vFinalPos.x - vInitialPos.x);
        float fVerticalDelta = Mathf.Abs(vFinalPos.y - vInitialPos.y);

        if (fHorizontalDelta > fVerticalDelta)
            GoToPage(m_iCurPageIndex);

        m_bOnDrag = false;
    }

    void Update()
    {
        if (m_bLerpPage)
        {
            m_fCurrentTimer += Time.deltaTime;
            float fLerpFactor = m_fCurrentTimer / m_fTotalTimer;

            float fCurValue = Mathf.Lerp(m_fInitBarValueLerp, GetValueByIndex(m_iCurPageIndex), fLerpFactor);
            m_pHorizontalbar.value = fCurValue;
            Canvas.ForceUpdateCanvases();

            if (fLerpFactor >= 1.0f)
                m_bLerpPage = false;
        }
    }
}
