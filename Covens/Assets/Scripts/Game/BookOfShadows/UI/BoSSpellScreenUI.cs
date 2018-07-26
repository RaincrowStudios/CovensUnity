using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BoSSpellScreenUI : UIBaseAnimated
{
    [Header("General UI Data")]
    public Text m_pTitleLabel;

    public Text m_pCostSpellLabel;
    public Text m_pCostSpellValue;

    public Text m_pSpellDescription;
    public Image m_pCrest;
    public Text m_pAlignAffectedLabel;

    //public HorizontalLayoutGroup m_pNavigator;
    //public string m_sNavMarkPrefab;
    public ScrollRect m_pDescriptionScrollView;
    public Button m_pCloseButton;

    [Header("Signature")]
    public string m_sSignaturePrefab;
    public Button m_pSignatureButton;
    public RectTransform m_pSignaturesNameButtonsGroup;
   
    [Header("Glyph")]
    public Image m_pGlyphBaseImage;
    public Image m_pGlyphImage;


    public BoSSignatureButton SelectedButton { get; set; }
 
    #region LOKAKI IDS
    private string m_sCostSpellLabelID = "BoS_SpellCost";
    private string m_sCostSpellValueID = "BoS_SpellCostValue";
    private string m_sSpellAlignAffectedID = "BoS_SpellAlignAffected";
    private string m_sKnownSignaturesID = "BoS_KnownSignatureMessage";
    private string m_sNotKnowSignatureID = "BoS_NotKnownSignatureMessage";
    #endregion

    private BoSManagerUI m_pManager = null;
    private BoS_Spell m_pCurrentSpell = null;
    private BoS_Signatures m_pCurrentSignatures = null;
    private List<BoSSignatureButton> m_lSignaturesAvailableButtons = null;

    private List<Bos_Signature_Data> m_lSignaturesList = null;

    public List<BoSSignatureButton> lSignatureAvailableButtons
    {
        get
        {
            return m_lSignaturesAvailableButtons;
        }
        set
        {
            m_lSignaturesAvailableButtons = value;
        }
    }

    
    public void SetupUI(BookOfShadows_Display pData, int iIndex, BoSManagerUI pParent)
    {
        m_pManager = pParent;
        m_pCurrentSpell = pData.spells[iIndex];
        m_lSignaturesAvailableButtons = new List<BoSSignatureButton>();

        m_pTitleLabel.text = Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_title");

        m_pCostSpellLabel.text = Oktagon.Localization.Lokaki.GetText(m_sCostSpellLabelID);
        m_pCostSpellValue.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sCostSpellValueID), pData.spells[iIndex].cost);

        m_pSpellDescription.text = Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_description");

        m_pCrest.sprite = GetCrestSprite(PlayerDataManager.playerData.degree);

        if (iIndex > 0)
            m_pAlignAffectedLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sSpellAlignAffectedID), Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_title"), GetAlignAffect(pData.spells[iIndex].school));
        else
            m_pAlignAffectedLabel.text = ""; //Invisibility Spell

        /*
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
        */
        
        
        if (pData.signatures.signaturesSpellList.TryGetValue(pData.spells[iIndex].id, out m_lSignaturesList))
        {
            m_pSignatureButton.gameObject.GetComponent<Button>().enabled = true;
            m_pSignatureButton.GetComponentInChildren<Text>().text = Oktagon.Localization.Lokaki.GetText(m_sKnownSignaturesID);
            m_pSignaturesNameButtonsGroup.gameObject.SetActive(true);

            for (int i = 0; i < m_lSignaturesList.Count; i++)
            {
                GameObject pSignature = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSignaturePrefab), m_pSignaturesNameButtonsGroup) as GameObject;
                pSignature.GetComponentInChildren<Text>().text = Oktagon.Localization.Lokaki.GetText(m_lSignaturesList[i].effect + "_title");

                BoSSignatureButton curButton = pSignature.GetComponent<BoSSignatureButton>();
                curButton.SpellData = m_lSignaturesList[i];
                curButton.ReferenceUI = this;
                pSignature.GetComponent<Button>().onClick.AddListener(delegate{ curButton.OnClickSignature(); });

                m_lSignaturesAvailableButtons.Add(curButton);
            }
        }
        else
        {
            m_pSignatureButton.gameObject.GetComponent<Button>().enabled = false;
            m_pSignatureButton.GetComponentInChildren<Text>().text = Oktagon.Localization.Lokaki.GetText(m_sNotKnowSignatureID);
            m_pSignaturesNameButtonsGroup.gameObject.SetActive(false);
        }
        m_pCloseButton.onClick.AddListener(delegate{ pParent.Close(); });
        m_pSignatureButton.onClick.AddListener(delegate { ShowSignatureUI(); });

        UpdateLayout();

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pSignaturesNameButtonsGroup);
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pSignaturesNameButtonsGroup.parent.GetComponent<RectTransform>());
    }

    private Sprite GetCrestSprite(int iDegree)
    {
        string sSpritePath = "BookOfShadows/SpritesUI/";

        string sLightCrestID = "LightCrestBlack";
        string sGrayCrestID = "GrayCrestBlack";
        string sShadowCrestID = "ShadowCrestBlack";

        Texture2D curTex = null;

        if (iDegree > 0)
            curTex = Resources.Load(sSpritePath + sLightCrestID) as Texture2D;
        else
        {
            if (iDegree < 0)
                curTex = Resources.Load(sSpritePath + sShadowCrestID) as Texture2D;
            else
                curTex = Resources.Load(sSpritePath + sGrayCrestID) as Texture2D;
        }

        return Sprite.Create(curTex, new Rect(0, 0, curTex.width, curTex.height), new Vector2(0.5f, 0.5f));
    }

    private string GetAlignAffect(int iSchool)
    {
        string sWhiteID = "Wardrobe_White";
        string sGrayID = "Wardrobe_Gray";
        string sShadowID = "Wardrobe_Shadow";

        if (iSchool > 0)
            return Oktagon.Localization.Lokaki.GetText(sWhiteID);
        else
        {
            if (iSchool < 0)
                return Oktagon.Localization.Lokaki.GetText(sShadowID);
        }

        return Oktagon.Localization.Lokaki.GetText(sGrayID);
    }

    public void SetupSignatureSpell(Bos_Signature_Data pSignature)
    {
        m_pTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_pCurrentSpell.id + "_title") + " - " + Oktagon.Localization.Lokaki.GetText(pSignature.effect + "_title");
        m_pCostSpellValue.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sCostSpellValueID), pSignature.cost);
        m_pSpellDescription.text = Oktagon.Localization.Lokaki.GetText(pSignature.effect + "_description");
    }

    private void ShowSignatureUI()
    {
        m_pManager.ShowSignatureUI(m_pCurrentSpell, m_lSignaturesList, this);
    }

    public void OnHorizontalDrag(Vector2 vInitPos, Vector2 vFinalPos)
    {
        if (!m_pManager.OnDragMovement())
            m_pManager.VerifyEndDrag(vInitPos, vFinalPos);
    }

    public void ForceDrag(float fDelta, float fInitValue)
    {
        m_pManager.m_pHorizontalbar.value = fInitValue + (fDelta * m_pManager.GetStepValue());  
    }
    
    public float GetHorizontalbarValue() { return m_pManager.m_pHorizontalbar.value; }

    private void UpdateLayout()
    {
        float fAspect = Camera.main.aspect;

        if ((fAspect == (16.0f / 9.0f)) || (fAspect == (16.0f / 10.0f)))
        {
            Debug.Log("Resolution 16/9 or 16/10");
            ChangeToResolution(new Vector2(1033.0f,410.0f), new Vector2(-270.0f,-127.0f), new Vector2(252.0f, -487.0f), new Vector2(-617.0f, 42.0f), new Vector2(0.2f,107.5f), 1000.0f);
        }

        if (fAspect == (3.0f / 2.0f))
        {
            Debug.Log("Resolution 3/2");
            ChangeToResolution(new Vector2(1033.0f, 410.0f), new Vector2(-270.0f, -127.0f), new Vector2(184.0f, -487.0f), new Vector2(-467.0f, 42.0f), new Vector2(0.2f, 107.5f), 950.0f);
        }

        if (fAspect == (4.0f / 3.0f))
        {
            Debug.Log("Resolution 4/3");
            ChangeToResolution(new Vector2(900.0f, 410.0f), new Vector2(-215.0f, -127.0f), new Vector2(184.0f, -487.0f), new Vector2(-434.0f, 42.0f), new Vector2(0.2f, 107.5f), 650.0f);
        }

        if (fAspect == (5.0f / 4.0f))
        {
            Debug.Log("Resolution 5/4");
            ChangeToResolution(new Vector2(780.0f, 410.0f), new Vector2(-215.0f, -127.0f), new Vector2(184.0f, -487.0f), new Vector2(-434.0f, 42.0f), new Vector2(0.2f, 107.5f), 650.0f);
        }
    }

    private void ChangeToResolution(Vector2 vContentSize, Vector2 vContentPos, Vector2 vCrestPos, Vector2 vGlyphPos, Vector2 vNamePos, float fNameWidth)
    {
        RectTransform pContentScrollView = m_pDescriptionScrollView.GetComponent<RectTransform>();
        RectTransform pContentParent = m_pDescriptionScrollView.transform.parent.GetComponent<RectTransform>();
        RectTransform pCrest = m_pCrest.GetComponent<RectTransform>();
        RectTransform pGlyph = m_pGlyphBaseImage.GetComponent<RectTransform>();
        RectTransform pName = m_pTitleLabel.GetComponent<RectTransform>();


        pContentScrollView.sizeDelta = vContentSize;
        pContentParent.anchoredPosition = vContentPos;

        pCrest.anchoredPosition = vCrestPos;
        pGlyph.anchoredPosition = vGlyphPos;

        Vector2 vLastSize = pName.sizeDelta;
        vLastSize.x = fNameWidth;
        pName.sizeDelta = vLastSize;
        pName.anchoredPosition = vNamePos;

        Vector2 vDescriptionSize = m_pSpellDescription.GetComponent<RectTransform>().sizeDelta;
        vDescriptionSize.x = vContentSize.x - 20.0f;
        m_pSpellDescription.GetComponent<RectTransform>().sizeDelta = vDescriptionSize;

        Canvas.ForceUpdateCanvases();
    }
}
