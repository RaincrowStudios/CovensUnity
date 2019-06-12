using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Raincrow.Chat.UI
{
    public class UIChatCoven : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _covenTitle;
        [SerializeField] private TextMeshProUGUI _numMembers;
        [SerializeField] private TextMeshProUGUI _founderName;
        [SerializeField] private TextMeshProUGUI _covenLevel;
        [SerializeField] private TextMeshProUGUI _covenXP;
        [SerializeField] private TextMeshProUGUI _worldRank;
        [SerializeField] private TextMeshProUGUI _sendRequestLabel;
        [SerializeField] private Button _sendRequestButton;
        [SerializeField] private Image _covenLogo;

        private string _covenName;

        protected virtual void OnEnable()
        {
            _sendRequestButton.onClick.AddListener(SendJoinCovenRequest);
        }

        protected virtual void OnDisable()
        {
            _sendRequestButton.onClick.RemoveListener(SendJoinCovenRequest);
        }

        public virtual void SetupCoven(ChatCovenData coven, UnityAction<bool> onRequestChatLoading = null, UnityAction onRequestChatClose = null)
        {
            // covens color
            _covenLogo.color = Utilities.GetSchoolColor(coven.alignment);

            // covens title text
            _covenName = coven.name;
            _covenTitle.text = string.Concat(_covenName, " ", Utilities.GetSchoolCoven(coven.alignment));

            // World Rank text
            _worldRank.text = string.Concat(LocalizeLookUp.GetText("lt_world_rank"), "<b><color=white>", coven.worldRank.ToString());

            // Experience text
            string experience = "<b><color=white>" + coven.xp.ToString() + "</b></color>";
            _covenXP.text = LocalizeLookUp.GetText("spell_xp").Replace("{{Number}}", experience);

            // Coven Level text
            _covenLevel.text = string.Concat(LocalizeLookUp.GetText("card_witch_level"), ":<b><color=white>", coven.level.ToString());

            // members text
            string members = string.Concat("<b><color=white>", coven.members.ToString());
            _numMembers.text = LocalizeLookUp.GetText("invite_member").Replace("{{member}}", members);

            // founder text
            string founder = coven.founder.ToString();
            _founderName.text = string.Concat(LocalizeLookUp.GetText("coven_founder"), "<b><color=white>", founder);

            // Send Request text
            _sendRequestLabel.text = LocalizeLookUp.GetText("invite_send_request");            
        }

        private void SendJoinCovenRequest()
        {
            TeamManager.RequestInvite((int response) =>
            {
                if (response == 200)
                {
                    _sendRequestLabel.text = LocalizeLookUp.GetText("coven_request_success"); // "Sent";
                }
                else if (response == 4805)
                {
                    _sendRequestLabel.text = LocalizeLookUp.GetText("coven_already_requested"); // "already requested invite";
                }
                else
                {
                    _sendRequestLabel.text = LocalizeLookUp.GetText("lt_failed"); // "Failed";
                }
            }, _covenName);
        }
    }
}