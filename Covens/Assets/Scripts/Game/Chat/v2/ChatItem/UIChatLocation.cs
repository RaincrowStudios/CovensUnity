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

    [Header("Buttons")]
    [SerializeField] private Button m_FlyToButton;

    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool)
    {
        base.SetupMessage(message, pool);

    }
}