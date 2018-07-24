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

#region NAVIGATOR_MARK
    public GridLayoutGroup m_pNavigator;
    public string m_sNavMarkPrefab;
    private List<GameObject> m_lNavMarkCreated;
#endregion

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

        m_lNavMarkCreated = new List<GameObject>();
        m_lNavMarkCreated.Add(m_pNavigator.GetComponentInChildren<Image>().transform.gameObject);

        for (int i = 0; i < pResponse.spells.Count; i++)
        {
            GameObject pSpellUI = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSpellPrefabName), m_pContentGrid.transform) as GameObject;
            pSpellUI.name = "SpellPage" + i.ToString();
            pSpellUI.transform.localScale = Vector3.one;
            pSpellUI.GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, i, this);

            GameObject pNavMark = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sNavMarkPrefab), m_pNavigator.transform) as GameObject;

            /*
            if (i == 0)
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.43137255f);
            else
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.17647f);
                */

            pNavMark.transform.localScale = Vector3.one;
            m_lNavMarkCreated.Add(pNavMark);
        }

        if (pResponse.spells.Count >= 25)
        {
            m_pNavigator.cellSize = new Vector2(29.0f, 29.0f);
            m_pNavigator.spacing = new Vector2(10.0f, 0.0f);
        }
        else
        {
            m_pNavigator.cellSize = new Vector2(58.0f, 58.0f);
            m_pNavigator.spacing = new Vector2(20.0f, 0.0f);
        }

        UpdateNavigatorColor();

        m_pHorizontalbar.numberOfSteps = 0;
        m_iSteps = pResponse.spells.Count + 1;
        m_pHorizontalbar.value = 0.0f;
        StartCoroutine(ChangeToPage(0));

        m_pLoadingUI.SetActive(false);
    }

    public override void Close()
    {
        BoSSpellScreenUI[] pAllChildUI = m_pContentGrid.GetComponentsInChildren<BoSSpellScreenUI>();

        for (int i = 0; i < pAllChildUI.Length; i++)
            GameObject.Destroy(pAllChildUI[i].gameObject);

        ResetUI();

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

    public void GoToPageImmediately(int iIndex)
    {
        StartCoroutine(ChangeToPage(iIndex));
    }

    private IEnumerator ChangeToPage(int iIndex)
    {
        yield return null;

        float fNewValue = 0.0f;
        if (m_iSteps > 1)
            fNewValue = GetValueByIndex(iIndex);

        m_pHorizontalbar.value = fNewValue;
        Canvas.ForceUpdateCanvases();

        m_iLastPageIndex = iIndex;
        m_iCurPageIndex = iIndex;
        UpdateNavigatorColor();
    }
    

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

    public bool OnDragMovement() { return m_bOnDrag; }

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
            {
                m_bLerpPage = false;
                UpdateNavigatorColor();
            }
        }
    }

    public void ResetUI()
    {
        for (int i = 1; i < m_lNavMarkCreated.Count; i++)
            GameObject.Destroy(m_lNavMarkCreated[i]);

        m_lNavMarkCreated.Clear();
    }

    private void UpdateNavigatorColor()
    {
        Color pCurrentMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 1.0f);
        Color pNearMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 0.43137255f);
        Color pOtherMarkColor = new Color(0.117647f, 0.117647f, 0.117647f, 0.17647f);

        for (int i = 0; i < m_lNavMarkCreated.Count; i++)
        {
            if (i == m_iCurPageIndex)
                m_lNavMarkCreated[i].GetComponent<Image>().color = pCurrentMarkColor;
            else
            {
                if ((i == (m_iCurPageIndex - 1)) || (i == (m_iCurPageIndex + 1)))
                    m_lNavMarkCreated[i].GetComponent<Image>().color = pNearMarkColor;
                else
                    m_lNavMarkCreated[i].GetComponent<Image>().color = pOtherMarkColor;
            }
        }
    }
}
