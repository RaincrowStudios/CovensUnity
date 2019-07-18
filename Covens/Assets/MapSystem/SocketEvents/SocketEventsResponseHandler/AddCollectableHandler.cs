using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class AddCollectableHandler : AddTokenHandler
    {
        public const string ResponseName = "add.token.item";

        public override void HandleResponse(string eventData)
        {
            CollectableToken item = JsonConvert.DeserializeObject<CollectableToken>(eventData);
            HandleEvent(item);
        }
    }
}
