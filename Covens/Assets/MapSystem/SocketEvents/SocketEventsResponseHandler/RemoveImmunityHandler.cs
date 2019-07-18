using UnityEngine;
using System.Collections;

namespace Raincrow.GameEventResponses
{
    public class RemoveImmunityHandler : IGameEventHandler
    {
        public const string ResponseName = "expire.immunity";

        public struct RemoveImmunityEventData
        {
            public string caster;
            public string target;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            RemoveImmunityEventData response = JsonUtility.FromJson<RemoveImmunityEventData>(eventData);
            OnMapImmunityChange.OnRemoveImmunity(response);
        }
    }
}
