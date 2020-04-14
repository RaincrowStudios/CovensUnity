using Raincrow.BattleArena.Model;
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
                string spell = PlayerManager.GetQuickcastSpell(i);
                if (!string.IsNullOrWhiteSpace(spell))
                {
                    SpellSlotView aux = Instantiate(_buttonSpellPrefab, _spellContainer.transform);
                    aux.Setup(spell, onClickSpell, openIngredients);
                    _spellButtons.Add(aux);
                }
            }
        }

        public void OnOpenIngredients(string spell)
        {
            _buttonSpellAstral.interactable = false;

            foreach (SpellSlotView button in _spellButtons)
            {
                if (!button.GetSpellName().Equals(spell))
                    button.SetInteractable(false);
                else
                    button.SetInteractable(true);
            }
        }

        public void OnClickYourself()
        {
            _buttonSpellAstral.interactable = false;

            foreach (SpellSlotView button in _spellButtons)
            {
                SpellData spell = DownloadedAssets.GetSpell(button.GetSpellName());
                button.SetInteractable(spell.target == SpellData.Target.SELF || spell.target == SpellData.Target.ANY);
            }
        }
        public void OnClickEnemy()
        {
            _buttonSpellAstral.interactable = true;

            foreach (SpellSlotView button in _spellButtons)
            {
                SpellData spell = DownloadedAssets.GetSpell(button.GetSpellName());
                button.SetInteractable(spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY);
            }
        }

        public void ActiveAllButtons()
        {
            _buttonSpellAstral.interactable = true;

            foreach (SpellSlotView button in _spellButtons)
            {
                button.SetInteractable(true);
            }
        }
    }
}
