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
        
        protected virtual void Awake()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show(Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            //setup main rect
            m_RectTransform.anchorMin = anchorMin;
            m_RectTransform.anchorMax = anchorMax;
            m_RectTransform.anchoredPosition = position;
            m_RectTransform.sizeDelta = size;
            
            gameObject.SetActive(true);
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            Hide(null, 1f, LeanTweenType.easeOutCubic);
        }

        public virtual void Hide(System.Action onComplete, float time, LeanTweenType easeType)
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}