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
        private int m_AnimTweenId;
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

        private void AnimateHand()
        {
            LeanTween.cancel(m_AnimTweenId);
            m_AnimTweenId = LeanTween.value(0, 1f, 1f)
                .setOnUpdate((float t) =>
                {
                    float s = LeanTween.easeInOutBack(1, 1.2f, t);
                    float a = LeanTween.easeInOutCubic(0.5f, 1f, t);
                    transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * s, s, s);
                    m_CanvasGroup.alpha = a;
                })
                //.setEaseInOutBack()
                .setLoopPingPong()
                .uniqueId;
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
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0.5f, 0.5f)
                .setEaseOutCubic()
                .setOnComplete(AnimateHand)
                .uniqueId;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Hide(null, 1f, LeanTweenType.easeOutCubic);
        }

        private void Hide(System.Action onComplete, float time, LeanTweenType easeType)
        {
            LeanTween.cancel(m_AnimTweenId);
            LeanTween.cancel(m_TweenId);

            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, time).setEase(easeType).setOnComplete(() =>
            {
                isShowing = false;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }).uniqueId;
        }
    }
}