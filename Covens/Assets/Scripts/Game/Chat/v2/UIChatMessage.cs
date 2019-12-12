using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;

namespace Raincrow.Chat.UI
{
    public abstract class UIChatMessage : UIChatItem
    {
        [Header("Header")]
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected TextMeshProUGUI _timeAgo;
        [Header("Content")]
        [SerializeField] protected Text _text;
        [SerializeField] protected Image _icon;
        [SerializeField] protected Button _iconButton;

        private long _timestamp;
        private RectTransform m_RectTransform;

        protected virtual float m_HeaderHeight => 50;

        protected override void Awake()
        {
            base.Awake();
            _iconButton?.onClick.AddListener(OnClickIcon);
        }

        public override void SetupProperties(ChatMessage message)
        {
            _timestamp = message.timestamp;
        }

        public override void SetContent(ChatMessage message)
        {
            _text.text = message.data.message;
        }

        public override void SetHeader(ChatMessage message)
        {
            _name.text = message.player.name;
            _timeAgo.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }

        public override void SetIcon(ChatMessage message)
        {

        }
        
        public void RefreshTimeAgo()
        {
            _timeAgo.text = Utilities.EpochToDateTimeChat(_timestamp);
        }

        public virtual float GetHeight(ChatMessage message)
        {
            if (message.data != null && message.data.message != null)
            {
                TextGenerator textGen = new TextGenerator();
                TextGenerationSettings generationSettings = _text.GetGenerationSettings(_text.rectTransform.rect.size);
                return textGen.GetPreferredHeight(message.data.message, generationSettings)*2 + m_HeaderHeight;
            }
            return 0;
        }
    }
}