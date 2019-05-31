using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using TMPro;
using UnityEngine.UI;

public class UIChatMessage : UIChatItem
{
    [SerializeField] private Sprite[] m_Avatars;

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI m_PlayerName;
    [SerializeField] private TextMeshProUGUI m_PlayerDegree;
    [SerializeField] private TextMeshProUGUI m_TimeAgo;
    [SerializeField] private TextMeshProUGUI m_Text;

    [SerializeField] private Image m_PlayerAvatar;
    [SerializeField] private Image m_PlayerAlignment;
    
    [Header("Buttons")]
    [SerializeField] private Button m_PlayerButton;

    private ChatMessage m_Message;

    private void Awake()
    {
        m_PlayerButton.onClick.AddListener(_OnClickPlayer);
    }

    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool)
    {
        base.SetupMessage(message, pool);
        m_Message = message;

        m_PlayerName.text = message.player.name + "(level" + message.player.level + ")";
        m_PlayerDegree.text = Utilities.witchTypeControlSmallCaps(message.player.degree);
        m_TimeAgo.text = Utilities.EpocToDateTimeChat(message.timestamp);
        m_Text.text = message.data.message;

        if (message.player.avatar >= 0 && message.player.avatar < m_Avatars.Length)
            m_PlayerAvatar.overrideSprite = m_Avatars[message.player.avatar];
        else
            m_PlayerAvatar.overrideSprite = null;
    }

    private void _OnClickPlayer()
    {
        ChatUI.Instance.GetPlayerDetails(m_Message.player.name);
    }
}
