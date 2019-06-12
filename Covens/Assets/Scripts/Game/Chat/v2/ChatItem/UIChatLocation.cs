using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class UIChatLocation : UIChatAvatarItem
    {
        [Header("Buttons")]
        [SerializeField] private Button _flyToButton;

        private double? _latitude; // stores the latitude of where the message came from
        private double? _longitude; // stores the longitude of where the message came from

        protected override void OnEnable()
        {
            base.OnEnable();

            // if no latitude or longitude has been set, it means we did not yet setup our message
            // we also deactivate our Fly Button
            bool enableFlyButton = _latitude.HasValue && _longitude.HasValue;
            FlyButtonSetInteractable(enableFlyButton);
        }

        protected override void OnDisable()
        {
            FlyButtonSetInteractable(false);
        }

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null,
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);
            
            _latitude = message.data.latitude;
            _longitude = message.data.longitude;

            // Can only interact with it if both latitude and longitude have been set
            bool enableFlyButton = _latitude.HasValue && _longitude.HasValue;
            FlyButtonSetInteractable(enableFlyButton);
        }

        protected void FlyButtonSetInteractable(bool interactable)
        {
            _flyToButton.interactable = interactable;
            if (interactable)
            {
                _flyToButton.onClick.AddListener(FlyToLocation);
            }
            else
            {
                _flyToButton.onClick.RemoveListener(FlyToLocation);
            }
        }

        /// <summary>
        /// Flies the player to the location of the message
        /// </summary>
        protected void FlyToLocation()
        {
            OnRequestChatClose?.Invoke();

            PlayerManager.Instance.FlyTo(_longitude.Value, _latitude.Value);
        }
    }
}