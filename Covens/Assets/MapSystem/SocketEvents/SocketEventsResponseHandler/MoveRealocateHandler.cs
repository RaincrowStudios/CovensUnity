using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MoveRealocateHandler : IGameEventHandler
    {
        public string EventName => "move.realocate";

        private struct RealocateEventData
        {
            public struct Tokens
            {
                public List<WitchToken> characters;
                public List<SpiritToken> spirits;
                public List<CollectableToken> items;
                public List<EnergyToken> energies;
                public List<PopToken> placesOfPower;
                public List<BossToken> boss;
                public List<LootToken> loots;
            }

            public double latitude;
            public double longitude;

            [JsonProperty("location")]
            public MarkerManagerAPI.MapMoveResponse.Location dominion;
            [JsonProperty("objects")]
            public Tokens tokens;
        }

        public void HandleResponse(string eventData)
        {
            RealocateEventData realocate = JsonConvert.DeserializeObject<RealocateEventData>(eventData);

            PlayerManager.marker.Coords = new Vector2((float)realocate.longitude, (float)realocate.latitude);
            PlayerDataManager.playerData.longitude = (float)realocate.longitude;
            PlayerDataManager.playerData.latitude = (float)realocate.latitude;

            MarkerManagerAPI.LoadMap(realocate.longitude, realocate.latitude, true, () =>
            {
                MarkerManagerAPI.UpdateDominion(realocate.dominion);
                MarkerManagerAPI.SpawnMarkers(
                    realocate.tokens.characters, 
                    realocate.tokens.spirits, 
                    realocate.tokens.items, 
                    realocate.tokens.energies,
                    realocate.tokens.placesOfPower,
                    realocate.tokens.boss,
                    realocate.tokens.loots);
            });
        }
    }
}