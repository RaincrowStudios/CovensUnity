using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpellcastingItem : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_Glyph;
    [SerializeField] private Image m_GlyphFill;
    [SerializeField] private Image m_Fill;

    private SpellData m_Spell;
    private System.Action<UISpellcastingItem, SpellData> m_OnClick;
    private int m_TweenId;
    private Coroutine m_CooldownCoroutine = null;

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
                m_Glyph.overrideSprite = m_GlyphFill.overrideSprite = spr;
            });

        gameObject.SetActive(true);
    }

    public void UpdateCanCast(CharacterMarkerDetail targetData, IMarker targetMarker)
    {
        if (m_CooldownCoroutine != null)
        {
            StopCoroutine(m_CooldownCoroutine);
            m_CooldownCoroutine = null;
        }

        Spellcasting.SpellState canCast = Spellcasting.CanCast(m_Spell, targetMarker, targetData);

        if (canCast == Spellcasting.SpellState.CanCast)
        {
            m_Fill.fillAmount = m_GlyphFill.fillAmount = 1;
        }
        else
        {
            m_Fill.fillAmount = m_GlyphFill.fillAmount = 0;

            if (canCast == Spellcasting.SpellState.InCooldown)
                m_CooldownCoroutine = StartCoroutine(CooldownCoroutine());
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

    private IEnumerator CooldownCoroutine()
    {
        CooldownManager.Cooldown? cooldown = CooldownManager.GetCooldown(m_Spell.id);
        
        float secondsRemaining = cooldown.HasValue ? (float)Utilities.TimespanFromJavaTime(cooldown.Value.timestamp).TotalSeconds : 0;

        while (secondsRemaining >= 0)
        {
            m_Fill.fillAmount = m_GlyphFill.fillAmount = 1 - secondsRemaining / cooldown.Value.duration;

            yield return new WaitForSeconds(0.1f);
            secondsRemaining -= 0.1f;
        }

        m_Fill.fillAmount = m_GlyphFill.fillAmount = 1;
    }
}
