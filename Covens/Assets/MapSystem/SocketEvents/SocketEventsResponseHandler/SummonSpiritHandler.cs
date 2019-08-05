using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SummonSpiritHandler : AddTokenHandler
    {
        public override string EventName => "summon.spirit";

        public override void HandleResponse(string eventData)
        {
            SpiritToken spirit = JsonConvert.DeserializeObject<SpiritToken>(eventData);
            HandleEvent(spirit);
        }
    }
}
