using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;

namespace Raincrow.GameEventResponses
{
    public class DailyProgressHandler : IGameEventHandler
    {
        public string EventName => "daily.progress";

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
            HandleResponse(data);
        }

        public static void HandleResponse(DailyProgressEventData data)
        {
            string message = null;
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

            if (data.silver != 0)
            {
                PlayerDataManager.playerData.silver += data.silver;
                PlayerManagerUI.Instance.UpdateDrachs();
            }

            ShowNotification(data.daily, data.silver, data.count);
        }

        public static void ShowNotification(string quest, int silver, int count)
        {
            string message = null;

            if (silver == 0)
            {
                if (quest == "gather")
                {
                    message =
                        LocalizeLookUp.GetText("daily_quest_progress") + " " +
                        LocalizeLookUp.GetText("daily_gather") + "\n" +
                        LocalizeLookUp.GetText("daily_completed") + " " + count.ToString() + "/" + QuestsController.Quests.gather.amount.ToString();
                }
                else if (quest == "spellcraft")
                {
                    message =
                        LocalizeLookUp.GetText("daily_quest_progress") + " " +
                        LocalizeLookUp.GetText("daily_spell") + "\n" +
                        LocalizeLookUp.GetText("daily_completed") + " " + count.ToString() + "/" + QuestsController.Quests.spellcraft.amount.ToString();
                }
            }
            else
            {
                if (quest == "gather")
                {
                    message = LocalizeLookUp.GetText("daily_completed_gather") + "\n" +
                        "+ " + silver.ToString() + " " + LocalizeLookUp.GetText("store_silver");
                }
                else if (quest == "spellcraft")
                {
                    message = LocalizeLookUp.GetText("daily_completed_spellcraft") + "\n" +
                        "+ " + silver.ToString() + " " + LocalizeLookUp.GetText("store_silver");
                }
                else
                {
                    message = LocalizeLookUp.GetText("daily_completed_explore") + "\n" +
                        "+ " + silver.ToString() + " " + LocalizeLookUp.GetText("store_silver");
                }
            }

            if (message != null)
            {
                LeanTween.value(0, 0, 1f).setOnComplete(() => PlayerNotificationManager.Instance.ShowNotification(message, null));
            }
            else
            {
                Debug.LogError("null quest progress text");
            }
        }
    }
}