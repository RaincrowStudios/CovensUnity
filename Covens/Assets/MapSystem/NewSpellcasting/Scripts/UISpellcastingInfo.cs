using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpellcastingInfo : MonoBehaviour
{
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_IngredientsButton;

    private IMarker m_Target;
    private SpellData m_Spell;
    private SpellData m_BaseSpell;
    private List<SpellData> m_Signatures;

    private void Awake()
    {
        m_CastButton.onClick.AddListener(OnClickCast);
        m_IngredientsButton.onClick.AddListener(OnClickIngredients);
        gameObject.SetActive(false);

        UISpellcastingIngredients.Instance.onConfirmIngredients += OnConfirmIngredients;
    }

    public void Setup(IMarker target, SpellData spell, SpellData baseSpell, List<SpellData> signatures)
    {
        m_Target = target;
        m_Spell = spell;
        m_BaseSpell = baseSpell;
        m_Signatures = signatures;

        m_CastButton.GetComponent<TextMeshProUGUI>().text = "Cast " + spell.displayName;
        gameObject.SetActive(true);
    }

    private void OnClickIngredients()
    {
        UISpellcastingIngredients.Instance.Show(m_Spell);
    }

    private void OnClickCast()
    {
        Spellcasting.CastSpell(m_Spell, m_Target, new List<spellIngredientsData>(), (result, response) =>
        {
            
        });
    }

    private void OnConfirmIngredients(List<spellIngredientsData> ingredients)
    {
        Spellcasting.CastSpell(m_Spell, m_Target, ingredients, (result, response) =>
        {

        });
    }
}
