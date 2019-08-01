using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.Team;
using System.Collections.Generic;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// triggered when another member rejects a request my coven received
    /// </summary>
    public class CovenRequestRejectMembersHandler : IGameEventHandler
    {
        public static event System.Action<string> OnRequestReject;

        public string EventName => "coven.request.reject.members";

        private struct RejectEventData
        {
            public string characterId;
        }

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            RejectEventData data = JsonConvert.DeserializeObject<RejectEventData>(eventData);
            List<PendingRequest> requests = TeamManager.MyCovenData.PendingRequests;

            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].Character == data.characterId)
                {
                    requests.RemoveAt(i);
                    break;
                }
            }

            OnRequestReject?.Invoke(data.characterId);
        }
    }
}
