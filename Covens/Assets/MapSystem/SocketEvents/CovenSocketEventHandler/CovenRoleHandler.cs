using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone else changes the role of a member
    /// </summary>
    public class CovenRoleHandler : IGameEventHandler
    {
        public static event System.Action<string> OnRoleChange;

        public string EventName => "coven.role";

        private struct RoleEventData
        {
            public string character;
            public string target;
            public int role;
        }

        public void HandleResponse(string eventData)
        {
            RoleEventData data = JsonConvert.DeserializeObject<RoleEventData>(eventData);
            
            //udpate coven data if cached
            if (TeamManager.MyCovenData == null)
                return;

            foreach (var member in TeamManager.MyCovenData.Members)
            {
                if (member.Name == data.target)
                {
                    member.Role = (CovenRole)data.role;
                    OnRoleChange?.Invoke(member.Id);
                    break;
                }
            }
        }
    }
}