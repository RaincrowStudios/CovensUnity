using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoSSignatureScreenUI : UIBaseAnimated
{
    [Header("General UI Data")]
    public Text m_pTitle;
    public Transform m_pContent;
    public string m_sSignatureDescriptionPrefab;

    public ScrollRect m_pScroll;
    public float m_pScrollFactor = 0.1f;

    private BoS_Spell m_pCurrentSpell;
    private List<Bos_Signature_Data> m_pCurrentSignatureSpell;
    private BoSManagerUI m_pManager = null;

    #region LOKAKI_IDS
    private string m_sTitleID = "BoS_SignatureTitle";
    
    #endregion

    public void SetupUI(BoS_Spell pSpellData, List<Bos_Signature_Data> pSignatureSpells, BoSManagerUI pManager, BoSSpellScreenUI pSpellUI)
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

        UpdateLayout();
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_pContent.GetComponent<RectTransform>());

        StartCoroutine(RollVerticalInit()); //Set vertical scroll to init position
    }

    private IEnumerator RollVerticalInit()
    {
        yield return new WaitForSeconds(0.15f);
        m_pScroll.verticalScrollbar.value = 1.0f;
        Canvas.ForceUpdateCanvases();
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

    private void UpdateLayout()
    {
        float fAspect = Camera.main.aspect;

        if (fAspect == (16.0f / 9.0f))
        {
            //Debug.Log("Resolution 16/9");
            ChangeToResolution(new Vector2(1860.5f, 905.3f));
        }


        if (fAspect == (16.0f / 10.0f))
        {
            //Debug.Log("Resolution 16/10");
            ChangeToResolution(new Vector2(1610.0f, 905.3f));
        }

        if (fAspect == (3.0f / 2.0f))
        {
            //Debug.Log("Resolution 3/2");
            ChangeToResolution(new Vector2(1450.0f, 905.3f));
        }

        if (fAspect == (4.0f / 3.0f))
        {
            //Debug.Log("Resolution 4/3");
            ChangeToResolution(new Vector2(1200.0f, 905.3f));
        }

        if (fAspect == (5.0f / 4.0f))
        {
            //Debug.Log("Resolution 5/4");
            ChangeToResolution(new Vector2(1100.0f, 905.3f));
        }
    }

    private void ChangeToResolution(Vector2 vContentSize)
    {
        RectTransform pContentScrollView = m_pScroll.GetComponent<RectTransform>();
        pContentScrollView.sizeDelta = vContentSize;

        BoSSignatureDescription[] lAllElements = m_pContent.GetComponentsInChildren<BoSSignatureDescription>();
        for (int i = 0; i < lAllElements.Length; i++)
        {
            Vector2 vLastSize = lAllElements[i].m_pInfo1.GetComponent<RectTransform>().sizeDelta;
            vLastSize.x = vContentSize.x - 20.0f;
            lAllElements[i].m_pInfo1.GetComponent<RectTransform>().sizeDelta = vLastSize;

            vLastSize = lAllElements[i].m_pInfo2.sizeDelta;
            vLastSize.x = vContentSize.x - 20.0f;
            lAllElements[i].m_pInfo2.sizeDelta = vLastSize;
        }

        Canvas.ForceUpdateCanvases();
    }
}
