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
    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;
    private int m_TweenId;

    public System.Action<SpellData, List<spellIngredientsData>> onConfirmSpellcast;

    private void Awake()
    {
        m_CastButton.onClick.AddListener(OnClickCast);

        UISpellcastingIngredients.onConfirmIngredients += OnConfirmIngredients;
    }

    public void Show(MarkerDataDetail target, IMarker marker, SpellData spell, SpellData baseSpell, List<SpellData> signatures)
    {
        m_Target = target;
        m_Spell = spell;
        m_BaseSpell = baseSpell;
        m_Signatures = signatures;

        m_SpellName.text = spell.displayName;
        m_SpellCost.text = $"({spell.cost} Energy)";
        m_SpellDesc.text = spell.description;

        Spellcasting.SpellState canCast = Spellcasting.CanCast(spell, marker, target);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        TextMeshProUGUI castText = m_CastButton.GetComponent<TextMeshProUGUI>();

        if (canCast == Spellcasting.SpellState.InvalidState)
            castText.text = "Can't cast on " + target.displayName;
        else if (canCast == Spellcasting.SpellState.MissingIngredients)
            castText.text = "Missing ingredients";
        else// (canCast == Spellcasting.SpellState.CanCast)
            castText.text = "Cast " + spell.displayName;

        gameObject.SetActive(true);
        m_CanvasGroup.alpha = 0;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1.25f).setEaseOutCubic().uniqueId;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f).setEaseOutCubic()
        //    .setOnComplete(() => gameObject.SetActive(false));
    }
    
    private void OnClickCast()
    {
        //m_CastButton.interactable = false;
        //onConfirmSpellcast?.Invoke(m_Spell, new List<spellIngredientsData>());
        UISpellcastingIngredients.Instance.Show(m_Spell);
        UISpellcasting.Instance.Close();
    }

    private void OnConfirmIngredients(List<spellIngredientsData> ingredients)
    {
        //m_CastButton.interactable = false;
        onConfirmSpellcast?.Invoke(m_Spell, ingredients);
        UISpellcasting.Instance.ReOpen();
    }
}
