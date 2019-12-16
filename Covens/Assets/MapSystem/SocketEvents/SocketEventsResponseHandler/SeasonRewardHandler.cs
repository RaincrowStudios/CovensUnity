using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SeasonRewardHandler : IGameEventHandler
    {
        public string EventName => "season.reward";

        public struct EventData
        {
            public CollectibleData[] items;
            public string[] cosmetics;
            public int gold;
            public int silver;
            public int power;
            public int resilience;

            public bool coven;
            public int position;

            public double timestamp;
        }

        public void HandleResponse(string json)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(json);

            if (data.items != null)
            {
                foreach (var item in data.items)
                    PlayerDataManager.playerData.AddIngredient(item.id, item.amount);
            }

            if (data.cosmetics != null)
            {
                foreach (var item in data.cosmetics)
                    PlayerDataManager.playerData.inventory.AddCosmetic(item);
            }

            PlayerDataManager.playerData.AddCurrency(data.silver, data.gold);
            PlayerDataManager.playerData.basePower += data.power;
            PlayerDataManager.playerData.baseResilience += data.resilience;

            UISeasonRewardsPopup.Instantiate().Show(data);
        }
    }
}
