using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private Color m_ActiveColor;
    [SerializeField] private Color m_DeactiveColor;
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_GlyphIcon;
    [SerializeField] private Image m_Fill;
    [SerializeField] private Image m_Frame;

    private IMarker m_Target;
    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;
    private System.Action<UISpellcastingItem, SpellData, SpellData, List<SpellData>> m_OnClick;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(IMarker target, SpellData spell, SpellData baseSpell, List<SpellData> signatures, System.Action<UISpellcastingItem, SpellData, SpellData, List<SpellData>> onClick)
    {
        m_Target = target;
        m_Spell = spell;
        m_BaseSpell = baseSpell;
        m_Signatures = signatures;
        m_OnClick = onClick;

        Spellcasting.SpellState canCast = Spellcasting.CanCast(spell, target);

        if (canCast == Spellcasting.SpellState.CanCast)
        {
            m_GlyphIcon.color = m_ActiveColor;
            m_Fill.color = new Color(1, 1, 1, 1);
            m_Frame.enabled = false;
        }
        else
        {
            m_GlyphIcon.color = m_DeactiveColor;
            m_Fill.color = new Color(1, 1, 1, 0.4f);
            m_Frame.enabled = true;
        }

        DownloadedAssets.GetSprite(spell.baseSpell,
            (spr) => 
            {
                m_GlyphIcon.sprite = spr;
                m_GlyphIcon.gameObject.SetActive(true);
            });

        gameObject.SetActive(true);
    }

    public void OnClick()
    {
        m_OnClick?.Invoke(this, m_Spell, m_BaseSpell, m_Signatures);
    }
}
