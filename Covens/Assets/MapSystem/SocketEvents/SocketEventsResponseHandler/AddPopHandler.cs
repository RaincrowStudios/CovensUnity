using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddPopHandler : AddTokenHandler
    {
        public override string EventName => "add.token.pop";

        public override void HandleResponse(string eventData)
        {
            PopToken pop = JsonConvert.DeserializeObject<PopToken>(eventData);
            HandleEvent(pop);
        }
    }
}