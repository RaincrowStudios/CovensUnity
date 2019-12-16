using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CollectLootHandler : IGameEventHandler
    {
        public string EventName => "collect.loot";

        public struct EventData
        {
            public int silver;
            public int gold;
            public ulong xp;
            public CollectibleData[] collectibles;
            public string[] cosmetics;
            public string bossId;
            public string type;
            public double timestamp;
        }

        public void HandleResponse(string json)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(json);

            PlayerDataManager.playerData.AddCurrency(data.silver, data.gold);
            CharacterXpHandler.HandleEvent(PlayerDataManager.playerData.xp + data.xp, data.timestamp);

            if (data.collectibles != null)
            {
                foreach (var item in data.collectibles)
                        PlayerDataManager.playerData.AddIngredient(item.id, item.amount);
            }

            if (data.cosmetics != null)
            {
                foreach (var item in data.cosmetics)
                    PlayerDataManager.playerData.inventory.AddCosmetic(item);
            }

            UILootRewardsPopup.Instantiate().Show(data);
        }
    }
}