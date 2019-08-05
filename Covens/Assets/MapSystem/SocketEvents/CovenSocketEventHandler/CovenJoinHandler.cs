using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone accepts your request to join a coven
    /// </summary>
    public class CovenJoinHandler : IGameEventHandler
    {
        public static event System.Action<string> OnCovenJoin;

        public string EventName => "coven.join";

        public void HandleResponse(string eventData)
        {
            CovenInfo coven = JsonConvert.DeserializeObject<CovenInfo>(eventData);
            PlayerDataManager.playerData.covenInfo = coven;
            TeamManager.MyCovenData = null;

            //remove from invites invites
            PlayerDataManager.playerData.covenInvites.RemoveAll((invite) => invite.coven == coven.coven);

            //remove from requests
            PlayerDataManager.playerData.covenRequests.RemoveAll((req) => req.coven == coven.coven);

            //trigger events
            TeamManager.OnJoinCoven?.Invoke(coven.coven, coven.name);
            OnCovenJoin?.Invoke(coven.name);
        }
    }
}
