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
        [SerializeField] private ButtonAnimationView _astralCloakAnimView;
        [SerializeField] private Button _buttonSpellAstral;

        // Privates variables
        private List<SpellSlotView> _spellButtons = new List<SpellSlotView>();
        private IBattleModel _battleModel;

        // Static readonlies
        private static readonly string SpellAstralCloak = "spell_astral";

        public void Show(IBattleModel battleModel, System.Action<string> onClickSpell, System.Action<string> openIngredients)
        {
            _battleModel = battleModel;

            // Astral Cloak Spell Data
            SpellData astralCloakSpellData = DownloadedAssets.GetSpell(SpellAstralCloak);
            int spellCooldownTurn = battleModel.GetCooldown(SpellAstralCloak);
            _astralCloakAnimView.SetCooldown(spellCooldownTurn, astralCloakSpellData.cooldownTurns);

            int quickCastCount = 4;
            for (int i = _spellButtons.Count; i < quickCastCount; i++)
            {
                string spell = PlayerManager.GetQuickcastSpell(i);
                if (!string.IsNullOrWhiteSpace(spell))
                {
                    SpellSlotView aux = Instantiate(_buttonSpellPrefab, _spellContainer.transform);

                    //aux.Setup(spell, maxCooldown, onClickSpell, openIngredients);
                    aux.Setup(spell, onClickSpell, openIngredients);
                    _spellButtons.Add(aux);

                    SpellData data = DownloadedAssets.GetSpell(spell);
                    int cooldown = battleModel.GetCooldown(spell);
                    int maxCooldown = data.cooldownTurns;
                    ButtonAnimationView buttonAnimationView = aux.GetComponent<ButtonAnimationView>();
                    buttonAnimationView.SetCooldown(cooldown, maxCooldown);

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

        public void OnClickYourself(bool canUseAstral)
        {
            _buttonSpellAstral.interactable = canUseAstral;

            foreach (SpellSlotView button in _spellButtons)
            {
                string spellId = button.GetSpellName();
                int spellCooldownTurn = _battleModel.GetCooldown(spellId);
                SpellData spell = DownloadedAssets.GetSpell(spellId);
                bool isValidTarget = spell.target == SpellData.Target.SELF || spell.target == SpellData.Target.ANY;
                bool isInCooldown = spellCooldownTurn > 0 && spell.cooldownTurns > 0;

                button.SetInteractable(isValidTarget && !isInCooldown);

                SpellData data = DownloadedAssets.GetSpell(spellId);
                int cooldown = _battleModel.GetCooldown(spellId);
                int maxCooldown = data.cooldownTurns;
                ButtonAnimationView buttonAnimationView = button.GetComponent<ButtonAnimationView>();
                buttonAnimationView.SetCooldown(cooldown, maxCooldown);
            }
        }
        public void OnClickEnemy(string objectType)
        {
            _buttonSpellAstral.interactable = false;

            foreach (SpellSlotView button in _spellButtons)
            {
                string spellId = button.GetSpellName();
                SpellData spell = DownloadedAssets.GetSpell(spellId);
                int spellCooldownTurn = _battleModel.GetCooldown(spellId);

                bool isValidTarget = false;
                bool isInCooldown = spellCooldownTurn > 0 && spell.cooldownTurns > 0;

                if (string.Equals(objectType, ObjectType.Spirit))
                {
                    isValidTarget = spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY;
                }
                else
                {
                    if (string.Equals(BattleType.PvE, _battleModel.BattleType))
                    {
                        isValidTarget = spell.beneficial && (spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY);
                    }
                    else
                    {
                        isValidTarget = spell.target == SpellData.Target.OTHER || spell.target == SpellData.Target.ANY;
                    }
                }

                button.SetInteractable(isValidTarget && !isInCooldown);

                SpellData data = DownloadedAssets.GetSpell(spellId);
                int cooldown = _battleModel.GetCooldown(spellId);
                int maxCooldown = data.cooldownTurns;
                ButtonAnimationView buttonAnimationView = button.GetComponent<ButtonAnimationView>();
                buttonAnimationView.SetCooldown(cooldown, maxCooldown);
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

                SpellData data = DownloadedAssets.GetSpell(spellId);
                int cooldown = _battleModel.GetCooldown(spellId);
                int maxCooldown = data.cooldownTurns;
                ButtonAnimationView buttonAnimationView = button.GetComponent<ButtonAnimationView>();
                buttonAnimationView.SetCooldown(cooldown, maxCooldown);
            }
        }
    }
}
