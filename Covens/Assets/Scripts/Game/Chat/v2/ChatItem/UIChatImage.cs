using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatImage : UIChatAvatarItem
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private LayoutElement m_LayoutElement;

        public override void SetupMessage(ChatMessage message,
                                          SimplePool<UIChatItem> pool,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);
            //generate the image from the bytes

            byte[] imageBytes = message.data.image;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageBytes);
            texture.Apply();

            
            float widthRatio = m_LayoutElement.preferredHeight / texture.height;
            float preferredWidth = texture.width * widthRatio;
            Rect imageRect = new Rect(0, 0, texture.width, texture.height);

            m_Image.overrideSprite = Sprite.Create(texture, imageRect, new Vector2(0.5f, 0.5f));
            m_LayoutElement.preferredWidth = preferredWidth;
        }

        public override void Despawn()
        {
            base.Despawn();
            //destroy the image
            Object.Destroy(m_Image.overrideSprite);
        }
    }
}
