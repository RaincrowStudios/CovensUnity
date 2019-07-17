using UnityEngine;
using System.Collections;

namespace Raincrow.GameEventResponses
{
    public class AddImmunityHandler : IGameEventHandler
    {
        public const string ResponseName = "add.immunity";

        public struct AddImmunityEventData
        {
            public string caster;
            public string target;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            AddImmunityEventData response = JsonUtility.FromJson<AddImmunityEventData>(eventData);
            OnMapImmunityChange.OnAddImmunity(response);
        }
    }
}
