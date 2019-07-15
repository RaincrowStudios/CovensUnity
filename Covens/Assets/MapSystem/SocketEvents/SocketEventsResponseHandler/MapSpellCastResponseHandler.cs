using UnityEngine;

namespace Raincrow.GameEventResponses
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
