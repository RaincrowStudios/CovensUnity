using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class ButtonAnimationView : MonoBehaviour
    {

        [SerializeField] private UnityEngine.UI.Button _button;

        [Header("Animation")]
        [SerializeField] private float _timeAnimation = 0.1f;
        [SerializeField] private float _sizeButtonAnimtion = 0.9f;

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

    }
}
