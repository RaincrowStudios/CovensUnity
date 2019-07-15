using UnityEngine;

namespace Raincrow.GameEvent
{
    public class MapSpellCastResponseHandler : IGameEventResponseHandler
    {
        public const string ResponseName = "cast.spell";

        public void HandleResponse(string eventData)
        {
            SpellCastResponse response = JsonUtility.FromJson<SpellCastResponse>(eventData);
            OnMapSpellcast.HandleEvent(response);
        }
    }   
}
