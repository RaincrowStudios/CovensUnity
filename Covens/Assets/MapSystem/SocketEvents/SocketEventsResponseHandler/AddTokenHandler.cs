using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public abstract class AddTokenHandler : IGameEventHandler
    {
        public static event System.Action<string> OnTokenAdd;
        public static event System.Action<IMarker> OnMarkerAdd;

        //public virtual string EventName { get { return "add.token"; } }
        public abstract string EventName { get; }
        public abstract void HandleResponse(string eventData);

        public static void HandleEvent(Token token)
        {
            //wait for marker spawning to finish before trying to add this token
            if (MarkerManagerAPI.IsSpawningTokens)
            {
                LeanTween.value(0, 0, 1f).setOnComplete(() => HandleEvent(token));
                return;
            }

            if (token == null)
            {
                Debug.LogError("no token on map_token_add");
                return;
            }

            //ignore if somehow trying to add a token of the local player
            if (token.instance == PlayerDataManager.playerData.instance)
                return;

            //ignore if the token is already dead
            if (token.type == "spirit" && ((token as SpiritToken).energy <= 0 || (token as SpiritToken).state == "dead"))
                return;
                        
            IMarker marker = MarkerSpawner.GetMarker(token.instance);
            bool isNew = marker == null;

            marker = MarkerSpawner.Instance.AddMarker(token);

            if (marker == null)
                return;

            //if (isNew)
            //{
            //    marker.GameObject.SetActive(false);
            //    marker.SetAlpha(0);
            //    marker.inMapView = false;
            //}

            OnMarkerAdd?.Invoke(marker);
            OnTokenAdd?.Invoke(token.instance);
        }

        public static void ForceEvent(Token token, bool forceCoordinates)
        {
            if (forceCoordinates)
            {
                token.longitude = PlayerDataManager.playerData.longitude;
                token.latitude = PlayerDataManager.playerData.latitude;
            }

            HandleEvent(token);
        }
    }
}