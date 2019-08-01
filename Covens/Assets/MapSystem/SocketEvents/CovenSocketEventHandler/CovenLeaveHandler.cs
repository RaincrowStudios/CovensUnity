using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;
using System.Collections.Generic;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// triggered when someone else leaves the coven I am in
    /// </summary>
    public class CovenLeaveHandler : IGameEventHandler
    {
        public static event System.Action<string> OnMemberLeave;

        public string EventName => "coven.leave";

        private struct LeaveEventData
        {
            public string character;
        }

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            LeaveEventData data = JsonConvert.DeserializeObject<LeaveEventData>(eventData);
            List<TeamMemberData> members = TeamManager.MyCovenData.Members;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].Name == data.character)
                {
                    members.RemoveAt(i);
                    break;
                }
            }

            OnMemberLeave?.Invoke(data.character);
        }
    }
}
