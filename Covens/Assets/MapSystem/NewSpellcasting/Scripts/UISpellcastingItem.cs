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

    private MarkerDataDetail m_Target;
    private SpellData m_Spell;
    private System.Action<UISpellcastingItem, SpellData> m_OnClick;
    private int m_TweenId;

    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(MarkerDataDetail target , IMarker marker, SpellData spell, System.Action<UISpellcastingItem, SpellData> onClick)
    {
        m_Target = target;
        m_Spell = spell;
        m_OnClick = onClick;

        m_CanvasGroup.alpha = 0;

        Spellcasting.SpellState canCast = Spellcasting.CanCast(spell, marker, target);

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
                m_GlyphIcon.overrideSprite = spr;
                m_GlyphIcon.gameObject.SetActive(true);
            });

        gameObject.SetActive(true);
    }

    public void Show()
    {
        m_CanvasGroup.alpha = 0;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f).setEaseOutCubic().setOnStart(() => gameObject.SetActive(true)).uniqueId;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //if (gameObject.activeSelf == false)
        //    return;

        //LeanTween.cancel(m_TweenId, true);
        //m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.15f).setEaseOutCubic().setOnComplete(() => gameObject.SetActive(false)).uniqueId;
    }

    public void OnClick()
    {
        m_OnClick?.Invoke(this, m_Spell);
    }
}
