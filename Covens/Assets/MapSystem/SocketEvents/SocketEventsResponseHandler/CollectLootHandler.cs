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
            public double timestamp;
        }

        public void HandleResponse(string json)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(json);

            string debug = data.silver + " silver\n" + data.gold + " gold\n" + data.xp;

            PlayerDataManager.playerData.AddCurrency(data.silver, data.gold);
            CharacterXpHandler.HandleEvent(PlayerDataManager.playerData.xp + data.xp, data.timestamp);

            foreach (var item in data.collectibles)
            {
                PlayerDataManager.playerData.AddIngredient(item.collectible, item.amount);
                debug += $"{item.collectible}({item.amount}),";
            }

            UIGlobalPopup.ShowPopUp(null,  debug);
        }
    }
}