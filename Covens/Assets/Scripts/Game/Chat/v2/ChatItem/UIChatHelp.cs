using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatHelp : UIChatItem
    {        
        [SerializeField] private Text text;
        [SerializeField] private Image image;
        [SerializeField] private Button imageButton;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private float toggleImageScale = 0.65f;
        [SerializeField] private float preferredHeight = 1080;
        [SerializeField] private TextMeshProUGUI timestamp;

        private bool useSmallImageSize = true;

        protected virtual void OnEnable()
        {
            imageButton.onClick.AddListener(ToggleImageSize);
        }

        protected virtual void OnDisable()
        {
            imageButton.onClick.RemoveListener(ToggleImageSize);
        }

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);            

            string messageText = message.data.message;
            if (!string.IsNullOrEmpty(messageText))
            {
                text.text = messageText;
                text.gameObject.SetActive(true);
            }            
            else
            {
                text.gameObject.SetActive(false);
            }

            byte[] imageBytes = message.data.image;
            if (imageBytes != null && imageBytes.Length > 0)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imageBytes);
                texture.Apply();

                Rect imageRect = new Rect(0, 0, texture.width, texture.height);

                image.overrideSprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));                

                layoutElement.preferredHeight = preferredHeight * toggleImageScale;
                float widthRatio = layoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                layoutElement.preferredWidth = preferredWidth;

                SetImageSize(true);
                image.gameObject.SetActive(true);
            }
            else
            {
                image.gameObject.SetActive(false);
            }

            timestamp.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }

        private void ToggleImageSize()
        {
            SetImageSize(!useSmallImageSize);
        }

        private void SetImageSize(bool imageBig)
        {
            useSmallImageSize = imageBig;

            if (useSmallImageSize)
            {
                Texture2D texture = image.overrideSprite.texture;
                layoutElement.preferredHeight = preferredHeight * toggleImageScale;
                float widthRatio = layoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                layoutElement.preferredWidth = preferredWidth;
            }
            else
            {
                Texture2D texture = image.overrideSprite.texture;
                layoutElement.preferredHeight = preferredHeight;
                float widthRatio = layoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                layoutElement.preferredWidth = preferredWidth;
            }
        }
    }
}