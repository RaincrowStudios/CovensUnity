using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;

namespace Raincrow.Chat.UI
{
    public abstract class UIChatItem : EnhancedScrollerCellView
    {

        public UnityEvent OnRequestChatClose { get; private set; }
        public UnityEvent<bool> OnRequestChatLoading { get; private set; }

        protected virtual void Awake() { }

        public void SetupMessage(ChatMessage message, UnityAction<bool> onRequestChatLoading = null, UnityAction onRequestChatClose = null)
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

            SetContent(message);
            SetIcon(message);
            SetHeader(message);
        }

        public virtual void Despawn()
        {
            OnRequestChatClose.RemoveAllListeners();
            OnRequestChatLoading.RemoveAllListeners();
        }

        public class RequestChatLoadingEvent : UnityEvent<bool> { }
        public abstract void SetIcon(ChatMessage message);
        public abstract void SetContent(ChatMessage message);
        public abstract void SetHeader(ChatMessage message);
        //public abstract float GetHeight(ChatMessage message);
        public abstract void OnClickIcon();
    }
}