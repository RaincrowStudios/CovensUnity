using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using TMPro;
using UnityEngine.UI;

public class UIChatLocation : UIChatItem
{
    [SerializeField] private Sprite[] m_Avatars;

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI m_PlayerName;
    [SerializeField] private TextMeshProUGUI m_PlayerDegree;
    [SerializeField] private TextMeshProUGUI m_TimeAgo;

    [Header("Player Portrait")]
    [SerializeField] private Image m_PlayerAvatar;
    [SerializeField] private Image m_PlayerAlignment;

    [Header("Buttons")]
    [SerializeField] private Button m_FlyToButton;

    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool)
    {
        base.SetupMessage(message, pool);

        ChatPlayer chatPlayer = message.player;
        m_PlayerName.text = string.Concat(message.player.name, " (level ", chatPlayer.level, ")");
        m_PlayerDegree.text = Utilities.witchTypeControlSmallCaps(chatPlayer.degree);
        m_PlayerAlignment.color = Utilities.GetSchoolColor(chatPlayer.degree);
        m_TimeAgo.text = Utilities.EpocToDateTimeChat(message.timestamp);
        if (chatPlayer.avatar >= 0 && chatPlayer.avatar < m_Avatars.Length)
        {
            m_PlayerAvatar.overrideSprite = m_Avatars[chatPlayer.avatar];
        }
        else
        {
            m_PlayerAvatar.overrideSprite = null;
        }
    }
}