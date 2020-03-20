using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIQuickcastButtonOvermap : MonoBehaviour
{
    [SerializeField] private Button m_ButtonCastSpell;
    [SerializeField] private Image m_SpellIcon_A;
    [SerializeField] private Image m_SpellIcon_B;
    [SerializeField] private Image m_CooldownFill;
    [SerializeField] private Image m_Highlight;
    [SerializeField] private Image m_LockOverlay;
    [SerializeField] private TextMeshProUGUI m_SpellName;

    public string Spell { get; private set; }

    public bool Interactable
    {
        get => m_Interactable;
        set
        {
            m_Interactable = value;
            AnimateInteractable(value);
        }
    }

    public Spellcasting.SpellState CastStatus { get; private set; }
    private bool m_Interactable;

    private int m_CooldownTweenId;

    private void Awake()
    {
        CastStatus = Spellcasting.SpellState.InvalidSpell;
        Interactable = false;
        Hightlight(false);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_CooldownTweenId);
        StopAllCoroutines();
    }

    public void Setup(string spell, UnityEngine.Events.UnityAction onClick)
    {
        StopAllCoroutines();

        Spell = spell;
        m_ButtonCastSpell.onClick.AddListener(onClick);

        m_SpellIcon_A.overrideSprite = m_SpellIcon_B.overrideSprite = null;
        if (string.IsNullOrEmpty(Spell) == false)
        {
            m_SpellName.text = LocalizeLookUp.GetSpellName(Spell);
            DownloadedAssets.GetSprite(Spell, spr =>
            {
                m_SpellIcon_A.overrideSprite = m_SpellIcon_B.overrideSprite = spr;
                m_SpellIcon_A.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            });
        }
    }

    public void UpdateCanCast(IMarker marker, CharacterMarkerData details)
    {
        LeanTween.cancel(m_CooldownTweenId);

        if (string.IsNullOrEmpty(Spell))
        {
            CastStatus = Spellcasting.SpellState.InvalidSpell;
            Interactable = true;
            SetFillAmount(0);
            return;
        }

        CooldownManager.Cooldown? cd = CooldownManager.GetCooldown(Spell);
        if (cd.HasValue)
        {
            CastStatus = Spellcasting.SpellState.InCooldown;
            Interactable = false;

            m_CooldownTweenId = LeanTween.value(cd.Value.Remaining / cd.Value.total, 0, cd.Value.Remaining)
                .setOnUpdate((float t) => SetFillAmount(t))
                .setOnComplete(() => UpdateCanCast(marker, details))
                .uniqueId;
        }
        else if (marker == null || details == null)
        {
            CastStatus = Spellcasting.SpellState.InvalidTarget;
            Interactable = false;
            SetFillAmount(0);
        }
        else
        {
            CastStatus = Spellcasting.CanCast(Spell, marker, details);
            Interactable = CastStatus == Spellcasting.SpellState.CanCast;
            SetFillAmount(0);
        }
    }

    public void Hightlight(bool highlight)
    {
        m_Highlight.gameObject.SetActive(highlight);
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetFillAmount(float value)
    {
        m_SpellIcon_B.fillAmount = m_CooldownFill.fillAmount = value;
    }

    private void AnimateInteractable(bool interactable)
    {
        m_LockOverlay.gameObject.SetActive(!interactable);
    }
}
