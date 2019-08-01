using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone else cancels a player invitation
    /// </summary>
    public class CovenInviteCancelMembersHandler : IGameEventHandler
    {
        public static event System.Action<string> OnPlayerInviteCancel;

        public string EventName => "coven.invite.cancel.members";

        private struct EventData
        {
            public string character;
        }

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            EventData data = JsonConvert.DeserializeObject<EventData>(eventData);
            var invites = TeamManager.MyCovenData.PendingInvites;
            for (int i = 0; i < invites.Count; i++)
            {
                if (invites[i].Character == data.character)
                {
                    invites.RemoveAt(i);
                    break;
                }
            }

            OnPlayerInviteCancel?.Invoke(data.character);
        }
    }
}
