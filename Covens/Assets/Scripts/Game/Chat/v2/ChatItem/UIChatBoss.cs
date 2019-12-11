using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChatBoss : UIChatItem
    {
        [SerializeField] private Image m_Portrait;
        [SerializeField] private TextMeshProUGUI m_BossName;
        [SerializeField] private TextMeshProUGUI m_TimeAgo;
        [SerializeField] private Text m_Text;

        public override void SetupMessage(ChatMessage message,
            UnityAction<bool> onRequestChatLoading = null,
            UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);

            string spiritId = message.data.name;
            m_Text.text = LocalizeLookUp.GetText(message.data.message);
            m_BossName.text = LocalizeLookUp.GetSpiritName(spiritId);
            m_TimeAgo.text = Utilities.EpochToDateTimeChat(message.timestamp);

            DownloadedAssets.GetSprite(spiritId + "_portrait", m_Portrait, true);
        }

        public float GetHeight(ChatMessage message)
        {
            if (message.data != null && message.data.message != null)
            {
                TextGenerator textGen = new TextGenerator();
                TextGenerationSettings generationSettings = m_Text.GetGenerationSettings(m_Text.rectTransform.rect.size);
                return textGen.GetPreferredHeight(LocalizeLookUp.GetText(message.data.message), generationSettings) + 60;
            }
            return 0;
        }
    }
}