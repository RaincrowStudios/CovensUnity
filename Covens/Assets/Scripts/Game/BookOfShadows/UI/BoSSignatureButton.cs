using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoSSignatureButton : MonoBehaviour
{
    public Bos_Signature_Data SpellData { get; set; }
    public BoSSpellScreenUI ReferenceUI { get; set; }

    public Color cNormalColor;
    public Color cSelectedColor;
    
    public void OnClickSignature()
    {
        if (ReferenceUI.SelectedButton != null)
            ReferenceUI.SelectedButton.GetComponentInChildren<Text>().color = cNormalColor;
;
        this.GetComponentInChildren<Text>().color = cSelectedColor;
        ReferenceUI.SelectedButton = this;
        ReferenceUI.SetupSignatureSpell(SpellData);
    }
}
