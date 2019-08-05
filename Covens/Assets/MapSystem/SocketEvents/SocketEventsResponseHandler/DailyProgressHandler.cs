using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    public class DailyProgressHandler : IGameEventHandler
    {
        public string EventName => "daily.progress";
        public static event System.Action<DailyProgressEventData> OnDailyProgress;

        public struct DailyProgressEventData
        {
            public string daily;
            public int count;
            public bool completed;
            public int silver;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            DailyProgressEventData data = JsonConvert.DeserializeObject<DailyProgressEventData>(eventData);

            if (data.silver != 0)
            {
                PlayerDataManager.playerData.silver += data.silver;
                PlayerManagerUI.Instance.UpdateDrachs();
            }

            switch (data.daily)
            {
                case "gather":
                    PlayerDataManager.playerData.quest.gather.count = data.count;
                    PlayerDataManager.playerData.quest.gather.completed = data.completed;
                    break;
                case "explore":
                    PlayerDataManager.playerData.quest.explore.count = data.count;
                    PlayerDataManager.playerData.quest.explore.completed = data.completed;
                    break;
                case "spellcraft":
                    PlayerDataManager.playerData.quest.spell.count = data.count;
                    PlayerDataManager.playerData.quest.spell.completed = data.completed;
                    break;
            }

            OnDailyProgress?.Invoke(data);
        }
    }
}