using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class PopupView : MonoBehaviour, IPopupView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        public bool IsVisible => isActiveAndEnabled;

        public IEnumerator Hide()
        {
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            // Fade out
            yield return FadeCanvas(1f, 0f);

            gameObject.SetActive(false);
        }

        private IEnumerator FadeCanvas(float start, float end)
        {
            for (float elapsedTime = 0, fadeTime = 0.25f; elapsedTime < fadeTime; elapsedTime += Time.deltaTime)
            {
                _canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / fadeTime);
                yield return null;
            }
            _canvasGroup.alpha = end;
        }

        public IEnumerator Show(string message, UnityAction confirmAction, UnityAction cancelAction = null)
        {
            gameObject.SetActive(true);

            // set message
            _message.text = message;

            // set confirm action
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(confirmAction);
            _confirmButton.gameObject.SetActive(true);

            // set cancel action
            _cancelButton.onClick.RemoveAllListeners();
            if (cancelAction != null)
            {
                _cancelButton.onClick.AddListener(cancelAction);
                _cancelButton.gameObject.SetActive(true);
            }
            else
            {
                _cancelButton.gameObject.SetActive(false);
            }

            // Fade in
            yield return FadeCanvas(0f, 1f);
        }
    }

    public interface IPopupView
    {
        IEnumerator Show(string message, UnityAction confirmAction, UnityAction cancelAction = null);
        IEnumerator Hide();

        bool IsVisible { get; }
    }
}