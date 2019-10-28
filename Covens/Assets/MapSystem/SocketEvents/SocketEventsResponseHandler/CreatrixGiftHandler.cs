using UnityEngine;
using Newtonsoft.Json;
using Raincrow.Maps;
using TMPro;

namespace Raincrow.GameEventResponses
{
    public class CreatrixGiftHandler : IGameEventHandler
    {
        private struct GiftData
        {
            public string id;
            public int gold;
            public int silver;
            public int energy;
            public double timestamp;
        }

        public string EventName => "creatrix.gift";

        public void HandleResponse(string eventData)
        {
            GiftData data = JsonConvert.DeserializeObject<GiftData>(eventData);

            if (data.id == PlayerDataManager.playerData.instance)
            {
                IMarker marker = PlayerManager.marker;
                OnMapEnergyChange.ForceEvent(PlayerManager.marker, PlayerDataManager.playerData.energy + data.energy, data.timestamp);
                PlayerDataManager.playerData.AddCurrency(data.silver, data.gold);
            }
            else
            {
                IMarker marker = MarkerSpawner.GetMarker(data.id);
                if (marker != null && marker is CharacterMarker)
                {
                    CharacterMarker character = marker as CharacterMarker;
                    OnMapEnergyChange.ForceEvent(character, (character.Token as CharacterToken).energy +  data.energy, data.timestamp);
                }
            }
        }
    }
}