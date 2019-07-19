using UnityEngine;
using System.Collections;
using Raincrow.Maps;

namespace Raincrow.GameEventResponses
{
    public class AddImmunityHandler : IGameEventHandler
    {
        public const string EventName = "add.immunity";
        public static System.Action<string, string, bool> OnImmunityChange;

        public struct AddImmunityEventData
        {
            public string caster;
            public string target;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            AddImmunityEventData response = JsonUtility.FromJson<AddImmunityEventData>(eventData);
            OnAddImmunity(response);
        }

        private static void OnAddImmunity(AddImmunityHandler.AddImmunityEventData data)
        {
            PlayerData player = PlayerDataManager.playerData;

            MarkerSpawner.AddImmunity(data.caster, data.target);
            OnImmunityChange?.Invoke(data.caster, data.target, true);
            if (data.caster == player.instance)
            {
                //add the fx if the witch is now immune to me
                IMarker marker = MarkerManager.GetMarker(data.target);
                if (marker != null && marker is WitchMarker)
                    (marker as WitchMarker).AddImmunityFX();

                return;
            }
        }
    }
}
