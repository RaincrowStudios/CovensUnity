using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddWitchHandler : AddTokenHandler
    {
        public const string EventName = "add.token.character";

        public override void HandleResponse(string eventData)
        {
            WitchToken witch = JsonConvert.DeserializeObject<WitchToken>(eventData);
            HandleEvent(witch);
        }
    }
}
