using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatHelp : UIChatItem
    {
        [SerializeField] private Text text;
        [SerializeField] private TextMeshProUGUI timestamp;

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);
            text.text = message.data.message;
            timestamp.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }
    }

}