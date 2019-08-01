using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    /// <summary>
    /// Triggered when someone else changes the title of a coven member
    /// </summary>
    public class CovenTitleHandler : IGameEventHandler
    {
        public static event System.Action<string> OnTitleChange;

        public string EventName => "coven.title";

        private struct TitleEventData
        {
            public string character;
            public string target;
            public string title;
        }

        public void HandleResponse(string eventData)
        {
            TitleEventData data = JsonConvert.DeserializeObject<TitleEventData>(eventData);

            if (data.target == PlayerDataManager.playerData.name)
                PlayerDataManager.playerData.covenInfo.title = data.title;

            if (TeamManager.MyCovenData == null)
                return;

            foreach (var member in TeamManager.MyCovenData.Members)
            {
                if (member.Name == data.target)
                {
                    member.Title = data.title;
                    OnTitleChange?.Invoke(member.Id);
                    break;
                }
            }
        }
    }
}