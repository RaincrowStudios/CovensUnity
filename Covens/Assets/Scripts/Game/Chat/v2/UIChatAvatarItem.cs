using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatAvatarItem : UIChatItem
    {
        [Header("Player Avatar")]
        [SerializeField] private Sprite[] _avatars;        
        [SerializeField] private Image _playerAvatar;
       // [SerializeField] private Image _playerAlignment;
        [SerializeField] private Button _showAvatarButton;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _playerDegree;
        [SerializeField] private TextMeshProUGUI _timeAgo;

        private ChatPlayer _chatPlayer; // stores the player that is sending the location
        private long _timestamp;

        protected virtual void OnEnable()
        {
            bool enableAvatarButton = _chatPlayer != null;
            AvatarButtonSetInteractable(enableAvatarButton);
        }

        protected virtual void OnDisable()
        {
            AvatarButtonSetInteractable(false);
        }

        public override void SetupMessage(ChatMessage message,
                                          UnityAction<bool> onRequestChatLoading = null, 
                                          UnityAction onRequestChatClose = null)
        {
            base.SetupMessage(message, onRequestChatLoading, onRequestChatClose);

            _chatPlayer = message.player;
            _playerName.text = string.Concat(message.player.name, " (level ", _chatPlayer.level, ")");
            _playerDegree.text = Utilities.WitchTypeControlSmallCaps(_chatPlayer.degree);
           // _playerAlignment.color = Utilities.GetSchoolColor(_chatPlayer.degree);

            _timestamp = message.timestamp;
            _timeAgo.text = Utilities.EpochToDateTimeChat(_timestamp);
            if (_chatPlayer.avatar >= 0 && _chatPlayer.avatar < _avatars.Length)
            {
                _playerAvatar.overrideSprite = _avatars[_chatPlayer.avatar];
            }
            else
            {
                _playerAvatar.overrideSprite = null;
            }


            // if no chat player has been set, it means we did not yet setup our message
            // we also deactivate our Avatar Button
            bool enableAvatarButton = _chatPlayer != null;
            AvatarButtonSetInteractable(enableAvatarButton);
        }

        protected void AvatarButtonSetInteractable(bool interactable)
        {
            _showAvatarButton.onClick.RemoveAllListeners();
            _showAvatarButton.interactable = interactable;
            if (interactable)
            {
                _showAvatarButton.onClick.AddListener(ShowAvatar);
            }
        }

        protected void ShowAvatar()
        {
            OnRequestChatLoading?.Invoke(true);
            TeamPlayerView.ViewCharacter(_chatPlayer.id,
                     (character, error) =>
                     {
                         OnRequestChatLoading?.Invoke(false);
                     });
        }

        public void RefreshTimeAgo()
        {
            _timeAgo.text = Utilities.EpochToDateTimeChat(_timestamp);
        }
    }    
}
