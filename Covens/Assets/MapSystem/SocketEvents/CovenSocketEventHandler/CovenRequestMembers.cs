using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// triggered when a player send a request to join the coven i am in
    /// </summary>
    public class CovenRequestMembers : IGameEventHandler
    {
        public static event System.Action<PendingRequest> OnReceiveRequest;

        public string EventName => "coven.request.members";

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            PendingRequest data = JsonConvert.DeserializeObject<PendingRequest>(eventData);
            foreach (PendingRequest request in TeamManager.MyCovenData.PendingRequests)
            {
                if (request.Character == data.Character)
                    return;
            }

            TeamManager.MyCovenData.PendingRequests.Add(data);
            OnReceiveRequest?.Invoke(data);
        }
    }
}