using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatImage : UIChatAvatarItem
    {
        [SerializeField] private Button _imageButton;
        [SerializeField] private Image _image;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private float _toggleImageScale = 0.65f;
        [SerializeField] private float _preferredHeight = 1080;

        private bool _useSmallImageSize = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            _imageButton.onClick.AddListener(ToggleImageSize);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _imageButton.onClick.RemoveListener(ToggleImageSize);
        }

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);
            //generate the image from the bytes

            byte[] imageBytes = message.data.image;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageBytes);
            texture.Apply();
                       
            Rect imageRect = new Rect(0, 0, texture.width, texture.height);

            _image.overrideSprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));

            _layoutElement.preferredHeight = _preferredHeight * _toggleImageScale;
            float widthRatio = _layoutElement.preferredHeight / texture.height;
            float preferredWidth = texture.width * widthRatio;
            _layoutElement.preferredWidth = preferredWidth;

            SetImageSize(true);
        }

        private void ToggleImageSize()
        {
            SetImageSize(!_useSmallImageSize);
        }

        private void SetImageSize(bool imageBig)
        {
            _useSmallImageSize = imageBig;

            if (_useSmallImageSize)
            {
                Texture2D texture = _image.overrideSprite.texture;
                _layoutElement.preferredHeight = _preferredHeight * _toggleImageScale;
                float widthRatio = _layoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                _layoutElement.preferredWidth = preferredWidth;
            }
            else
            {
                Texture2D texture = _image.overrideSprite.texture;
                _layoutElement.preferredHeight = _preferredHeight;
                float widthRatio = _layoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                _layoutElement.preferredWidth = preferredWidth;
            }
        }
    }
}
