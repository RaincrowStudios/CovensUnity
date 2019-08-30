using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.FTF
{
    public class FTFPointerHand : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Canvas;
        [SerializeField] private RectTransform m_RectTransform;
        [SerializeField] private CanvasGroup m_CanvasGroup;

        private int m_TweenId;
        private bool isShowing;
        
        private void Awake()
        {
            if (m_RectTransform == null)
                m_RectTransform = this.GetComponent<RectTransform>();
            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            gameObject.SetActive(false);
            m_CanvasGroup.alpha = 0;
        }

        public void Show(FTFPointData pointer)
        {
            if (isShowing)
            {
                Hide(() => Show(pointer), 0.2f, LeanTweenType.linear);
                return;
            }

            isShowing = true;

            m_RectTransform.anchorMin = pointer.anchorMin;
            m_RectTransform.anchorMax = pointer.anchorMax;
            m_RectTransform.anchoredPosition = pointer.position;

            if (pointer.position.x <= m_Canvas.sizeDelta.x / 2)
                m_RectTransform.localScale = new Vector3(1, 1, 1);
            else
                m_RectTransform.localScale = new Vector3(-1, 1, 1);

            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f).setEaseOutCubic().uniqueId;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Hide(null, 1f, LeanTweenType.easeOutCubic);
        }

        private void Hide(System.Action onComplete, float time, LeanTweenType easeType)
        {
            isShowing = false;

            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, time).setEase(easeType).setOnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }).uniqueId;
        }
    }
}