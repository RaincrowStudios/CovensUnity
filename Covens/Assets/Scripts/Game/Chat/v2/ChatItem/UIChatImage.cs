using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace Raincrow.Chat.UI
{
    public class UIChatImage : UIChatAvatarItem
    {
        [SerializeField] private Button m_ImageButton;
        [SerializeField] private Image m_Image;
        [SerializeField] private LayoutElement m_LayoutElement;
        [SerializeField] private float m_ToggleImageScale = 0.65f;
        [SerializeField] private float m_fPreferredHeight = 1080;

        private bool m_UseSmallImageSize = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ImageButton.onClick.AddListener(ToggleImageSize);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_ImageButton.onClick.RemoveListener(ToggleImageSize);
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

            m_Image.overrideSprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));

            m_LayoutElement.preferredHeight = m_fPreferredHeight * m_ToggleImageScale;
            float widthRatio = m_LayoutElement.preferredHeight / texture.height;
            float preferredWidth = texture.width * widthRatio;
            m_LayoutElement.preferredWidth = preferredWidth;

            SetImageSize(true);
        }

        private void ToggleImageSize()
        {
            SetImageSize(!m_UseSmallImageSize);
        }

        private void SetImageSize(bool imageBig)
        {
            m_UseSmallImageSize = imageBig;

            if (m_UseSmallImageSize)
            {
                Texture2D texture = m_Image.overrideSprite.texture;
                m_LayoutElement.preferredHeight = m_fPreferredHeight * m_ToggleImageScale;
                float widthRatio = m_LayoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                m_LayoutElement.preferredWidth = preferredWidth;
            }
            else
            {
                Texture2D texture = m_Image.overrideSprite.texture;
                m_LayoutElement.preferredHeight = m_fPreferredHeight;
                float widthRatio = m_LayoutElement.preferredHeight / texture.height;
                float preferredWidth = texture.width * widthRatio;
                m_LayoutElement.preferredWidth = preferredWidth;
            }
        }
    }
}
