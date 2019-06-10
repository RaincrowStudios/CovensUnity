using System.Collections;
using System.Collections.Generic;
using Raincrow.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatImage : UIChatAvatarItem
    {
        [SerializeField] private Image m_Image;

        public override void SetupMessage(ChatMessage message,
                                          SimplePool<UIChatItem> pool,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);
            //generate the image from the bytes
        }

        public override void Despawn()
        {
            base.Despawn();
            //destroy the image
        }
    }
}
