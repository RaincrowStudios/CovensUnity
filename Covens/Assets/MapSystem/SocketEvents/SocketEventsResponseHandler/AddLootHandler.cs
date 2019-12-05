using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddLootHandler : AddTokenHandler
    {
        public override string EventName => "add.token.loot";

        public override void HandleResponse(string eventData)
        {
            LootToken loot = JsonConvert.DeserializeObject<LootToken>(eventData);
            HandleEvent(loot);
        }
    }
}