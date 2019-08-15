using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class CharacterEquiphandler : IGameEventHandler
    {
        public string EventName => "character.equip";

        private struct EquipEventData
        {
            public string character;
            public List<EquippedApparel> equips;
        }

        public void HandleResponse(string eventData)
        {
            EquipEventData data = JsonConvert.DeserializeObject<EquipEventData>(eventData);
            IMarker marker = MarkerSpawner.GetMarker(data.character);

            if (marker.Type != MarkerManager.MarkerType.WITCH)
                return;

            (marker as WitchMarker).UpdateEquips(data.equips);
        }
    }
}