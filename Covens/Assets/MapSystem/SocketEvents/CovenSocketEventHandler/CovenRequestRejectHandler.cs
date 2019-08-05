using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// triggered when a coven rejects my request to join it
    /// </summary>
    public class CovenRequestRejectHandler : IGameEventHandler
    {
        public static event System.Action<string> OnRequestReject;

        public string EventName => "coven.request.reject";

        private struct RejectEventData
        {
            public string covenId;
        }

        public void HandleResponse(string eventData)
        {
            RejectEventData data = JsonConvert.DeserializeObject<RejectEventData>(eventData);
            List<CovenRequest> requests = PlayerDataManager.playerData.covenRequests;

            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].coven == data.covenId)
                {
                    requests.RemoveAt(i);
                    break;
                }
            }

            OnRequestReject?.Invoke(data.covenId);
        }
    }
}
