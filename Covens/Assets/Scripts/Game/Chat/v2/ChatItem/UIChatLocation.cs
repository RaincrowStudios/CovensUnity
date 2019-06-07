using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Chat;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

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

    private double? m_Latitude; // stores the latitude of where the message came from
    private double? m_Longitude; // stores the longitude of where the message came from

    protected virtual void OnEnable()
    {
        // if no latitude or longitude has been set, it means we did not yet setup our message
        bool enableFlyButton = m_Latitude.HasValue && m_Longitude.HasValue;
        FlyButtonSetInteractable(enableFlyButton);
    }

    protected virtual void OnDisable()
    {
        FlyButtonSetInteractable(false);
    }

    public override void SetupMessage(ChatMessage message, SimplePool<UIChatItem> pool, UnityAction onRequestChatClose = null)
    {
        base.SetupMessage(message, pool, onRequestChatClose);

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

        m_Latitude = message.data.latitude;
        m_Longitude = message.data.longitude;        
        
        // Can only interact with it if both latitude and longitude have been set
        bool enableFlyButton = m_Latitude.HasValue && m_Longitude.HasValue;
        FlyButtonSetInteractable(enableFlyButton);
    }

    protected void FlyButtonSetInteractable(bool interactable)
    {
        m_FlyToButton.interactable = interactable;

        if (interactable)
        {
            m_FlyToButton.onClick.AddListener(FlyToLocation);
        }
        else
        {
            m_FlyToButton.onClick.RemoveListener(FlyToLocation);
        }
    }

    /// <summary>
    /// Flies the player to the location of the message
    /// </summary>
    protected void FlyToLocation()
    {
        OnRequestChatClose?.Invoke();

        PlayerManager.Instance.FlyTo(m_Longitude.Value, m_Latitude.Value);        
    }
}