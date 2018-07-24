using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSLandingScreenUI : UIBaseAnimated
{
    public Text m_pTitleLabel;
    public Text m_pDegreeAndSchoolLabel;

    public Image m_pCrestImage;
    public Image m_pPathImage;
    public Text m_pCovenNameLabel;
    public Text m_pCovenTitleLabel;
    public Text m_pWorldRankLabel;
    public Text m_pWorldButtonLabel;
    public Text m_pDominionRankLabel;
    public Text m_pDominionLabel;
    public Text m_pFavoriteSpellTitleLabel;
    public Text m_pFavoriteSpellLabel;
    public Text m_pNemesisTitleLabel;
    public Text m_pNemesisLabel;
    public Text m_pBenefactorTitleLabel;
    public Text m_pBenefactorLabel;
    public Text m_pPathLabel;
    //public HorizontalLayoutGroup m_pNavigator;
    //public string m_sNavMarkPrefab;

    public VerticalLayoutGroup m_pContentInfo;

    public BoSManagerUI Manager{ get; set; }
    private int m_iFavoriteSpellIndex = 0;

    #region Lokaki IDs
    private string m_sTitleID = "BoS_LandingTitle";
    private string m_sDegreeSchoolID = "BoS_DegreeSchool";
    private string m_sCovenInfoID = "BoS_CovenInfo";
    private string m_sWorldRankID = "BoS_WorldRank";
    private string m_sDomininionRankID = "BoS_DominionRank";
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

    //private List<GameObject> m_lNavMarkCreated;
    

    public void SetupUI(BookOfShadows_Display pData)
    {
        m_pTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sTitleID);

        int iDegree = PlayerDataManager.playerData.degree;
        m_pDegreeAndSchoolLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sDegreeSchoolID), Mathf.Abs(iDegree), GetOrdinalNumberSuffix(Mathf.Abs(iDegree)), GetPlayerAlignment(iDegree));

        m_pCrestImage.sprite = GetCrestSprite(iDegree);
        m_pPathImage.sprite = GetPathSprite(iDegree);

        if (string.IsNullOrEmpty(pData.covenName))
        {
            m_pCovenTitleLabel.text = "";
            m_pCovenNameLabel.text = "Not in Coven!";
        }
        else
        {
            m_pCovenTitleLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sCovenInfoID), pData.covenTitle);
            m_pCovenNameLabel.text = pData.covenName;
        }

        m_pWorldRankLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sWorldRankID), pData.worldRank);
        m_pWorldButtonLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sWorldButtonID));

        m_pDominionRankLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sDomininionRankID), pData.dominionRank);
        m_pDominionLabel.text = PlayerDataManager.playerData.dominion;

        m_pFavoriteSpellTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sFavoriteSpellID);
        m_pFavoriteSpellLabel.text = Oktagon.Localization.Lokaki.GetText(pData.favoriteSpell + "_title");
        m_iFavoriteSpellIndex = pData.spells.IndexOf(pData.spells.Find(x => x.id.Equals(pData.favoriteSpell))) + 1;

        m_pNemesisTitleLabel.text = Oktagon.Localization.Lokaki.GetText(m_sNemesisID);
        m_pNemesisLabel.text = pData.nemesis;

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pFavoriteSpellLabel.transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pCovenNameLabel.transform.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pContentInfo.GetComponent<RectTransform>());
    }

    public void OnError(string sResponse)
    {
        Debug.Log("Getting Book of Shadows Data: Faliure");
        Debug.Log("Message: " + sResponse);
    }
	
    public void GoToFavoriteSpell()
    {
        //Debug.Log("Go To " + m_iFavoriteSpellIndex.ToString() + " Element");
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
}
