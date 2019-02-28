using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private Color m_ActiveColor;
    [SerializeField] private Color m_DeactiveColor;
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_GlyphIcon;
    [SerializeField] private Image m_Active;
    [SerializeField] private Image m_Deactive;

    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;
    private System.Action<UISpellcastingItem, SpellData, SpellData, List<SpellData>> m_OnClick;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(SpellData spell, SpellData baseSpell, List<SpellData> signatures, System.Action<UISpellcastingItem, SpellData, SpellData, List<SpellData>> onClick)
    {
        m_Spell = spell;
        m_BaseSpell = baseSpell;
        m_Signatures = signatures;
        m_OnClick = onClick;

        Spellcasting.SpellState canCast = Spellcasting.CanCast(spell);
        if (canCast == Spellcasting.SpellState.CanCast)
        {
            m_GlyphIcon.color = m_ActiveColor;
            m_Active.enabled = true;
            m_Deactive.enabled = false;
            m_Button.interactable = true;
        }
        else if (canCast == Spellcasting.SpellState.MissingIngredients)
        {
            m_GlyphIcon.color = m_DeactiveColor;
            //m_Active.enabled = false;
            m_Deactive.enabled = true;
            m_Button.interactable = false;
        }

        DownloadedAssets.GetSprite(spell.baseSpell,
            (spr) => 
            {
                m_GlyphIcon.sprite = spr;
                m_GlyphIcon.gameObject.SetActive(true);
            });

        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        m_OnClick?.Invoke(this, m_Spell, m_BaseSpell, m_Signatures);
    }
}
