using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class BoSSignatureButton : MonoBehaviour
{
    public Bos_Signature_Spell_Data SpellData { get; set; }
    public BoSSpellScreenUI ReferenceUI { get; set; }

    public Color cNormalColor;
    public Color cSelectedColor;

    public JsonSignatureTeste pDictionaryTesting;

    public void OnClickSignature()
    {
        if (ReferenceUI.SelectedButton != null)
            ReferenceUI.SelectedButton.GetComponentInChildren<Text>().color = cNormalColor;
;
        this.GetComponentInChildren<Text>().color = cSelectedColor;
        ReferenceUI.SelectedButton = this;
        ReferenceUI.SetupSignatureSpell(SpellData);

        pDictionaryTesting = new JsonSignatureTeste();
        pDictionaryTesting.TestingJson = new Dictionary<string, Bos_Signature_Spell_Data>() { { "Test_ID", SpellData } };
        string jsonTxt = JsonConvert.SerializeObject(pDictionaryTesting.TestingJson, Formatting.Indented);

        Debug.Log(jsonTxt);
    }
}

public class JsonSignatureTeste
{
    public Dictionary<string, Bos_Signature_Spell_Data> TestingJson { get; set; }
}
