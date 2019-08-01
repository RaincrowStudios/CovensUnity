using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone else invites someone to the coven
    /// </summary>
    public class CovenInviteMembersHandler : IGameEventHandler
    {
        public static event System.Action<PendingInvite> OnInviteSent;

        public string EventName => "coven.invite.members";

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            PendingInvite data = JsonConvert.DeserializeObject<PendingInvite>(eventData);

            foreach (PendingInvite invite in TeamManager.MyCovenData.PendingInvites)
            {
                if (invite.Character == data.Character)
                    return;
            }

            TeamManager.MyCovenData.PendingInvites.Add(data);
            OnInviteSent?.Invoke(data);
        }
    }
}
