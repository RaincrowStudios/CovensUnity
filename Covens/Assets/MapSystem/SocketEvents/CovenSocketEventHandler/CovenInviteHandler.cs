using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when you receive an invite from a coven
    /// </summary>
    public class CovenInviteHandler : IGameEventHandler
    {
        public static event System.Action<CovenInvite> OnCovenInviteReceive;

        public string EventName => "coven.invite";
        
        public void HandleResponse(string eventData)
        {
            CovenInvite data = JsonConvert.DeserializeObject<CovenInvite>(eventData);

            //just in case
            foreach(CovenInvite invite in PlayerDataManager.playerData.covenInvites)
            {
                if (invite.coven == data.coven)
                    return;
            }

            PlayerDataManager.playerData.covenInvites.Add(data);
            OnCovenInviteReceive?.Invoke(data);
        }
    }
}
