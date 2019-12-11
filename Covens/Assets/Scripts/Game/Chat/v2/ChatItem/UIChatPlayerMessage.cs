using UnityEngine;
using Raincrow.Chat;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChatPlayerMessage : UIChatMessage
    {
        [Header("Player")]
        [SerializeField] private Sprite[] _avatars;
        [SerializeField] private TextMeshProUGUI _degree;

        protected ChatPlayer _chatPlayer;

        protected override void Awake()
        {
            base.Awake();
            _iconButton.onClick.AddListener(OnClickIcon);
        }

        public override void SetIcon(ChatMessage message)
        {
            _chatPlayer = message.player;

            if (_chatPlayer.avatar >= 0 && _chatPlayer.avatar < _avatars.Length)
                _icon.overrideSprite = _avatars[_chatPlayer.avatar];
            else
                _icon.overrideSprite = null;
        }

        public override void SetContent(ChatMessage message)
        {
            _chatPlayer = message.player;
            base.SetContent(message);
        }

        public override void SetHeader(ChatMessage message)
        {
            _chatPlayer = message.player;
            _name.text = string.Concat(_chatPlayer, " (", LocalizeLookUp.GetText("cast_level"), " ", _chatPlayer.level, ")");
            _degree.text = Utilities.WitchTypeControlSmallCaps(_chatPlayer.degree);
            base.SetHeader(message);
        }

        public override void OnClickIcon()
        {
            if (_chatPlayer == null)
            {
                Debug.LogError("null player");
                return;
            }

            OnRequestChatLoading?.Invoke(true);
            TeamPlayerView.ViewCharacter(
                _chatPlayer.id,
                (character, error) => OnRequestChatLoading?.Invoke(false),
                false,
                () => UIChat.Close());
        }
    }
}