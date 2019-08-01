using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when an invite you received is canceled
    /// </summary>
    public class CovenInviteCancelHandler : IGameEventHandler
    {
        public static event System.Action<string> OnCovenInviteCancel;

        public string EventName => "coven.invite.cancel";

        private struct EventData
        {
            public string covenId;
        }

        public void HandleResponse(string eventData)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(eventData);
            var covenInvites = PlayerDataManager.playerData.covenInvites;
            for (int i = 0; i < covenInvites.Count; i++)
            {
                if (covenInvites[i].coven == data.covenId)
                {
                    PlayerDataManager.playerData.covenInvites.RemoveAt(i);
                    break;
                }
            }
            OnCovenInviteCancel?.Invoke(data.covenId);
        }
    }
}
