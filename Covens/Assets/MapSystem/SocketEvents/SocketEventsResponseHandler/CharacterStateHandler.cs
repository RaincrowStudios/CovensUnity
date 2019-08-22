using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterStateHandler : IGameEventHandler
    {
        public string EventName => "character.state";

        public void HandleResponse(string eventData)
        {
            Debug.LogError("TODO: HANDLE CHARACTER.STATE");
        }
    }
}