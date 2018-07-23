using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoSSignatureScreenUI : UIBaseAnimated
{
    public Text m_pTitle;
    public Transform m_pContent;
    public string m_sSignatureDescriptionPrefab;

    public ScrollRect m_pScroll;
    public float m_pScrollFactor = 0.1f;

    private BoS_Spell m_pCurrentSpell;
    private List<Bos_Signature_Spell_Data> m_pCurrentSignatureSpell;
    private BoSManagerUI m_pManager = null;

    #region LOKAKI_IDS
    private string m_sTitleID = "BoS_SignatureTitle";
    
    #endregion

    public void SetupUI(BoS_Spell pSpellData, List<Bos_Signature_Spell_Data> pSignatureSpells, BoSManagerUI pManager, BoSSpellScreenUI pSpellUI)
    {
        m_pCurrentSpell = pSpellData;
        m_pCurrentSignatureSpell = pSignatureSpells;
        m_pManager = pManager;

        m_pTitle.text = string.Format(Oktagon.Localization.Lokaki.GetText(m_sTitleID), Oktagon.Localization.Lokaki.GetText(pSpellData.id + "_title"));

        for (int i = 0; i < pSignatureSpells.Count; i++)
        {
            GameObject pSignatureDescr = GameObject.Instantiate(Resources.Load("BookOfShadows/" + m_sSignatureDescriptionPrefab), m_pContent.transform) as GameObject;

            pSignatureDescr.GetComponent<BoSSignatureDescription>().SetupDescription(pSpellData, pSignatureSpells[i], pSpellUI, this);
            pSignatureDescr.transform.localScale = Vector3.one;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pContent.GetComponent<RectTransform>());
    }

    public override void Close()
    {
        List<BoSSignatureDescription> lAllDescriptions = new List<BoSSignatureDescription>(m_pContent.GetComponentsInChildren<BoSSignatureDescription>());

        for (int i = 0; i < lAllDescriptions.Count; i++)
            GameObject.Destroy(lAllDescriptions[i].gameObject);

        base.Close();
    }

    public void RollUp()
    {
        m_pScroll.verticalScrollbar.value = m_pScroll.verticalScrollbar.value + m_pScrollFactor;
    }

    public void RollDown()
    {
        m_pScroll.verticalScrollbar.value = m_pScroll.verticalScrollbar.value - m_pScrollFactor;
    }

}
