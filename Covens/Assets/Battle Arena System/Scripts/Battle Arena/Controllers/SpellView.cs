using Raincrow.BattleArena.Model;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class SpellView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private LayoutGroup _spellContainer;
        [SerializeField] private SpellSlotView _buttonSpellPrefab;
        [SerializeField] private Button _buttonSpellAstral;

        //Privates variables
        private List<SpellSlotView> _spellButtons = new List<SpellSlotView>();
        private ICharacterModel _target;

        public void Show(System.Action<string> onClickSpell, System.Action<string> openIngredients)
        {
            int quickcastCount = 4;
            for (int i = _spellButtons.Count; i < quickcastCount; i++)
            {
                SpellSlotView aux = Instantiate(_buttonSpellPrefab, _spellContainer.transform);
                aux.Setup(i, onClickSpell, openIngredients);
                _spellButtons.Add(aux);
            }
        }

        public void OnOpenIngredients(string spell)
        {
            _buttonSpellAstral.interactable = false;

            foreach(SpellSlotView button in _spellButtons)
            {
                if(!button.GetSpellName().Equals(spell))
                    button.SetInteractable(false);
                else
                    button.SetInteractable(true);
            }
        }

        public void OnCloseIngredients()
        {
            _buttonSpellAstral.interactable = true;

            foreach (SpellSlotView button in _spellButtons)
            {
                button.SetInteractable(true);
            }
        }
    }
}
