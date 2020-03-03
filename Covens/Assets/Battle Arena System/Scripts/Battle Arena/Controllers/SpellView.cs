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
        [SerializeField] private Button _moreSpells;

        private List<SpellSlotView> _spellButtons = new List<SpellSlotView>();
        private ICharacterModel _target;

        public void Show(System.Action<string> onClickSpell)
        {
            int quickcastCount = 4;
            for (int i = _spellButtons.Count; i < quickcastCount; i++)
            {
                SpellSlotView aux = Instantiate(_buttonSpellPrefab, _spellContainer.transform);
                aux.Setup(i, onClickSpell);
                _spellButtons.Add(aux);
            }
        }


        private void OnClickMoreSpells()
        {
            if (_target == null)
                return;

            //UISpellcastBook.Open(
            //    targetData,
            //    target,
            //    PlayerDataManager.playerData.UnlockedSpells,
            //    (spell, ingredients) =>
            //    {
            //        Spellcasting.CastSpell(spell, m_Target, ingredients,
            //            (result) => this._Hide(false),
            //            () => this._Hide(false)
            //        );
            //    },
            //    () => this._Hide(false),
            //    () => this._Hide(false)
            //);
        }
    }
}
