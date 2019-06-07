using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;

public abstract class UIChatItem : MonoBehaviour
{
    public UnityEvent OnRequestChatClose { get; private set; }

    private RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
                m_RectTransform = this.GetComponent<RectTransform>();
            return m_RectTransform;
        }
    }

    private SimplePool<UIChatItem> m_Pool;

    public virtual void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool, UnityAction onRequestChatClose = null)
    {
        OnRequestChatClose = new UnityEvent();

        if (onRequestChatClose != null)
        {
            OnRequestChatClose.AddListener(onRequestChatClose);
        }
        m_Pool = pool;
    }

    public virtual void Despawn()
    {
        OnRequestChatClose.RemoveAllListeners();
        m_Pool.Despawn(this);        
    }
}
