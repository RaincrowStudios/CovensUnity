using System.Collections;
using UnityEngine;

namespace Raincrow.Loading.View
{
    public class LoadingView : MonoBehaviour, ILoadingView
    {
        // Private readonlies
        private static readonly int MaxAngle = 360;
        private static readonly int DotsAnimationSeconds = 3;
        private static readonly string Dots = "...";

        // Private serialized variables
        [SerializeField] private CanvasGroup _rootCanvas;
        [SerializeField] private TMPro.TextMeshProUGUI _loadingTextMesh;
        [SerializeField] private RectTransform _iconRectTransform;   

        // Private variables
        private IEnumerator _fadeRoutine;
        private string _loadingText;
        private float _iconRotationsPerSecond = 50f;

        public IEnumerator Show(float fadeTime, float iconRotationsPerSecond)
        {
            gameObject.SetActive(true);
            _iconRotationsPerSecond = iconRotationsPerSecond;
            _fadeRoutine = FadeRoutine(fadeTime, 0f, 1f);
            yield return StartCoroutine(_fadeRoutine);
        }

        protected virtual void Update()
        {
            // Update dots
            int length = Mathf.CeilToInt(Time.time % DotsAnimationSeconds);
            _loadingTextMesh.text = string.Concat(_loadingText, Dots.Substring(0, length));

            // Rotate icon
            float rotationSpeed = MaxAngle / _iconRotationsPerSecond;
            _iconRectTransform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime); 
        }

        private IEnumerator FadeRoutine(float fadeTime, float alphaSource, float alphaDest)
        {
            for (float f = 0; f < fadeTime; f += Time.deltaTime)
            {
                _rootCanvas.alpha = Mathf.Lerp(alphaSource, alphaDest, f / fadeTime);
                yield return null;
            }
            _rootCanvas.alpha = alphaDest;
        }

        public void UpdateMessage(string message)
        {
            _loadingText = message;            
        }

        public IEnumerator Hide(float fadeTime)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = FadeRoutine(fadeTime, 1f, 0f);
            yield return StartCoroutine(_fadeRoutine);
            gameObject.SetActive(false);
        }        
    }

    public interface ILoadingView
    {
        IEnumerator Show(float fadeTime, float iconRotationsPerSecond);
        void UpdateMessage(string message);
        IEnumerator Hide(float fadeTime);
    }
}