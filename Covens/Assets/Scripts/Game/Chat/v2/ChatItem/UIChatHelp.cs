using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatHelp : UIChatMessage
    {        
        [SerializeField] private Image image;
        [SerializeField] private Button imageButton;
        //[SerializeField] private LayoutElement layoutElement;
        //[SerializeField] private float toggleImageScale = 0.65f;
        //[SerializeField] private float preferredHeight = 1080;
        [SerializeField] private TextMeshProUGUI timestamp;

        private bool useSmallImageSize = true;
        private Sprite _generatedSprite = null;

        protected override void Awake()
        {
            base.Awake();
            imageButton.onClick.AddListener(ToggleImageSize);
        }

        protected virtual void OnDisable()
        {
            imageButton.onClick.RemoveListener(ToggleImageSize);
        }

        public override void SetContent(ChatMessage message)
        {
            DestroyGeneratedImage();

            string messageText = message.data.message;
            if (!string.IsNullOrEmpty(messageText))
            {
                _text.text = messageText;
                _text.gameObject.SetActive(true);
            }            
            else
            {
                _text.gameObject.SetActive(false);
            }

            byte[] imageBytes = message.data.image;
            if (imageBytes != null && imageBytes.Length > 0)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imageBytes);
                texture.Apply();

                Rect imageRect = new Rect(0, 0, texture.width, texture.height);

                _generatedSprite = image.overrideSprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));                

                //layoutElement.preferredHeight = preferredHeight * toggleImageScale;
                //float widthRatio = layoutElement.preferredHeight / texture.height;
                //float preferredWidth = texture.width * widthRatio;
                //layoutElement.preferredWidth = preferredWidth;

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
                //Texture2D texture = image.overrideSprite.texture;
                //layoutElement.preferredHeight = preferredHeight * toggleImageScale;
                //float widthRatio = layoutElement.preferredHeight / texture.height;
                //float preferredWidth = texture.width * widthRatio;
                //layoutElement.preferredWidth = preferredWidth;
            }
            else
            {
                //Texture2D texture = image.overrideSprite.texture;
                //layoutElement.preferredHeight = preferredHeight;
                //float widthRatio = layoutElement.preferredHeight / texture.height;
                //float preferredWidth = texture.width * widthRatio;
                //layoutElement.preferredWidth = preferredWidth;
            }
        }

        public override void OnClickIcon()
        {

        }

        private void DestroyGeneratedImage()
        {
            if (_generatedSprite == null)
                return;

            Destroy(_generatedSprite.texture);
            Destroy(_generatedSprite);
        }

        public override float GetHeight(ChatMessage message)
        {
            float height = 0;
            if (message.data.image?.Length > 0)
                height += 850;

            if (!string.IsNullOrWhiteSpace(message.data.message))
                return height + base.GetHeight(message);
            else
                return height + m_Spacing;
        }
    }
}