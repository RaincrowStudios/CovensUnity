using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class WaitScreenBattleView : MonoBehaviour
    {
        // Private readonlies
        private static readonly int DotsAnimationSeconds = 3;
        private static readonly string Dots = "...";

        // Private serialized variables
        [SerializeField] private CanvasGroup _rootCanvas;
        [SerializeField] private TMPro.TextMeshProUGUI _loadingTextMesh;

        // Private variables
        private IEnumerator _fadeRoutine;
        private string _loadingText;

        public IEnumerator Show(float fadeTime)
        {
            gameObject.SetActive(true);

            // Cleanup
            _loadingText = string.Empty;
            _loadingTextMesh.text = string.Empty;

            _fadeRoutine = FadeRoutine(fadeTime,0f, 1f);
            yield return StartCoroutine(_fadeRoutine);
        }

        protected virtual void Update()
        {
            // Update dots
            if (!string.IsNullOrEmpty(_loadingText))
            {
                int length = Mathf.CeilToInt(Time.time % DotsAnimationSeconds);
                _loadingTextMesh.text = string.Concat(_loadingText, Dots.Substring(0, length));
            }
        }

        private IEnumerator FadeRoutine(float fadeTime, float alphaSource, float alphaDest, bool active = true)
        {
            for (float f = 0; f < fadeTime; f += Time.deltaTime)
            {
                _rootCanvas.alpha = Mathf.Lerp(alphaSource, alphaDest, f / fadeTime);
                yield return null;
            }
            gameObject.SetActive(active);
            _rootCanvas.alpha = alphaDest;
        }

        public void UpdateMessage(string message)
        {
            _loadingText = message;
        }

        public IEnumerator Hide(float fadeTime)
        {
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
            }

            if (isActiveAndEnabled)
            {
                _fadeRoutine = FadeRoutine(fadeTime, 1f, 0f,false);
                yield return StartCoroutine(_fadeRoutine);

            }

            // Cleanup
            _loadingText = string.Empty;
            _loadingTextMesh.text = string.Empty;
        }
    }
}

