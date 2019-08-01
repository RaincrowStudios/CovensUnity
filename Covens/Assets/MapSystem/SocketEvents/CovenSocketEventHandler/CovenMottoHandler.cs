using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// triggered when another player changes the coven's motto
    /// </summary>
    public class CovenMottoHandler : IGameEventHandler
    {
        public static event System.Action<string> OnMottoChange;

        public string EventName => "coven.motto";

        private struct MottoEventData
        {
            public string motto;
        }

        public void HandleResponse(string eventData)
        {
            if (TeamManager.MyCovenData == null)
                return;

            MottoEventData data = JsonConvert.DeserializeObject<MottoEventData>(eventData);
            TeamManager.MyCovenData.Motto = data.motto;
            OnMottoChange?.Invoke(data.motto);
        }
    }
}
