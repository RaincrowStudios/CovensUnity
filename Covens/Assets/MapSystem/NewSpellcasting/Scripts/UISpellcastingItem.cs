using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_Text;

    public void Setup(string baseSpellId, SpellData baseSpell, List<SpellData> signatures)
    {

    }
}
