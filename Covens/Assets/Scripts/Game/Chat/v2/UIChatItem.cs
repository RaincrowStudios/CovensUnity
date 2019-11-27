using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Raincrow.Chat.UI
{
    public abstract class UIChatItem : EnhancedScrollerCellView
    {
        public UnityEvent OnRequestChatClose { get; private set; }
        public UnityEvent<bool> OnRequestChatLoading { get; private set; }

        public virtual void SetupMessage(ChatMessage message,
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
            cellIdentifier = message.type.ToString();
        }

        public virtual void Despawn()
        {
            OnRequestChatClose.RemoveAllListeners();
            OnRequestChatLoading.RemoveAllListeners();
        }

        public class RequestChatLoadingEvent : UnityEvent<bool> { }

        public virtual float Height => ((RectTransform)transform).sizeDelta.y;
        
    }
}