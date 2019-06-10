using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatMessage : UIChatAvatarItem
    {
        [Header("Message")]
        [SerializeField] private TextMeshProUGUI m_Text;

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);

            m_Text.text = message.data.message;
        }
    }

}