using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChatNpc : UIChatMessage
    {
        [SerializeField] private Image m_Portrait;
        [SerializeField] private TextMeshProUGUI m_NpcName;
        [SerializeField] private TextMeshProUGUI m_TimeAgo;
        [SerializeField] private Text m_Text;
        [SerializeField] private Button m_FlyTo;

        protected override float m_HeaderHeight => 70;

        private string _message;
        private double? _longitude;
        private double? _latitude;

        protected override void Awake()
        {
            base.Awake();
            m_FlyTo.onClick.AddListener(OnClickFlyTo);
        }

        public override void SetupProperties(ChatMessage message)
        {
            base.SetupProperties(message);

            _message = GetLocalizedMessage(message);
            _longitude = message.data.longitude;
            _latitude = message.data.latitude;

            m_FlyTo.gameObject.SetActive(_longitude.HasValue && _latitude.HasValue);
        }

        public override void SetContent(ChatMessage message)
        {
            _text.text = _message;
        }

        public override void SetIcon(ChatMessage message)
        {
            DownloadedAssets.GetSprite(message.data.name + "_portrait", m_Portrait, true);
        }

        public override void SetHeader(ChatMessage message)
        {
            _name.text = LocalizeLookUp.GetText(message.data.name + "_name");
            _timeAgo.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }

        public override float GetHeight(ChatMessage message)
        {
            SetupProperties(message);
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = _text.GetGenerationSettings(_text.rectTransform.rect.size);
            return textGen.GetPreferredHeight(_message, generationSettings) * (1/transform.lossyScale.y) + m_HeaderHeight;
        }

        public static string GetLocalizedMessage(ChatMessage message)
        {
            string msg = LocalizeLookUp.GetText(message.data.message);

            if (string.IsNullOrEmpty(message.data.previousMessage) == false)
                msg = msg.Replace("{{previousMessage}}", LocalizeLookUp.GetText(message.data.previousMessage));

            if (message.data.respawnAt != 0)
            {
                var respawnTime = Utilities.FromJavaTime(message.data.respawnAt);
                var messageTime = Utilities.FromJavaTime(message.timestamp);
                var timespan = respawnTime - messageTime;
                msg = msg.Replace("{{respawnAt}}", Utilities.GetTimeRemaingString(timespan));
            }

            return msg;
        }

        public override void OnClickIcon()
        {
        }

        private void OnClickFlyTo()
        {
            if (!_longitude.HasValue || !_latitude.HasValue)
                return;

            OnRequestChatClose?.Invoke();

            PlayerManager.Instance.FlyTo(_longitude.Value, _latitude.Value, 0.0003f * 3, 0.0006f * 3);
        }
    }
}