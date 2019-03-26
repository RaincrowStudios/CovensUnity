using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpellcastingInfo : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_SpellName;
    [SerializeField] private TextMeshProUGUI m_SpellCost;
    [SerializeField] private TextMeshProUGUI m_SpellDesc;
    [SerializeField] private Button m_CastButton;

    private MarkerDataDetail m_Target;
    private IMarker m_Marker;
    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;
    private int m_TweenId;

    private System.Action<SpellData> m_OnConfirmSpellcast;

    private void Awake()
    {
        m_CastButton.onClick.AddListener(OnClickCast);
    }

    public void Show(MarkerDataDetail target, IMarker marker, SpellData spell, System.Action<SpellData> onCast)
    {
        m_Target = target;
        m_Spell = spell;
        m_Marker = marker;
        m_OnConfirmSpellcast = onCast;

        m_SpellName.text = spell.displayName;
        m_SpellCost.text = $"({spell.cost} Energy)";
        m_SpellDesc.text = spell.description;

        UpdateCanCast();

        gameObject.SetActive(true);
        m_CanvasGroup.alpha = 0;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1.25f).setEaseOutCubic().uniqueId;
    }

    public void UpdateCanCast()
    {
        Spellcasting.SpellState canCast = Spellcasting.CanCast(m_Spell, m_Marker, m_Target);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        TextMeshProUGUI castText = m_CastButton.GetComponent<TextMeshProUGUI>();

        if (canCast == Spellcasting.SpellState.TargetImmune)
            castText.text = "Witch is immune";
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
            castText.text = "You are silenced";
        else if (canCast == Spellcasting.SpellState.MissingIngredients)
            castText.text = "Missing ingredients";
        else if (canCast == Spellcasting.SpellState.CanCast)
            castText.text = "Cast " + m_Spell.displayName;
        else
            castText.text = "Can't cast on " + m_Target.displayName;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnClickCast()
    {
        m_OnConfirmSpellcast?.Invoke(m_Spell);
    }
}