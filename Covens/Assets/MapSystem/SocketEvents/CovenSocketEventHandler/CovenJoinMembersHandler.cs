using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when a new player joins the coven
    /// </summary>
    public class CovenJoinMembersHandler : IGameEventHandler
    {
        public static event System.Action<TeamMemberData> OnNewMember;

        public string EventName => "coven.join.members";
        
        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            TeamMemberData data = JsonConvert.DeserializeObject<TeamMemberData>(eventData);

            var members = TeamManager.MyCovenData.Members;
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].Id == data.Id)
                {
                    members[i] = data;
                    OnNewMember?.Invoke(data);
                    return;
                }
            }

            //removed from cached data
            TeamManager.MyCovenData.PendingInvites.RemoveAll(inv => inv.Character == data.Id);
            TeamManager.MyCovenData.PendingRequests.RemoveAll(req => req.Character == data.Id);

            //trigger events
            TeamManager.MyCovenData.Members.Add(data);
            OnNewMember?.Invoke(data);
        }
    }
}