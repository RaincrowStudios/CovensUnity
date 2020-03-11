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

        private System.Action<string> _onClickSpell;
        private System.Action<string> _openIngredients;
        private float _startTimeOnClick = 0.0f;
        private bool _pointerDown;
        private int _clickAnimationID;
        //Const variable
        private const float TIME_HOLD = 0.5f;

        public void Setup(string spell, System.Action<string> onClickSpell, System.Action<string> openIngredients)
        {
            StopAllCoroutines();

            _spell = spell;

            _onClickSpell = onClickSpell;
            _openIngredients = openIngredients;

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

        public void SetInteractable(bool value)
        {
            _buttonSpell.interactable = value;
        }

        public void OnPointerDownSpell()
        {
            if (_buttonSpell.interactable)
            {
                _pointerDown = true;
                StartCoroutine(WaitPointerUpCoroutine());
            }
        }

        public void OnPointerUpSpell()
        {
            if (_buttonSpell.interactable)
            {
                _pointerDown = false;
            }
        }

        private IEnumerator WaitPointerUpCoroutine()
        {
            float time = 0;
            while (_pointerDown)
            {
                yield return 0;
                time += Time.unscaledDeltaTime;

                if (time > TIME_HOLD)
                {
                    _openIngredients(_spell);
                    yield break;
                }
            }

            _onClickSpell(_spell);
        }

        public string GetSpellName()
        {
            return _spell;
        }
    }
}
