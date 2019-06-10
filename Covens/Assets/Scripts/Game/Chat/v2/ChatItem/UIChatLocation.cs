using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatLocation : UIChatAvatarItem
    {
        [Header("Buttons")]
        [SerializeField] private Button m_FlyToButton;

        private double? m_Latitude; // stores the latitude of where the message came from
        private double? m_Longitude; // stores the longitude of where the message came from

        protected override void OnEnable()
        {
            base.OnEnable();

            // if no latitude or longitude has been set, it means we did not yet setup our message
            // we also deactivate our Fly Button
            bool enableFlyButton = m_Latitude.HasValue && m_Longitude.HasValue;
            FlyButtonSetInteractable(enableFlyButton);
        }

        protected override void OnDisable()
        {
            FlyButtonSetInteractable(false);
        }

        public override void SetupMessage(ChatMessage message,
                                          SimplePool<UIChatItem> pool,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, pool, onRequestChatLoading, onRequestChatClose);
            
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
}