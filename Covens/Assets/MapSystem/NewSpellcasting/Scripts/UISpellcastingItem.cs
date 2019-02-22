using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_Title;

    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;
    private System.Action<SpellData, SpellData, List<SpellData>> m_OnClick;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(SpellData spell, SpellData baseSpell, List<SpellData> signatures, System.Action<SpellData, SpellData, List<SpellData>> onClick)
    {
        m_Spell = spell;
        m_BaseSpell = baseSpell;
        m_Signatures = signatures;
        m_OnClick = onClick;

        m_Title.text = spell.displayName;
        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        m_OnClick?.Invoke(m_Spell, m_BaseSpell, m_Signatures);
    }
}
