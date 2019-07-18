using Newtonsoft.Json;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class SpellCastHandler : IGameEventHandler
    {
        public struct Character
        {
            public string id;
            public string type;
            public int energy;

            [JsonIgnore]
            public MarkerManager.MarkerType Type => Token.TypeFromString(type);
        }

        public struct Result
        {
            public int damage;
            public bool isCritical;
            public bool isSuccess;
        }

        public struct SpellCastEventData
        {
            public string spell;
            public Character caster;
            public Character target;
            public Result result;
            public double timestamp;
            public bool immunity;
        }

        public const string ResponseName = "cast.spell";

        public void HandleResponse(string eventData)
        {
            SpellCastEventData response = JsonConvert.DeserializeObject<SpellCastEventData>(eventData);
            OnMapSpellcast.HandleEvent(response);
        }
    }   
}
