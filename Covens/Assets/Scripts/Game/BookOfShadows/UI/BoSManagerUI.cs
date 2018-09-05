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

    [Header("Crest Images")]
    public Sprite[] m_pCrestImages;

    private Vector2 m_vDragInitMousePosition = Vector2.zero;
    private bool m_bOnDrag = false;
    private int m_iCurPageIndex = 0;
    private int m_iSteps = 0;
    private int m_iLastPageIndex = 0;
    private List<BoSSpellScreenUI> m_lSpellUI = null;
    private bool m_bIsInit = false;

#region NAVIGATOR_MARK
    public GridLayoutGroup m_pNavigator;
    public string m_sNavMarkPrefab;
    private List<Image> m_lNavMarkCreated;
#endregion

    #region LERP
    private bool m_bLerpPage = false;
    private float m_fInitBarValueLerp = 0.0f;
    private float m_fCurrentTimer = 0.0f;
    private float m_fTotalTimer = 0.65f;
    #endregion

    public override void Show()
    {
        m_pMainCanvasRect = FindObjectOfType<DeathState>().GetComponent<RectTransform>();
        m_pLoadingUI.SetActive(true);
        //BookOfShadowsAPI.Display(SetupDataUI, OnError);
        

        m_pContentGrid.cellSize = m_pMainCanvasRect.sizeDelta;
        m_pHorizontalbar.value = 0;

        if (!m_bIsInit)
        {
            m_lSpellUI = new List<BoSSpellScreenUI>();
            m_lSpellUI.Clear();

            m_lNavMarkCreated = new List<Image>();
            m_lNavMarkCreated.Clear();
        }
        
        base.Show();
        SetupDataUI(BookOfShadowsAPI.data);
    }

    public void SetupDataUI(BookOfShadows_Display pResponse)
    {
        string sInvisibilitySpellID = "spell_invisibility";

        DisplayData = pResponse;
        StartCoroutine(m_pContentGrid.GetComponentInChildren<BoSLandingScreenUI>().SetupUI(DisplayData)); //Setup Landing Screen UI

        //Swapping the first spell with Invisibility Spell
        int iInvisibilityIndex = pResponse.spells.FindIndex(x => x.id.Equals(sInvisibilitySpellID));
        BoS_Spell pInvisibilitySpell = pResponse.spells[iInvisibilityIndex];
        pResponse.spells[iInvisibilityIndex] = pResponse.spells[0];
        pResponse.spells[0] = pInvisibilitySpell;

        //If is Spell UI is not Initialized, instantiate the Spell UIs and Navigator Mark
        if (!m_bIsInit)
        {
            m_lNavMarkCreated.Add(m_pNavigator.GetComponentInChildren<Image>());

            for (int i = 0; i < pResponse.spells.Count; i++)
                StartCoroutine(InstantiateNewSpellUI(i));
        }
        else //If there is spell page instantiate, verifying if it's valid yet
            ValidSpellUIs();


        ResizeNavigatorMark(); //Resizing navigator mark
        UpdateNavigatorColor(); //Updating Navigator mark

        //Go to Init Page
        m_pHorizontalbar.numberOfSteps = 0;
        m_iSteps = pResponse.spells.Count + 1;
        m_pHorizontalbar.value = 0.0f;
        StartCoroutine(ChangeToPage(0)); 

        //m_pLoadingUI.SetActive(false); //Close the Loading UI
        m_bIsInit = true;
    }

    private void ResizeNavigatorMark()
    {
        int iDefaultSizeLimit = 25;
        Vector2 vDefaultSize = new Vector2(58.0f, 58.0f);
        Vector2 vDefaultSpacing = new Vector2(20.0f, 0.0f);

        if (DisplayData.spells.Count >= iDefaultSizeLimit)
        {
            m_pNavigator.cellSize = new Vector2(vDefaultSize.x / 2, vDefaultSize.y / 2);
            m_pNavigator.spacing = new Vector2(vDefaultSpacing.x / 2, vDefaultSpacing.y / 2);
        }
        else
        {
            m_pNavigator.cellSize = vDefaultSize;
            m_pNavigator.spacing = vDefaultSpacing;
        }
    }

    private IEnumerator InstantiateNewSpellUI(int iCurrentIndex)
    {
        ResourceRequest loadAsync = Resources.LoadAsync("BookOfShadows/" + m_sSpellPrefabName, typeof(GameObject));
        while (!loadAsync.isDone)
        {
            yield return null;
        }

        //GameObject pSpellUI = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSpellPrefabName), m_pContentGrid.transform) as GameObject;
        GameObject pSpellUI = GameObject.Instantiate(loadAsync.asset as GameObject, m_pContentGrid.transform) as GameObject;
        pSpellUI.name = "SpellPage" + iCurrentIndex.ToString();
        pSpellUI.transform.localScale = Vector3.one;
        pSpellUI.GetComponent<BoSSpellScreenUI>().SetupUI(DisplayData, iCurrentIndex, this);

        ResourceRequest navAsync = Resources.LoadAsync("BookOfShadows/" + m_sNavMarkPrefab, typeof(GameObject));
        while (!navAsync.isDone)
            yield return null;

        //GameObject pNavMark = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sNavMarkPrefab), m_pNavigator.transform) as GameObject;
        GameObject pNavMark = GameObject.Instantiate(navAsync.asset as GameObject, m_pNavigator.transform) as GameObject;
        pNavMark.transform.localScale = Vector3.one;
        m_lNavMarkCreated.Add(pNavMark.GetComponent<Image>());

        m_lSpellUI.Add(pSpellUI.GetComponent<BoSSpellScreenUI>());

        Debug.Log(string.Format("Instantiate {0} Spell UI done!", DisplayData.spells[iCurrentIndex].id));
    }

    private void ValidSpellUIs()
    {
        //bUIVerified is the boolean array that setting true if the current spell is contained on current API request
        bool[] bUIVerified = new bool[m_lSpellUI.Count]; 
        for (int i = 0; i < bUIVerified.Length; i++) { bUIVerified[i] = false; }

        //For all spells on current API Request
        for (int i = 0; i < DisplayData.spells.Count; i++)
        {
            if (m_lSpellUI.Find(x => x.GetSpell().id.Equals(DisplayData.spells[i].id)) != null) //Current Spell on Request has a Spell UI
            {
                int iCurIndex = m_lSpellUI.FindIndex(x => x.GetSpell().id.Equals(DisplayData.spells[i].id)); //Getting Current Index
                bUIVerified[iCurIndex] = true; //Validating Spell UI linked with this current spell on Request
                m_lSpellUI[iCurIndex].UpdateUI(); //Updating Spell UI
            }
            else
                InstantiateNewSpellUI(i); //If current spell on Request hasn't a Spell UI, instantiate new Spell UI
        }

        //For each spell UI, verifying your validation
        for (int i = 0; i < bUIVerified.Length; i++)
        {
            //If it is not valid
            if (!bUIVerified[i])
            {
                //Destroying Spell UI and Navigator mark relative to this spell  
                Debug.Log(string.Format("Removing {0} UI", m_lSpellUI[i].GetSpell().id));
                GameObject.Destroy(m_lSpellUI[i].gameObject);
                GameObject.Destroy(m_lNavMarkCreated[i].gameObject);
                m_lSpellUI[i] = null;
                m_lNavMarkCreated[i] = null;
            }
        }

        //Removing Spell UI and Navigator mark invalid on relative list
        m_lSpellUI.RemoveAll(x => x == null);
        int aux = m_lNavMarkCreated.RemoveAll(w => w == null);
        Debug.Log("Removed " + aux.ToString() + " navigator mark");
    }

    public override void Close()
    {
        for (int i = 0; i < m_lSpellUI.Count; i++)
            m_lSpellUI[i].DefaultSpellData();

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
        yield return new WaitForSeconds(0.25f);

        float fNewValue = 0.0f;
        if (m_iSteps > 1)
            fNewValue = GetValueByIndex(iIndex);

        m_pHorizontalbar.value = fNewValue;
        Canvas.ForceUpdateCanvases();
        Debug.Log("___________ Change to Page 0_________________");

        m_iLastPageIndex = iIndex;
        m_iCurPageIndex = iIndex;
        UpdateNavigatorColor();

        if (m_pLoadingUI.activeSelf)
            m_pLoadingUI.SetActive(false); //Close the Loading UI
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
        float fPercentLimitToChangePage = 0.1f;

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

            if (fDiff > fPercentLimitToChangePage)
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
            GameObject.Destroy(m_lNavMarkCreated[i].gameObject);

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
                m_lNavMarkCreated[i].color = pCurrentMarkColor;
            else
            {
                if ((i == (m_iCurPageIndex - 1)) || (i == (m_iCurPageIndex + 1)))
                    m_lNavMarkCreated[i].color = pNearMarkColor;
                else
                    m_lNavMarkCreated[i].color = pOtherMarkColor;
            }
        }
    }
}
