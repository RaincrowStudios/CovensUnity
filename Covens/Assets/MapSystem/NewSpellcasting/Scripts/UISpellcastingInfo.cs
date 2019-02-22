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
        LoadingOverlay.Show();
        Spellcasting.CastSpell(m_Spell, m_Target, UISpellcastingIngredients.Instance.ingredients, (result, response) =>
        {
            if (result == 200)
            {

            }
            else
            {
                if (response == "4301") //target dead
                {
                }
                else if (response == "4700") //you are dead
                {
                }
                else if (response == "4704") //target escaped
                {
                }
                else
                {
                    UIGlobalErrorPopup.ShowError(() => { }, "Unknown error [" + result + "] " + response);
                }
            }
            LoadingOverlay.Hide();
        });
    }
}
