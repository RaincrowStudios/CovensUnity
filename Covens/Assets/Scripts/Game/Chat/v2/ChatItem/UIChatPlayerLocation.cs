using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChatPlayerLocation : UIChatPlayerMessage
    {
        [Header("Location")]
        [SerializeField] private Button _flyToButton;

        private double? _latitude; // stores the latitude of where the message came from
        private double? _longitude; // stores the longitude of where the message came from

        protected override void Awake()
        {
            base.Awake();
            _flyToButton.onClick.AddListener(OnClickFly);
        }

        public override void SetupProperties(ChatMessage message)
        {
            base.SetupProperties(message);

            _longitude = message.data.longitude;
            _latitude = message.data.latitude;
        }

        public override void SetContent(ChatMessage message)
        {
        }

        public void OnClickFly()
        {
            if (!_longitude.HasValue || !_latitude.HasValue)
                return;

            OnRequestChatClose?.Invoke();
            PlayerManager.Instance.FlyTo(_longitude.Value, _latitude.Value);
        }

        public override float GetHeight(ChatMessage message)
        {
            return 150 + m_Spacing;
        }
    }
}