using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;

public class UIChatHelp : UIChatItem
{
    public override void SetupMessage(ChatMessage message, 
                                      SimplePool<UIChatItem> pool, 
                                      UnityAction<bool> onRequestChatLoading = null, 
                                      UnityAction onRequestChatClose = null)
    {
        base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);
    }
}
