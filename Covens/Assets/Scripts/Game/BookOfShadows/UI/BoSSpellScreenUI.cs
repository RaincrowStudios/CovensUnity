using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSSpellScreenUI : UIBaseAnimated
{
    public Text m_pTitleLabel;

    public Text m_pCostSpellLabel;
    public Text m_pCostSpellValue;

    public Text m_pSpellDescription;
    public Image m_pCrest;
    public Text m_pAlignAffectedLabel;

    public HorizontalLayoutGroup m_pNavigator;
    public string m_sNavMarkPrefab;
    public Button m_pCloseButton;
    public Button m_pSignatureButton;

    #region LOKAKI IDS
    private string m_sCostSpellLabelID = "BoS_SpellCost";
    private string m_sCostSpellValueID = "BoS_SpellCostValue";
    private string m_sSpellAlignAffectedID = "BoS_SpellAlignAffected";
    #endregion


    public void SetupUI(BookOfShadows_Display pData, int iIndex, BoSManagerUI pParent)
    {
        m_pTitleLabel.text = Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_title");

        m_pCostSpellLabel.text = Oktagon.Localization.Lokaki.GetText(m_sCostSpellLabelID);
        m_pCostSpellValue.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sCostSpellValueID), pData.spells[iIndex].cost);

        m_pSpellDescription.text = Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_description");

        m_pCrest.sprite = GetCrestSprite(PlayerDataManager.playerData.degree);

        if (iIndex > 0)
            m_pAlignAffectedLabel.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sSpellAlignAffectedID), Oktagon.Localization.Lokaki.GetText(pData.spells[iIndex].id + "_title"), GetAlignAffect(pData.spells[iIndex].school));
        else
            m_pAlignAffectedLabel.text = ""; //Invisibility Spell


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

        m_pCloseButton.onClick.AddListener(delegate{ pParent.Close(); });
        m_pSignatureButton.onClick.AddListener(delegate { pParent.ShowSignatureUI(); });
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

}
