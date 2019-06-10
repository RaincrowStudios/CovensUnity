using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatAvatarItem : UIChatItem
    {
        [Header("Player Avatar")]
        [SerializeField] private Sprite[] m_Avatars;        
        [SerializeField] private Image m_PlayerAvatar;
        [SerializeField] private Image m_PlayerAlignment;
        [SerializeField] private Button m_ShowAvatarButton;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI m_PlayerName;
        [SerializeField] private TextMeshProUGUI m_PlayerDegree;
        [SerializeField] private TextMeshProUGUI m_TimeAgo;

        private ChatPlayer m_ChatPlayer; // stores the player that is sending the location

        protected virtual void OnEnable()
        {
            bool enableAvatarButton = m_ChatPlayer != null;
            AvatarButtonSetInteractable(enableAvatarButton);
        }

        protected virtual void OnDisable()
        {
            AvatarButtonSetInteractable(false);
        }

        public override void SetupMessage(ChatMessage message,
                                          SimplePool<UIChatItem> pool,
                                          UnityAction<bool> onRequestChatLoading = null, 
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);

            m_ChatPlayer = message.player;
            m_PlayerName.text = string.Concat(message.player.name, " (level ", m_ChatPlayer.level, ")");
            m_PlayerDegree.text = Utilities.witchTypeControlSmallCaps(m_ChatPlayer.degree);
            m_PlayerAlignment.color = Utilities.GetSchoolColor(m_ChatPlayer.degree);

            m_TimeAgo.text = Utilities.EpocToDateTimeChat(message.timestamp);
            if (m_ChatPlayer.avatar >= 0 && m_ChatPlayer.avatar < m_Avatars.Length)
            {
                m_PlayerAvatar.overrideSprite = m_Avatars[m_ChatPlayer.avatar];
            }
            else
            {
                m_PlayerAvatar.overrideSprite = null;
            }


            // if no chat player has been set, it means we did not yet setup our message
            // we also deactivate our Avatar Button
            bool enableAvatarButton = m_ChatPlayer != null;
            AvatarButtonSetInteractable(enableAvatarButton);
        }

        protected void AvatarButtonSetInteractable(bool interactable)
        {
            m_ShowAvatarButton.interactable = interactable;
            if (interactable)
            {
                m_ShowAvatarButton.onClick.AddListener(ShowAvatar);
            }
            else
            {
                m_ShowAvatarButton.onClick.RemoveListener(ShowAvatar);
            }
        }

        protected void ShowAvatar()
        {
            OnRequestChatLoading?.Invoke(true);
            TeamManager.ViewCharacter(m_ChatPlayer.name,
                     (character, resultCode) =>
                     {
                         if (resultCode == 200)
                         {
                             TeamPlayerView.Instance.Setup(character);
                         }

                         OnRequestChatLoading?.Invoke(false);
                     });
        }
    }    
}
