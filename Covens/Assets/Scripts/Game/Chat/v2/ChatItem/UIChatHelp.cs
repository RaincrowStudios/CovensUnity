using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatHelp : UIChatMessage
    {
        [SerializeField] private RectTransform _contents;
        [SerializeField] private Image image;
        [SerializeField] private Button imageButton;
        //[SerializeField] private LayoutElement layoutElement;
        //[SerializeField] private float toggleImageScale = 0.65f;
        //[SerializeField] private float preferredHeight = 1080;
        [SerializeField] private TextMeshProUGUI timestamp;

        private bool useSmallImageSize = true;
        private Sprite _generatedSprite = null;

        private float _imageHeight = 1280;

        protected override void Awake()
        {
            base.Awake();
            imageButton.onClick.AddListener(ToggleImageSize);
        }

        protected virtual void OnDisable()
        {
            imageButton.onClick.RemoveListener(ToggleImageSize);
        }

        public override void SetHeader(ChatMessage message)
        {
            _timeAgo.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }

        public override void SetIcon(ChatMessage message)
        {

        }

        public override void SetContent(ChatMessage message)
        {
            DestroyGeneratedImage();

            string messageText = message.data.message;
            if (!string.IsNullOrEmpty(messageText))
            {
                _text.text = messageText;
                _contents.sizeDelta = new Vector2(GetWidth(message), _contents.sizeDelta.y);
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

                float aspectRatio = texture.width / (float)texture.height;
                _contents.sizeDelta = new Vector2(aspectRatio * _imageHeight, _contents.sizeDelta.y);

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
            }
            else
            {
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

        private float GetWidth(ChatMessage message)
        {
            int characterCount = 0;
            if (message.data != null && message.data.message != null)
                characterCount = message.data.message.Length;
            if (characterCount >= 89)
                return 2400f;
            else
                return Mathf.Max(300f, 2400 * characterCount / 89f);
        }

        public override float GetHeight(ChatMessage message)
        {
            if (message.data.image != null && message.data.image.Length > 0)
            {
                return _imageHeight;
            }
            else
            {
                int characterCount = 0;
                if (message.data != null && message.data.message != null)
                    characterCount = message.data.message.Length;
                int lineCount = Mathf.CeilToInt(characterCount / 89f);
                return lineCount * 48 + m_Spacing + m_HeaderHeight;
            }
        }
    }
}