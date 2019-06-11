using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatMessage : UIChatAvatarItem
    {
        [Header("Message")]
        [SerializeField] private TextMeshProUGUI _text;

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);

            _text.text = message.data.message;
        }
    }

}