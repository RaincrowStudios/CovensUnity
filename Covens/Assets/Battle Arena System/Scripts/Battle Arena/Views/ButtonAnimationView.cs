using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class ButtonAnimationView : MonoBehaviour
    {

        [SerializeField] private UnityEngine.UI.Button _button;

        [Header("Animation")]
        [SerializeField] private float _timeAnimation = 0.1f;
        [SerializeField] private float _sizeButtonAnimtion = 0.9f;
        [SerializeField] private Image _spellCooldownFillImage;
        [SerializeField] private GameObject _spellCooldown;
        [SerializeField] private TextMeshProUGUI _spellCooldownValue;

        private int _clickAnimationID;

        public void OnPointerDown()
        {
            if (_button.interactable)
            {
                LeanTween.cancel(_clickAnimationID);
                _clickAnimationID = LeanTween.scale(gameObject, new Vector3(_sizeButtonAnimtion, _sizeButtonAnimtion, 1), _timeAnimation).uniqueId;
            }
        }

        public void OnPointerUp()
        {
            if (_button.interactable)
            {
                LeanTween.cancel(_clickAnimationID);
                _clickAnimationID = LeanTween.scale(gameObject, Vector3.one, _timeAnimation).uniqueId;
            }
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
    }
}
