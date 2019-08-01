using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone else kicks a coven member
    /// </summary>
    public class CovenKickHandler : IGameEventHandler
    {
        public static event System.Action<string> OnMemberKick;

        public string EventName => "coven.kick";

        private struct KickEventData
        {
            public string character;
        }

        public void HandleResponse(string eventData)
        {
            KickEventData data = JsonConvert.DeserializeObject<KickEventData>(eventData);

            //clear cached coven
            if (data.character == PlayerDataManager.playerData.name)
            {
                TeamManager.MyCovenData = null;
                PlayerDataManager.playerData.covenInfo = new CovenInfo();
                
                TeamManager.OnLeaveCoven?.Invoke();
                OnMemberKick?.Invoke(data.character);
                return;
            }

            //update cached coven
            if (TeamManager.MyCovenData == null)
                return;
            for (int i = 0; i < TeamManager.MyCovenData.Members.Count; i++)
            {
                if (TeamManager.MyCovenData.Members[i].Name == data.character)
                {
                    TeamManager.MyCovenData.Members.RemoveAt(i);
                    OnMemberKick?.Invoke(data.character);
                    break;
                }
            }
        }
    }
}
