using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddSpiritHandler : AddTokenHandler
    {
        public const string ResponseName = "add.token.spirit";

        public override void HandleResponse(string eventData)
        {
            SpiritToken spirit = JsonConvert.DeserializeObject<SpiritToken>(eventData);
            HandleEvent(spirit);
        }
    }
}