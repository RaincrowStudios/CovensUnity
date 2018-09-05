using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoSSignatureDescription : MonoBehaviour
{

    public Text m_pInfo1;
    public RectTransform m_pInfo2;
    public Text[] m_pIngredients;
    public Text m_pComplement;
    public Text m_pEffectType;
    public Text m_pEffectName;
    public Button m_pEffectButton;

    private BoSSpellScreenUI m_pSpellUI = null;
    private Bos_Signature_Data m_pSignatureData = null;
    private BoSSignatureScreenUI m_pSignatureUI = null;

    #region LOKAKI_IDS
    private string m_sInfo1ID = "BoS_Info1";
    private string m_sIngredientsID = "BoS_IngredientsList";
    private string m_sComplementID = "BoS_SignatureComplementText";
    private string m_sEffectTypeID = "BoS_SignatureEffectType";
    private string m_sEffectNameID = "BoS_SignatureEffectName";
    #endregion

    public void SetupDescription(BoS_Spell pSpellData, Bos_Signature_Data pCurSignatureSpell, BoSSpellScreenUI pSpellUI, BoSSignatureScreenUI pSignatureUI)
    {
        DateTime pCurTime = new DateTime(pCurSignatureSpell.timestamp);
        m_pInfo1.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sInfo1ID),
                                      pCurTime.DayOfYear.ToString() + GetOrdinalNumberSuffix(pCurTime.DayOfYear),
                                      pCurTime.Year.ToString(),
                                      Oktagon.Localization.Lokaki.GetText(pCurSignatureSpell.tribunal),
                                      Oktagon.Localization.Lokaki.GetText(pSpellData.id + "_title")
            );

        for (int i = 0; i < m_pIngredients.Length; i++)
        {
            if (i < pCurSignatureSpell.ingredients.Count)
            {
                m_pIngredients[i].gameObject.SetActive(true);
                string sResultStr = string.Format("{0} {1}.", pCurSignatureSpell.ingredients[i].count, Oktagon.Localization.Lokaki.GetText(pCurSignatureSpell.ingredients[i].id));
                m_pIngredients[i].text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sIngredientsID), sResultStr);
            }
            else
                m_pIngredients[i].gameObject.SetActive(false);
        }

        //m_pIngredients.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sIngredientsID), GetIngredients(pCurSignatureSpell.ingredients));
        m_pComplement.text = Oktagon.Localization.Lokaki.GetText(m_sComplementID);
        m_pEffectType.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sEffectTypeID), GetPositiveOrNegativeSpell(pCurSignatureSpell.effect));
        m_pEffectName.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sEffectNameID), GetEffect(pCurSignatureSpell.effect));

        m_pSpellUI = pSpellUI;
        m_pSignatureData = pCurSignatureSpell;
        m_pSignatureUI = pSignatureUI;

        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
    }

    private string GetOrdinalNumberSuffix(int iNumber)
    {
        int iSuffix;

        if (iNumber > 100)
        {
            iSuffix = iNumber % 100;
        }
        else
        {
            if (iNumber > 10)
                iSuffix = iNumber % 10;
            else
                iSuffix = iNumber;
        }

        if (iSuffix == 1)
            return "st";

        if (iSuffix == 2)
            return "nd";

        if (iSuffix == 3)
            return "rd";

        return "th";
    }

    /*
    private string GetIngredients(List<BoS_Signature_Spell_Ingredient> lAllIngredients)
    {
        string sResultStr = "";

        for (int i = 0; i < lAllIngredients.Count; i++)
        {
            sResultStr += string.Format("{0} {1}. ", lAllIngredients[i].count, Oktagon.Localization.Lokaki.GetText(lAllIngredients[i].id));
        }

        return sResultStr;
    }
    */

    private string GetPositiveOrNegativeSpell(string sCurID)
    {
        string sPositiveID = "Signature_Positive";
        string sNegativeID = "Signature_Negative";

        if (sCurID.Contains("spell_"))
            return Oktagon.Localization.Lokaki.GetText(sPositiveID);

        return Oktagon.Localization.Lokaki.GetText(sNegativeID);
    }

    private string GetEffect(string sCurID)
    {
        if (sCurID.Contains("spell_"))
            return string.Format("<color=black>{0}!</color>", Oktagon.Localization.Lokaki.GetText(sCurID + "_title"));

        return string.Format("<color=red>{0}!</color>", Oktagon.Localization.Lokaki.GetText(sCurID + "_title"));
    }

    public void ReturnToSpellUI()
    {
        m_pSpellUI.lSignatureAvailableButtons.Find(x => x.SpellData.Equals(m_pSignatureData)).OnClickSignature();
        m_pSignatureUI.Close();
    }
}
