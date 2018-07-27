using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSLandingScreenUI : UIBaseAnimated /*, IBeginDragHandler, IEndDragHandler, IDragHandler*/
{
    [Header("General UI Data")]
    public Text m_pTitleLabel;
    public Text m_pDegreeAndSchoolLabel;
    public Image m_pCrestImage;
    public Image m_pPathImage;
    public Text m_pPathLabel;
    public VerticalLayoutGroup m_pContentInfo;
    public ScrollRect m_pContentScrollView;

    [Header("Coven")]
    public Text m_pCovenRankLabel;
    public Text m_pCovenTitleLabel;
    public Text m_pCovenNameLabel;

    [Header("World Rank")]
    public Text m_pWorldRankLabel;
    public Text m_pWorldRankValue;
    public Text m_pWorldRankComplement;
    public Text m_pWorldButtonLabel;

    [Header("Dominion")]
    public Text m_pDominionRankLabel;
    public Text m_pDominionRankValue;
    public Text m_pDominionRankComplement;
    public Text m_pDominionLabel;

    [Header("Favorite Spell")]
    public Text m_pFavoriteSpellTitleLabel;
    public Text m_pFavoriteSpellLabel;

    [Header("Nemesis")]
    public Text m_pNemesisTitleLabel;
    public Text m_pNemesisLabel;

    [Header("Benefactor")]
    public Text m_pBenefactorTitleLabel;
    public Text m_pBenefactorLabel;

    //public HorizontalLayoutGroup m_pNavigator;
    //public string m_sNavMarkPrefab;

    private int m_iFavoriteSpellIndex = 0;
    //private List<GameObject> m_lNavMarkCreated;

    #region GET/SET
    public BoSManagerUI Manager{ get; set; }
    #endregion

    #region Lokaki IDs
    private string m_sTitleID = "BoS_LandingTitle";
    private string m_sDegreeSchoolID = "BoS_DegreeSchool";
    private string m_sCovenInfoID = "BoS_CovenInfo";
    private string m_sWorldRankID = "BoS_WorldRank";
    private string m_sRankID = "BoS_Rank";
    private string m_sDominionComplementID = "BoS_DominionRank";
    private string m_sFavoriteSpellID = "BoS_FavoriteSpell";
    private string m_sNemesisID = "BoS_Nemesis";
    private string m_sBenefactorID = "BoS_Benefactor";
    private string m_sPathID = "BoS_Path";
    private string m_sWorldButtonID = "BoS_WorldButton";
    #endregion

    protected override void Awake()
    {
        Manager = FindObjectOfType<BoSManagerUI>();
        base.Awake();
    }

    public void SetupUI(BookOfShadows_Display pData)
    {
        m_pTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sTitleID);

        int iDegree = PlayerDataManager.playerData.degree;
        m_pDegreeAndSchoolLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sDegreeSchoolID), Mathf.Abs(iDegree), GetOrdinalNumberSuffix(Mathf.Abs(iDegree)), GetPlayerAlignment(iDegree));

        m_pCrestImage.sprite = GetCrestSprite(iDegree);
        m_pPathImage.sprite = GetPathSprite(iDegree);

        // Setting Coven Data---------------------
        if (string.IsNullOrEmpty(pData.covenName))
        {
            m_pCovenTitleLabel.text = "";
            m_pCovenNameLabel.text = "Not in Coven!";
        }
        else
        {
            m_pCovenRankLabel.text = Oktagon.Localization.Lokaki.GetText(pData.covenTitle);
            m_pCovenTitleLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sCovenInfoID));
            m_pCovenNameLabel.text = pData.covenName;
        }

        //Setting World Rank
        m_pWorldRankLabel.text = Oktagon.Localization.Lokaki.GetText(m_sRankID);
        m_pWorldRankValue.text = string.Format("{0}", pData.worldRank);
        m_pWorldRankComplement.text = Oktagon.Localization.Lokaki.GetText(m_sWorldRankID);
        m_pWorldButtonLabel.text = Oktagon.Localization.Lokaki.GetText(m_sWorldButtonID);


        //Setting Dominion Data-----------------
        m_pDominionRankLabel.text = Oktagon.Localization.Lokaki.GetText(m_sRankID);
        m_pDominionRankValue.text = string.Format("{0}", pData.dominionRank);
        m_pDominionRankComplement.text = Oktagon.Localization.Lokaki.GetText(m_sDominionComplementID);
        m_pDominionLabel.text = PlayerDataManager.playerData.dominion;
        
        
        //Setting Favorite Spell--------------------
        m_pFavoriteSpellTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sFavoriteSpellID);
        m_pFavoriteSpellLabel.text = Oktagon.Localization.Lokaki.GetText(pData.favoriteSpell + "_title");
        m_iFavoriteSpellIndex = pData.spells.IndexOf(pData.spells.Find(x => x.id.Equals(pData.favoriteSpell))) + 1;

        //Setting Nemesis-----------------------------
        m_pNemesisTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sNemesisID);
        m_pNemesisLabel.text = pData.nemesis;

        //Setting Benefactor-------------------------
        m_pBenefactorTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sBenefactorID);
        m_pBenefactorLabel.text = pData.benefactor;

        m_pPathLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sPathID), GetPlayerAlignment(iDegree));

        /*
        m_lNavMarkCreated = new List<GameObject>();

        for (int i = 0; i < pData.spells.Count; i++)
        {
            GameObject pNavMark = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sNavMarkPrefab), m_pNavigator.transform) as GameObject;

            if (i == 0)
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.43137255f);
            else
                pNavMark.GetComponent<Image>().color = new Color(0.117647f, 0.117647f, 0.117647f, 0.17647f);

            pNavMark.transform.localScale = Vector3.one;
            m_lNavMarkCreated.Add(pNavMark);
        }
        */

        UpdateLayout();

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pFavoriteSpellLabel.transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pCovenNameLabel.transform.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pContentInfo.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pContentInfo.transform.parent.GetComponent<RectTransform>());
    }

    public void OnError(string sResponse)
    {
        Debug.Log("Getting Book of Shadows Data: Faliure");
        Debug.Log("Message: " + sResponse);
    }
	
    public void GoToFavoriteSpell()
    {
        Manager.GoToPageImmediately(m_iFavoriteSpellIndex);
    }

    /*
    public void ResetUI()
    {
        for (int i = 0; i < m_lNavMarkCreated.Count; i++)
            GameObject.Destroy(m_lNavMarkCreated[i]);

        m_lNavMarkCreated.Clear();
    }
    */

    private string GetOrdinalNumberSuffix(int iNumber)
    {
        if (iNumber == 0)
            return "";

        int iSuffix;

        if (iNumber > 10)
            iSuffix = iNumber % 10;
        else
            iSuffix = iNumber;

        if (iSuffix == 1)
            return "st";

        if (iSuffix == 2)
            return "nd";

        if (iSuffix == 3)
            return "rd";

        return "th";
    }

    private string GetPlayerAlignment(int iDegree)
    {
        string sWhiteID = "Wardrobe_White";
        string sGrayID = "Wardrobe_Gray";
        string sShadowID = "Wardrobe_Shadow";

        if (iDegree < 0)
            return Oktagon.Localization.Lokaki.GetText(sShadowID);

        if (iDegree > 0)
            return Oktagon.Localization.Lokaki.GetText(sWhiteID);

        return Oktagon.Localization.Lokaki.GetText(sGrayID);
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

        return Sprite.Create(curTex, new Rect(0,0,curTex.width,curTex.height), new Vector2(0.5f, 0.5f));
    }

    private Sprite GetPathSprite(int iDegree)
    {
        string sSpritePath = "BookOfShadows/SpritesUI/";

        string sLightPathID = "LightPath";
        string sGrayPathID = "GrayPath";
        string sShadowPathID = "ShadowPath";

        Texture2D curTex = null;

        if (iDegree > 0)
            curTex = Resources.Load(sSpritePath + sLightPathID) as Texture2D;
        else
        {
            if (iDegree < 0)
                curTex = Resources.Load(sSpritePath + sShadowPathID) as Texture2D;
            else
                curTex = Resources.Load(sSpritePath + sGrayPathID) as Texture2D;
        }

        return Sprite.Create(curTex, new Rect(0, 0, curTex.width, curTex.height), new Vector2(0.5f, 0.5f));
    }

    private void UpdateLayout()
    {
        float fAspect = Camera.main.aspect;

        if (fAspect == (16.0f / 9.0f))
        {
            //Debug.Log("Resolution 16/9");
            ChangeToResolution(new Vector2(1150.0f, 575.0f), new Vector2(-215.0f, -172.0f), 252.0f, -617.0f);
        }


        if (fAspect == (16.0f / 10.0f))
        {
            //Debug.Log("Resolution 16/10");
            ChangeToResolution(new Vector2(972.0f, 575.0f), new Vector2(-261.0f, -172.0f), 252.0f, -617.0f);
        }

        if (fAspect == (3.0f/2.0f))
        {
            //Debug.Log("Resolution 3/2");
            ChangeToResolution(new Vector2(828.0f, 575.0f), new Vector2(-261.0f, -172.0f), 252.0f, -617.0f);
        }

        if (fAspect == (4.0f / 3.0f))
        {
            //Debug.Log("Resolution 4/3");
            ChangeToResolution(new Vector2(750.0f, 575.0f), new Vector2(-281.0f, -172.0f), 170.0f, -566.0f);
        }

        if (fAspect == (5.0f/4.0f))
        {
            //Debug.Log("Resolution 5/4");
            ChangeToResolution(new Vector2(640.0f, 575.0f), new Vector2(-286.6f, -172.0f), 170.0f, -566.0f);
        }
    }

    private void ChangeToResolution(Vector2 vContentSize, Vector2 vContentPosition, float fCrestPosX, float fPathPosX)
    {
        RectTransform pContentScrollView = m_pContentScrollView.GetComponent<RectTransform>();
        RectTransform pContentInfo = m_pContentInfo.GetComponent<RectTransform>();
        RectTransform pCrest = m_pCrestImage.GetComponent<RectTransform>();
        RectTransform pPathArea = m_pPathLabel.transform.parent.GetComponent<RectTransform>();

        pContentScrollView.sizeDelta = vContentSize;
        pContentScrollView.anchoredPosition = vContentPosition;

        Vector2 vLastSize = pContentInfo.sizeDelta;
        vLastSize.x = vContentSize.x - 20.0f;
        pContentInfo.sizeDelta = vLastSize;

        m_pContentScrollView.verticalScrollbar.value = 1.0f;
        Canvas.ForceUpdateCanvases();

        Vector3 vCrestPosition = pCrest.anchoredPosition;
        vCrestPosition.x = fCrestPosX;
        pCrest.anchoredPosition = vCrestPosition;

        Vector3 vPathPosition = pPathArea.anchoredPosition;
        vPathPosition.x = fPathPosX;
        pPathArea.anchoredPosition = vPathPosition;
    }

    public void ForceDrag(float fDelta, float fInitValue)
    {
        Manager.m_pHorizontalbar.value = fInitValue + (fDelta * Manager.GetStepValue());
    }

    public void OnHorizontalDrag(Vector2 vInitPos, Vector2 vFinalPos)
    {
        if (!Manager.OnDragMovement())
            Manager.VerifyEndDrag(vInitPos, vFinalPos);
    }

    public float GetHorizontalbarValue() { return Manager.m_pHorizontalbar.value; }
}
