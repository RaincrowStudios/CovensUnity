using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatMessage : UIChatAvatarItem
    {
        [Header("Message")]
        [SerializeField] private Text _text;

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);

            _text.text = message.data.message;
        }

        public override void UpdateHeigth(ChatMessage message)
        {
            return;
        }
        public float GetHeight(ChatMessage message)
        {
            if(message.data != null && message.data.message != null)
            {
                TextGenerator textGen = new TextGenerator();
                TextGenerationSettings generationSettings = _text.GetGenerationSettings(_text.rectTransform.rect.size);
                return textGen.GetPreferredHeight(message.data.message, generationSettings);
            }
            return 0;
        }
    }

}