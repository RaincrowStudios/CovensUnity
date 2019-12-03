using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddBossHandler : AddTokenHandler
    {
        public override string EventName => "add.token.boss";

        public override void HandleResponse(string eventData)
        {
            BossToken spirit = JsonConvert.DeserializeObject<BossToken>(eventData);
            HandleEvent(spirit);
        }
    }
}