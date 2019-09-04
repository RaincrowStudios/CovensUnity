using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.FTF
{
    public class FTFHighlight : FTFRectBase
    {
        [Header("FTFHighlight")]
        [SerializeField] protected CanvasGroup m_CanvasGroup;
        [SerializeField] private RectTransform m_Left;
        [SerializeField] private RectTransform m_Right;
        [SerializeField] private RectTransform m_Top;
        [SerializeField] private RectTransform m_Bot;

        private int m_TweenId;

        protected override void Awake()
        {
            base.Awake();
            m_CanvasGroup.alpha = 0;
        }

        public override void Show(FTFRectData data)
        {
            if (Mathf.Approximately(m_CanvasGroup.alpha, 0) == false)
            {
                //hide first
                Hide(() => Show(data), 0.2f, LeanTweenType.linear);
                return;
            }

            base.Show(data);
            gameObject.SetActive(false);

            Vector2 canvas = m_Canvas.sizeDelta;
            Vector2 position = data.position;
            Vector2 size = data.size;

            //animate
            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f)
                .setEaseOutCubic()
                .uniqueId;
            
            //fit the screen
            m_Top.sizeDelta = new Vector2(
                m_Top.sizeDelta.x,
                canvas.x/* / 2 - position.y - m_RectTransform.pivot.y / 2 - size.y / 2*/);

            m_Bot.sizeDelta = new Vector2(
                m_Bot.sizeDelta.x,
                canvas.x/* - m_Top.sizeDelta.y - size.y*/);


            m_Right.sizeDelta = new Vector2(
                canvas.x /* / 2f - position.x - m_RectTransform.pivot.x / 2 - size.x / 2*/,
                canvas.x/*- size.y*/);

            m_Left.sizeDelta = new Vector2(
                canvas.x/* - m_Right.sizeDelta.x - size.x*/,
                canvas.x/* - size.y*/);

            //if (position.y > 0)
            //    m_Right.anchoredPosition = m_Left.anchoredPosition = new Vector2(0, -m_Bot.sizeDelta.y / 2);
            //else
            //    m_Right.anchoredPosition = m_Left.anchoredPosition = new Vector2(0, m_Top.sizeDelta.y / 2);

            gameObject.SetActive(true);
        }

        public override void Hide(Action onComplete, float time, LeanTweenType easeType)
        {
            //base.Hide(onComplete, time, easeType); 
            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, time)
                .setOnComplete(() =>
                {
                    IsShowing = false;
                    gameObject.SetActive(false);
                    m_CanvasGroup.alpha = 0;
                    onComplete?.Invoke();
                })
                .setEase(easeType)
                .uniqueId;
        }
    }
}