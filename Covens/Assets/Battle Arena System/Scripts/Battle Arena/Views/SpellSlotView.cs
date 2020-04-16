using System.Collections;
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
        [SerializeField] private Image _spellCooldownFillImage;
        [SerializeField] private GameObject _spellCooldown;
        [SerializeField] private TextMeshProUGUI _spellCooldownValue;

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

        public void Setup(string spell, int maxCooldown, System.Action<string> onClickSpell, System.Action<string> openIngredients)
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
            ResponseRequireIngredients required = Spellcasting.RequireIngredients(_spell);
            _imageIngredientGem.sprite = required.RequiredGem ? _spriteFilled : _spriteEmpty;
            _imageIngredientTool.sprite = required.RequiredTool ? _spriteFilled : _spriteEmpty;
            _imageIngredientHerb.sprite = required.RequiredHerb ? _spriteFilled : _spriteEmpty;

            SetCooldown(0, maxCooldown);
        }

        public void SetInteractable(bool value)
        {
            _buttonSpell.interactable = value;
        }

        public void SetCooldown(int cooldown, int maxCooldown)
        {
            if (cooldown > 0)
            {
                _spellCooldownFillImage.gameObject.SetActive(true);
                float normalizedCooldown = cooldown / (float)maxCooldown;
                _spellCooldownFillImage.fillAmount = normalizedCooldown;

            }
            else
            {
                _spellCooldownFillImage.gameObject.SetActive(false);
                _spellCooldownFillImage.fillAmount = 0f;
            }

            _spellCooldown.SetActive(maxCooldown > 0);
            _spellCooldownValue.text = maxCooldown.ToString();
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
