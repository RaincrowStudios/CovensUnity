using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatMessage : UIChatAvatarItem
    {
        [Header("Message")]
        [SerializeField] private TextMeshProUGUI m_Text;

        public override void SetupMessage(ChatMessage message,
                                          SimplePool<UIChatItem> pool,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);

            m_Text.text = message.data.message;
        }
    }

}