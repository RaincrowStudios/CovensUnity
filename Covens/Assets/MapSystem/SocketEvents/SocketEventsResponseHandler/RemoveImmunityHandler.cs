using UnityEngine;
using System.Collections;
using Raincrow.Maps;

namespace Raincrow.GameEventResponses
{
    public class RemoveImmunityHandler : IGameEventHandler
    {
        public string EventName => "expire.immunity";
        public static System.Action<string, string, bool> OnImmunityChange;

        public struct RemoveImmunityEventData
        {
            public string caster;
            public string target;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            RemoveImmunityEventData response = JsonUtility.FromJson<RemoveImmunityEventData>(eventData);
            OnRemoveImmunity(response);
        }

        private static void OnRemoveImmunity(RemoveImmunityHandler.RemoveImmunityEventData data)
        {
            PlayerData player = PlayerDataManager.playerData;

            MarkerSpawner.RemoveImmunity(data.caster, data.target);
            OnImmunityChange?.Invoke(data.caster, data.target, false);

            if (data.caster == player.instance)
            {
                //remove the fx if the witch was immune to me
                IMarker marker = MarkerManager.GetMarker(data.target);
                if (marker != null && marker is WitchMarker)
                    (marker as WitchMarker).OnRemoveImmunity();

                return;
            }
        }
    }
}
