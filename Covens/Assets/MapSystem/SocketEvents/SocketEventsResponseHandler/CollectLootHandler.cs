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
            public struct Collectible
            {
                public int amount;
                public string collectible;
                public string type;
            }

            public int silver;
            public int gold;
            public ulong xp;
            public Collectible[] collectibles;
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
                        PlayerDataManager.playerData.AddIngredient(item.collectible, item.amount);
            }

            if (data.cosmetics != null)
            {
                foreach (var item in data.cosmetics)
                {
                    bool owned = false;
                    foreach (var cosmetic in PlayerDataManager.playerData.inventory.cosmetics)
                    {
                        if (cosmetic.id == item)
                        {
                            owned = true;
                            break;
                        }
                    }

                    if (!owned)
                    {
                        PlayerDataManager.playerData.inventory.cosmetics.Add(DownloadedAssets.GetCosmetic(item));
                    }
                }
            }

            UILootRewards.Instantiate().Show(data);
        }
    }
}