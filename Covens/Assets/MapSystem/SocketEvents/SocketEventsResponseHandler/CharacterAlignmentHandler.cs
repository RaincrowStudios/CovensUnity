using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterAlignmentHandler : IGameEventHandler
    {
        public string EventName => "character.alignment";

        public struct EventData
        {
            public long alignment;
            public string actionId;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            EventData data = JsonConvert.DeserializeObject<EventData>(eventData);
            PlayerDataManager.playerData.alignment = data.alignment;
        }
    }
}