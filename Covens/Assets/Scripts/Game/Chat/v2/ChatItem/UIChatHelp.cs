using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
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

}