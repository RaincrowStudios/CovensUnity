using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;

public abstract class UIChatItem : MonoBehaviour
{
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

    public abstract void SetupMessage(ChatMessage message);
    
}
