using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.FTF
{
    public abstract class FTFRectBase : MonoBehaviour
    {
        [Header("FTFRectBase")]
        [SerializeField] protected RectTransform m_Canvas;
        [SerializeField] protected RectTransform m_RectTransform;

        public RectTransform RectTransform { get => m_RectTransform; }
        public bool IsShowing { get; protected set; }
        
        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show(FTFRectData data)
        {
            IsShowing = true;

            //setup main rect
            m_RectTransform.anchorMin = data.anchorMin;
            m_RectTransform.anchorMax = data.anchorMax;
            m_RectTransform.anchoredPosition = data.position;
            m_RectTransform.sizeDelta = data.size;
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Hide(null, 1f, LeanTweenType.easeOutCubic);
        }

        public virtual void Hide(System.Action onComplete, float time, LeanTweenType easeType)
        {
            IsShowing = false;

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}