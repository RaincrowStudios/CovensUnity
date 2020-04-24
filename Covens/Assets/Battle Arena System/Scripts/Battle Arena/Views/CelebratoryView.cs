using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.BattleArena.Views
{
    public class CelebratoryView : MonoBehaviour, ICelebratoryView
    {
        [Header("UI")]
        [SerializeField] private Image _backgroundaAlpha;
        [SerializeField] private Image _backgroundaSideBar;
        [SerializeField] private CanvasGroup _celebratoryUI;

        [Header("Result UI")]
        [SerializeField] private TextMeshProUGUI _textResult;
        [SerializeField] private Image[] _sideGlows;

        [Header("Animation")]
        [SerializeField] private float _finalPositionAnimationY = 300f;
        [SerializeField] private float _startPositionAnimationY = 230f;
        [SerializeField] private float _animationTime = 0.3f;

        public void Show(bool victory)
        {
            _textResult.text = victory ? LocalizeLookUp.GetText("battle_victory") : LocalizeLookUp.GetText("battle_defeat");
            _textResult.color = victory ? Color.white : Color.red;
            _backgroundaSideBar.color = victory ? Color.white : Utilities.Red;

            foreach (Image sg in _sideGlows)
            {
                sg.color = victory ? Color.white : Color.red;
            }

            gameObject.SetActive(true);

            AnimationIn();
        }

        public void Hide()
        {
            AnimationOut();
        }

        private void AnimationIn()
        {
            if (_backgroundaAlpha == null)
            {
                return;
            }

            LeanTween.value(0f, 0.1f, _animationTime).setOnUpdate((value) =>
            {
                if (_backgroundaAlpha != null)
                {
                    _backgroundaAlpha.color = new Vector4(0, 0, 0, value);
                }
            });

            LeanTween.alphaCanvas(_celebratoryUI, 1f, _animationTime);

            LeanTween.moveLocalY(_celebratoryUI.gameObject, _finalPositionAnimationY, _animationTime);
        }

        private void AnimationOut()
        {

            LeanTween.value(0.1f, 0f, _animationTime).setOnUpdate((value) =>
            {
                if (_backgroundaAlpha != null)
                {
                    _backgroundaAlpha.color = new Vector4(0, 0, 0, value);
                }
            });

            LeanTween.alphaCanvas(_celebratoryUI, 0f, _animationTime);

            LeanTween.moveLocalY(_celebratoryUI.gameObject, _startPositionAnimationY, _animationTime);
        }
    }

    public interface ICelebratoryView
    {
        void Show(bool victory);
        void Hide();
    }
}