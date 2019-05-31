using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;

public class UIChatHelp : UIChatItem
{
    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool)
    {
        base.SetupMessage(message, pool);
        throw new System.NotImplementedException();
    }
}
