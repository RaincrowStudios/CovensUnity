using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using System.Collections.Generic;
using Raincrow.Team;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when a player decline my coven's invite
    /// </summary>
    public class CovenInviteDeclineHandler : IGameEventHandler
    {
        public static event System.Action<string> OnPlayerInviteDecline;

        public string EventName => "coven.invite.decline";

        private struct DeclineEventData
        {
            public string characterId;
        }

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            DeclineEventData data = JsonConvert.DeserializeObject<DeclineEventData>(eventData);

            List<PendingInvite> invites = TeamManager.MyCovenData.PendingInvites;
            for (int i = 0; i < invites.Count; i++)
            {
                if (invites[i].Character == data.characterId)
                {
                    invites.RemoveAt(i);
                    break;
                }
            }

            OnPlayerInviteDecline?.Invoke(data.characterId);
        }
    }
}
