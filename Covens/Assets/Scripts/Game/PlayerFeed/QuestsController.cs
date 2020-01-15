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

    public static event System.Action OnCollectDailyRewards;
    public static event System.Action OnResetDaily;

    public static CovenDaily Quests => PlayerDataManager.playerData.quest.daily;

    public static int m_DailyResetTweenId;

    public static void GetQuests(System.Action<string> callback)
    {
        if (PlayerDataManager.playerData.quest.daily != null)
        {
            System.TimeSpan timeRemaing = Utilities.TimespanFromJavaTime(PlayerDataManager.playerData.quest.daily.endDate);
            if (timeRemaing.TotalSeconds > 0)
            {
                ResetDailyTimer(PlayerDataManager.playerData.quest.daily.endDate);
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
                    ResetDailyTimer(PlayerDataManager.playerData.quest.daily.endDate);
                    callback?.Invoke(null);
                }
                else
                {
                    callback?.Invoke(APIManager.ParseError(result));
                    Debug.LogException(new System.Exception("failed to retrieve quests:\n" + result));
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
                callback?.Invoke(response);
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
                    OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy + rewards.energy);

                    if (rewards.effect != null)
                        MarkerSpawner.ApplyStatusEffect(PlayerDataManager.playerData.instance, PlayerDataManager.playerData.instance, rewards.effect);

                    if (PlayerDataManager.Instance != null)
                    {
                        PlayerManagerUI.Instance.UpdateDrachs();
                    }

                    callback?.Invoke(rewards, null);
                    OnCollectDailyRewards?.Invoke();
                }
                else
                {
                    callback?.Invoke(new DailyRewards(), APIManager.ParseError(result));
                }
            });
    }

    private static void ResetDailyTimer(double expireOn)
    {
        var expireDate = Utilities.TimespanFromJavaTime(expireOn);

        Debug.Log("[QuestController] daily reset in " + expireDate.TotalSeconds);

        LeanTween.cancel(m_DailyResetTweenId);
        m_DailyResetTweenId = LeanTween.value(0, 0, 0)
            .setDelay((float)expireDate.TotalSeconds)
            .setOnComplete(() => 
            {
                GetQuests((error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        OnResetDaily?.Invoke();
                    }
                    else
                    {
                        Debug.LogError("failed to reset dailies:\n" + error);
                    }
                });
            })
            .uniqueId;
    }
}