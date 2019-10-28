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

    public static event System.Action OnCollectDailyRewards;

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
}