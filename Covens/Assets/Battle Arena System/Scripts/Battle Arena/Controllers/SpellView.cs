using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class SpellView : MonoBehaviour
    {
        // Serialized Fields
        [Header("UI")]
        [SerializeField] private LayoutGroup _spellContainer;
        [SerializeField] private SpellSlotView _buttonSpellPrefab;
        [SerializeField] private Button _buttonSpellAstral;

        //Privates variables
        private List<SpellSlotView> _spellButtons = new List<SpellSlotView>();
        private IBattleModel _battleModel;

        public void Show(IBattleModel battleModel, System.Action<string> onClickSpell, System.Action<string> openIngredients)
        {
            _battleModel = battleModel;

            int quickCastCount = 4;
            for (int i = _spellButtons.Count; i < quickCastCount; i++)
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
                if (!string.Equals(button.GetSpellName(), spell))
                {
                    button.SetInteractable(false);
                }
                else
                {
                    button.SetInteractable(true);
                }
            }
        }

        public void OnClickYourself()
        {
            _buttonSpellAstral.interactable = false;

            foreach (SpellSlotView button in _spellButtons)
            {
                string spellId = button.GetSpellName();
                int spellCooldownTurn = _battleModel.GetCooldown(spellId);
                SpellData spell = DownloadedAssets.GetSpell(spellId);
                bool isValidTarget = spell.target == SpellData.Target.SELF || spell.target == SpellData.Target.ANY;
                bool isInCooldown = spellCooldownTurn > 0 && spell.cooldownTurns > 0;

                button.SetInteractable(isValidTarget && !isInCooldown);
                button.SetCooldown(spellCooldownTurn, spell.cooldownTurns);
            }
        }
        public void OnClickEnemy()
        {
            _buttonSpellAstral.interactable = true;

            foreach (SpellSlotView button in _spellButtons)
            {
                string spellId = button.GetSpellName();
                SpellData spell = DownloadedAssets.GetSpell(spellId);
                int spellCooldownTurn = _battleModel.GetCooldown(spellId);
                bool isValidTarget = spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY;
                bool isInCooldown = spellCooldownTurn > 0 && spell.cooldownTurns > 0;

                button.SetInteractable(isValidTarget && !isInCooldown);
                button.SetCooldown(spellCooldownTurn, spell.cooldownTurns);
            }
        }

        public void ActivateAllButtons()
        {
            _buttonSpellAstral.interactable = true;

            foreach (SpellSlotView button in _spellButtons)
            {
                string spellId = button.GetSpellName();
                SpellData spell = DownloadedAssets.GetSpell(spellId);
                int spellCooldownTurn = _battleModel.GetCooldown(spellId);
                bool isValidTarget = spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY;
                bool isInCooldown = spellCooldownTurn > 0 && spell.cooldownTurns > 0;

                button.SetInteractable(isValidTarget && !isInCooldown);
                button.SetCooldown(spellCooldownTurn, spell.cooldownTurns);
            }
        }
    }
}
