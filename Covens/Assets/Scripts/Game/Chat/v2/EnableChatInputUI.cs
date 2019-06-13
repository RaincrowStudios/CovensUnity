using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class EnableChatInputUI : MonoBehaviour
    {
        [SerializeField]
        private InputField _messageInputField;
        [SerializeField]
        private Button _sendMessageButton;
        [SerializeField]
        private Button _sendLocationButton;

        protected virtual void OnEnable()
        {
            _messageInputField.interactable = true;
            _sendMessageButton.interactable = true;
            _sendLocationButton.interactable = true;
        }

        protected virtual void OnDisable()
        {
            _messageInputField.interactable = false;
            _sendMessageButton.interactable = false;
            _sendLocationButton.interactable = false;
        }
    }
}
