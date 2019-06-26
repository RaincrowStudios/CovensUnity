using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Color m_ActiveColor;
    [SerializeField] private Color m_DeactiveColor;
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_GlyphIcon;
    [SerializeField] private Image m_Fill;
    [SerializeField] private Image m_Frame;

    private SpellData m_Spell;
    private System.Action<UISpellcastingItem, SpellData> m_OnClick;
    private int m_TweenId;

    public bool Visible { get; private set; }

    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(CharacterMarkerDetail target , IMarker marker, SpellData spell, System.Action<UISpellcastingItem, SpellData> onClick)
    {
        m_Spell = spell;
        m_OnClick = onClick;

        m_CanvasGroup.alpha = 0;

        UpdateCanCast(target, marker);

        DownloadedAssets.GetSprite(spell.baseSpell,
            (spr) => 
            {
                m_GlyphIcon.overrideSprite = spr;
                m_GlyphIcon.gameObject.SetActive(true);
            });

        gameObject.SetActive(true);
    }

    public void UpdateCanCast(CharacterMarkerDetail targetData, IMarker targetMarker)
    {
        Spellcasting.SpellState canCast = Spellcasting.CanCast(m_Spell, targetMarker, targetData);

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
    }

    public void Prepare()
    {
        m_CanvasGroup.alpha = 0;
        gameObject.SetActive(true);
    }

    public void Show()
    {
        Visible = true;
        m_CanvasGroup.alpha = 0;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.8f)
            .setEaseOutCubic()
            .setOnStart(() => gameObject.SetActive(true))
            .uniqueId;
    }

    public void Hide()
    {
        Visible = false;
        LeanTween.cancel(m_TweenId);
        m_CanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        m_OnClick?.Invoke(this, m_Spell);
    }
}
