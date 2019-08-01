using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    public class CovenDisbandHandler : IGameEventHandler
    {
        public static event System.Action OnCovenDisband;

        public string EventName => "coven.disband";

        public void HandleResponse(string eventData)
        {
            TeamManager.MyCovenData = null;
            PlayerDataManager.playerData.covenInfo = new CovenInfo();

            TeamManager.OnLeaveCoven?.Invoke();
            OnCovenDisband?.Invoke();
        }
    }
}
