using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChatBoss : UIChatMessage
    {
        [SerializeField] private Image m_Portrait;
        [SerializeField] private TextMeshProUGUI m_BossName;
        [SerializeField] private TextMeshProUGUI m_TimeAgo;
        [SerializeField] private Text m_Text;

        protected override float m_HeaderHeight => 70;

        public override void OnClickIcon()
        {

        }

        public override void SetContent(ChatMessage message)
        {
            _text.text = LocalizeLookUp.GetText(message.data.message);
        }

        public override void SetIcon(ChatMessage message)
        {
            base.SetIcon(message);
            DownloadedAssets.GetSprite(message.data.name + "_portrait", m_Portrait, true);
        }

        public override void SetHeader(ChatMessage message)
        {
            _name.text = LocalizeLookUp.GetText(message.data.name);
            _timeAgo.text = Utilities.EpochToDateTimeChat(message.timestamp);
        }

        public override float GetHeight(ChatMessage message)
        {
            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings = _text.GetGenerationSettings(_text.rectTransform.rect.size);
            return textGen.GetPreferredHeight(LocalizeLookUp.GetText(message.data.message), generationSettings) + m_HeaderHeight + m_Spacing;
        }
    }
}