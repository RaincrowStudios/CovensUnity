using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class QuestsController
{
    public class CovenDaily
    {
        public struct Spellcraft
        {
            public int amount;
            public string country;
            public int? degree;
            public string ingredient;
            public string relation;
            public string spell;
            public string type;
        }

        public struct Gather
        {
            public string country;
            public string type;
            public int amount;
        }

        public struct Explore
        {
            public struct Location
            {
                public string type;
                public float[] coordinates;
                [JsonIgnore] public float latitude => coordinates != null && coordinates.Length > 0 ? coordinates[0] : 0;
                [JsonIgnore] public float longitude => coordinates != null && coordinates.Length > 1 ? coordinates[1] : 0;
            }

            public int amount;
            public string type;
            public Location location;
            public string id;
        }

        public bool active;
        public double startDate;
        public double endDate;

        public Spellcraft spellcraft;
        public Gather gather;
        public Explore explore;
    }

    //public static CovenDaily Quests { get; private set; }


    //public void ExploreQuestDone(string id)
    //{
    //    SoundManagerOneShot.Instance.MenuSound();
    //    SoundManagerOneShot.Instance.PlayReward();
    //    ExploreQuestObject.SetActive(true);
    //    var g = Utilities.InstantiateUI(ExploreQuestObject, NotificationTransform);
    //    g.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy(g); });
    //    g.transform.GetChild(2).GetComponent<Text>().text = LocalizeLookUp.GetExploreTitle(id);
    //    g.transform.GetChild(3).GetComponent<Text>().text = LocalizeLookUp.GetExploreDesc(id);
    //}

    //public void OnProgress(string quest, int count, int silver)
    //{
    //    StartCoroutine(OnProgressHelper(quest, count, silver));
    //}

    //IEnumerator OnProgressHelper(string quest, int count, int silver)
    //{
    //    yield return new WaitForSeconds(3.5f);
    ////    var pQuest = PlayerDataManager.playerData.dailies;

    ////    string message = null;

    ////    if (silver == 0)
    ////    {
    ////        if (quest == "gather")
    ////        {
    ////message = LocalizeLookUp.GetText("daily_quest_progress") + " " + LocalizeLookUp.GetText("daily_gather") + "\n" + LocalizeLookUp.GetText("daily_completed") + " " + count.ToString() + "/" + pQuest.gather.amount.ToString();
    ////            pQuest.gather.count = count;
    ////        }
    ////        else if (quest == "spellcraft")
    ////        {
    ////message = LocalizeLookUp.GetText("daily_quest_progress") + " " + LocalizeLookUp.GetText("daily_spell") + "\n" + LocalizeLookUp.GetText("daily_completed") + " " + count.ToString() + "/" + pQuest.gather.amount.ToString();
    ////            pQuest.spellcraft.count = count;
    ////        }
    ////    }
    ////    else
    ////    {
    ////        if (quest == "gather")
    ////        {
    ////message = LocalizeLookUp.GetText ("daily_completed_gather") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
    ////            pQuest.gather.count = count;
    ////            pQuest.explore.complete = true;

    ////        }
    ////        else if (quest == "spellcraft")
    ////        {
    ////message = LocalizeLookUp.GetText ("daily_completed_spellcraft") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
    ////            pQuest.spellcraft.count = count;
    ////            pQuest.spellcraft.complete = true;
    ////        }
    ////        else
    ////        {
    ////message = LocalizeLookUp.GetText ("daily_completed_explore") + "\n" + "+ " + silver.ToString () +  " " + LocalizeLookUp.GetText ("store_silver");
    ////            pQuest.explore.count = 1;
    ////            pQuest.explore.complete = true;
    ////        }
    ////    }

    ////    if (message != null)
    ////    {
    ////        PlayerNotificationManager.Instance.ShowNotification(message, m_NotificationIcon);
    ////    }
    ////    else
    ////    {
    ////        Debug.LogError("null quest progress text");
    ////    }

    ////    //yield return new WaitForSeconds(5f);
    ////    //Destroy(g);
    //}

    public static CovenDaily Quests => PlayerDataManager.playerData.quest.daily;

    public static void GetQuests(System.Action<string> callback)
    {
        if (PlayerDataManager.playerData.quest.daily != null)
        {
            System.TimeSpan timeRemaing = Utilities.TimespanFromJavaTime(PlayerDataManager.playerData.quest.daily.endDate);
            if (timeRemaing.TotalSeconds > 0)
            {
                callback?.Invoke(null);
                return;
            }
        }

        Debug.Log("quests expired or null, retrieving new quests");

        APIManager.Instance.Get("dailies/quest",
            (string result, int response) =>
            {
                if (response == 200)
                {
                    PlayerDataManager.playerData.quest.daily = Newtonsoft.Json.JsonConvert.DeserializeObject<CovenDaily>(result);
                    callback?.Invoke(null);
                }
                else
                {
                    callback?.Invoke(APIManager.ParseError(result));
                }
            });
    }

    public static void CompleteExplore(System.Action<string> callback)
    {
        APIManager.Instance.Get("dailies/explore", (response, result) =>
        {
            if (result == 200)
            {
                DailyProgressHandler.DailyProgressEventData data =
                    JsonConvert.DeserializeObject<DailyProgressHandler.DailyProgressEventData>(response);

                DailyProgressHandler.HandleResponse(data);

                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(APIManager.ParseError(response));
            }
        });
    }

    public static void ClaimRewards(System.Action<DailyRewards, string> callback)
    {
        APIManager.Instance.Get("dailies/reward",
            (string result, int response) =>
            {
                if (response == 200)
                {
                    DailyRewards rewards = JsonConvert.DeserializeObject<DailyRewards>(result);

                    PlayerDataManager.playerData.quest.completed = true;
                    PlayerDataManager.playerData.silver += rewards.silver;
                    PlayerDataManager.playerData.gold += rewards.gold;
                    PlayerDataManager.playerData.AddEnergy(rewards.energy);

                    if (rewards.effect != null)
                        ConditionManager.AddCondition(rewards.effect, PlayerManager.marker);

                    if (PlayerDataManager.Instance != null)
                    {
                        PlayerManagerUI.Instance.UpdateDrachs();
                    }

                    callback?.Invoke(rewards, null);
                }
                else
                {
                    callback?.Invoke(new DailyRewards(), APIManager.ParseError(result));
                }
                //if (response == 200)
                //{
                //    var reward = JsonConvert.DeserializeObject<Rewards>(result);
                //    StartCoroutine(ShowRewards(reward));
                //    PlayerDataManager.playerData.quest.completed = true;
                //}
                //else
                //{
                //    Debug.Log(result + response);
                //    bottomInfo.text = LocalizeLookUp.GetText("daily_could_not_claim");//"Couldn't Claim rewards . . .";
                //}
            });
    }
}