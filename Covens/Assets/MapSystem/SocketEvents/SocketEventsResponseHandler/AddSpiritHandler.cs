using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddSpiritHandler : AddTokenHandler
    {
        public override string EventName => "add.token.spirit";

        public override void HandleResponse(string eventData)
        {
            SpiritToken spirit = JsonConvert.DeserializeObject<SpiritToken>(eventData);
            HandleEvent(spirit);
        }
    }
}