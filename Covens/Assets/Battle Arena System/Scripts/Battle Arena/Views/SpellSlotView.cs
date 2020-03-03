using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.BattleArena.Views
{
    public class SpellSlotView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _textNameSpell;
        [SerializeField] private Image _imageIconSpell;
        [SerializeField] private Button _buttonSpell;

        [Header("Ingredients")]
        [SerializeField] private Image _imageIngredientGem;
        [SerializeField] private Image _imageIngredientTool;
        [SerializeField] private Image _imageIngredientHerb;
        [SerializeField] private Sprite _spriteFilled;
        [SerializeField] private Sprite _spriteEmpty;

        //Privates variable
        private string _spell;

        System.Action<string> _onClick;

        public void Setup(int index, System.Action<string> onClick)
        {
            StopAllCoroutines();

            _spell = PlayerManager.GetQuickcastSpell(index);

            _onClick = onClick;

            _buttonSpell.onClick.AddListener(OnClickCastSpell);

            if (string.IsNullOrEmpty(_spell) == false)
            {
                _textNameSpell.text = LocalizeLookUp.GetSpellName(_spell);
                DownloadedAssets.GetSprite(_spell, spr =>
                {
                    _imageIconSpell.sprite = spr;
                });
            }

            ResponseRequireIngredients requireds = Spellcasting.RequireIngredients(_spell);

            _imageIngredientGem.sprite = requireds.RequiredGem ? _spriteFilled : _spriteEmpty;
            _imageIngredientTool.sprite = requireds.RequiredTool ? _spriteFilled : _spriteEmpty;
            _imageIngredientHerb.sprite = requireds.RequiredHerb ? _spriteFilled : _spriteEmpty;
        }

        private void OnClickCastSpell()
        {
            _onClick(_spell);
        }
    }
}
