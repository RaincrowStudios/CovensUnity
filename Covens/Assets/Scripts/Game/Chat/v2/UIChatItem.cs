using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIChatItem : MonoBehaviour
{

    public UnityEvent OnRequestChatClose { get; private set; }
    public class RequestChatLoadingEvent : UnityEvent<bool> { }
    public UnityEvent<bool> OnRequestChatLoading { get; private set; }    
    private SimplePool<UIChatItem> m_Pool;

    public virtual void SetupMessage(ChatMessage message, 
                                     SimplePool<UIChatItem> pool, 
                                     UnityAction<bool> onRequestChatLoading = null, 
                                     UnityAction onRequestChatClose = null)
    {
        OnRequestChatClose = new UnityEvent();

        if (onRequestChatClose != null)
        {
            OnRequestChatClose.AddListener(onRequestChatClose);
        }

        OnRequestChatLoading = new RequestChatLoadingEvent();
        if (onRequestChatLoading != null)
        {
            OnRequestChatLoading.AddListener(onRequestChatLoading);
        }

        m_Pool = pool;        
    }

    public virtual void Despawn()
    {
        OnRequestChatClose.RemoveAllListeners();
        OnRequestChatLoading.RemoveAllListeners();
        m_Pool.Despawn(this);        
    }
}
