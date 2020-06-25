using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using Newtonsoft.Json;
using Oktagon.Analytics;
using Raincrow.Analytics;

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
            public int xp;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            DailyProgressEventData data = JsonConvert.DeserializeObject<DailyProgressEventData>(eventData);
            HandleResponse(data);
        }

        public static void HandleResponse(DailyProgressEventData data)
        {
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

            if (data.xp != 0)
            {
                System.Collections.Generic.Dictionary<string, object> eventParams = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "clientVersion", Application.version },
                    { "dailyQuestType", data.daily}
                };

                // Track quest finished.
                OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.DailyQuest, eventParams);

                PlayerDataManager.playerData.xp += System.Convert.ToUInt64(data.xp);
                PlayerManagerUI.Instance.setupXP();
            }

            ShowNotification(data.daily, data.xp, data.count);
        }

        public static void ShowNotification(string quest, int xp, int count)
        {
            string message = null;

            if (xp == 0)
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
                        "+ " + xp.ToString() + " XP";
                }
                else if (quest == "spellcraft")
                {
                    message = LocalizeLookUp.GetText("daily_completed_spellcraft") + "\n" +
                        "+ " + xp.ToString() + " XP";
                }
                else
                {
                    message = LocalizeLookUp.GetText("daily_completed_explore") + "\n" +
                        "+ " + xp.ToString() + " XP";
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