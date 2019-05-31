using System.Collections;
using System.Collections.Generic;
using Raincrow.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIChatImage : UIChatItem
{
    [SerializeField] private Sprite[] m_Avatars;

    [Header("Message")]
    [SerializeField] private TextMeshProUGUI m_PlayerName;
    [SerializeField] private TextMeshProUGUI m_PlayerDegree;
    [SerializeField] private TextMeshProUGUI m_TimeAgo;

    [SerializeField] private Image m_Image;

    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool)
    {
        base.SetupMessage(message, pool);
        //generate the image from the bytes
    }

    public override void Despawn()
    {
        base.Despawn();
        //destroy the image
    }
}
