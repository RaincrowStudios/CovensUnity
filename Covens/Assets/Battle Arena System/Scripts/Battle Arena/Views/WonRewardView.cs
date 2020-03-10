using UnityEngine;
using TMPro;

namespace Raincrow.BattleArena.Views
{
    public class WonRewardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private UnityEngine.UI.Image _imageIcon;
        [SerializeField] private CanvasGroup _groupPrefab;

        [Header("Animations")]
        [SerializeField] private Vector3 _toScale;
        [SerializeField] private float _timeAnimationScale = 0.5f;
        [SerializeField] private float _timeAnimationAlpha= 0.3f;
        [SerializeField] private float _timeDelay = 0.05f;

        public WonRewardView InitVariables(string name, Sprite icon)
        {
            _textName.text = name;
            _imageIcon.sprite = icon;

            return this;
        }

        public void Show()
        {
            LeanTween.alphaCanvas(_groupPrefab, 1, _timeAnimationAlpha);

            LeanTween.scale(gameObject, _toScale, _timeAnimationScale).setOnComplete(() => {
                LeanTween.scale(gameObject, Vector3.one, _timeAnimationScale).setDelay(_timeDelay);
            });
        }
    }
}