using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIQuickcastButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image m_Background;
    [SerializeField] private Image m_SpellIcon_A;
    [SerializeField] private Image m_SpellIcon_B;
    [SerializeField] private Image m_CooldownFill;
    [SerializeField] private Image m_Highlight;
    [SerializeField] private Image m_LockOverlay;
    [SerializeField] private TextMeshProUGUI m_SpellName;

    public string Spell { get; private set; }
    public int QuickcastIndex { get; private set; }

    private bool m_Interactable;
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
    private System.Action m_OnClick;
    private System.Action m_OnHold;

    private bool m_PointerDown;
    private bool m_PointerInside;

    private Color m_BackgroundColor;
    private Color m_IconColor_A;
    private Color m_IconColor_B;
    private Color m_FillColor;

    private int m_CooldownTweenId;

    private void Awake()
    {
        m_BackgroundColor = m_Background.color;
        m_IconColor_A = m_SpellIcon_A.color;
        m_IconColor_B = m_SpellIcon_B.color;
        m_FillColor = m_CooldownFill.color;
        CastStatus = Spellcasting.SpellState.InvalidSpell;
        Interactable = false;
        Hightlight(false);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_CooldownTweenId);
        StopAllCoroutines();
    }

    public void Setup(int index, System.Action onClick, System.Action onHold)
    {
        StopAllCoroutines();

        Spell = PlayerManager.GetQuickcastSpell(index);
        QuickcastIndex = index;
        m_OnClick = onClick;
        m_OnHold = onHold;

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
            SpellData spell = DownloadedAssets.GetSpell(Spell);
            CastStatus = Spellcasting.CanCast(Spell, marker, details);
            Interactable = CastStatus == Spellcasting.SpellState.CanCast && spell.beneficial;
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

    public void OnPointerDown(PointerEventData eventData)
    {
        m_PointerDown = true;
        StartCoroutine(WaitPointerUpCoroutine());

        AnimateButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_PointerDown = false;
        AnimateButtonUp();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_PointerInside = true;
        if (m_PointerDown)
            AnimateButtonDown();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_PointerInside = false;
        AnimateButtonUp();
    }

    private void AnimateButtonDown()
    {
        m_Background.color = m_BackgroundColor * 0.5f;
        m_SpellIcon_A.color = m_IconColor_A * 0.5f;
        m_SpellIcon_B.color = m_IconColor_B * 0.5f;
        m_CooldownFill.color = m_FillColor * 0.5f;
    }

    private void AnimateButtonUp()
    {
        m_Background.color = m_BackgroundColor;
        m_SpellIcon_A.color = m_IconColor_A;
        m_SpellIcon_B.color = m_IconColor_B;
        m_CooldownFill.color = m_FillColor;
    }

    private void AnimateInteractable(bool interactable)
    {
        m_LockOverlay.gameObject.SetActive(!interactable);
    }

    private IEnumerator WaitPointerUpCoroutine()
    {
        float time = 0;
        while (m_PointerDown)
        {
            yield return 0;
            time += Time.unscaledDeltaTime;

            if (time > 0.5f)
            {
                m_OnHold?.Invoke();
                yield break;
            }
        }

        if (time < 0.25f)
            m_OnClick?.Invoke();
    }
}
