using Newtonsoft.Json;
using Raincrow.Maps;
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
                public WitchToken[] characters;
                public SpiritToken[] spirits;
                public CollectableToken[] items;
                public EnergyToken[] energies;
                public PopToken[] placesOfPower;
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
                    realocate.tokens.placesOfPower);
            });
        }
    }
}