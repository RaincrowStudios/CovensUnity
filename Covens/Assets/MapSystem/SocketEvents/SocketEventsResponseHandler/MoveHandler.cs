using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MoveHandler : IGameEventHandler
    {
        public struct MoveEventData
        {
            [JsonProperty("_id")]
            public string instance;

            public float longitude;
            public float latitude;
            public double timestamp;
        }

        public const string ResponseName = "move";
        public static event System.Action<string, Vector3> OnTokenMove;
        public static event System.Action<IMarker, Vector3> OnMarkerMove;

        public void HandleResponse(string eventData)
        {
            MoveEventData data = JsonConvert.DeserializeObject<MoveEventData>(eventData);
            HandleEvent(data);
        }

        private static void HandleEvent(MoveEventData data)
        {
            if (MarkerManager.Markers.ContainsKey(data.instance))
            {
                IMarker marker = MarkerSpawner.GetMarker(data.instance);

                if (marker == null)
                    marker.coords = new Vector2(data.longitude, data.latitude);

                Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(data.longitude, data.latitude);
                double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.coords, new Vector2(data.longitude, data.latitude));

                //triger the move events
                if (distance < PlayerDataManager.DisplayRadius)
                {
                    OnTokenMove?.Invoke(data.instance, targetPos);

                    if (marker != null)
                        OnMarkerMove?.Invoke(marker, targetPos);
                }
                //force a remove event in case the token is too far away
                else if (marker != null)
                {
                    marker.interactable = false;
                    marker.SetAlpha(0, 2f);
                    marker.SetWorldPosition(targetPos, 2f);
                    LeanTween.value(0, 0, 2f).setOnComplete(() => OnMapTokenRemove.ForceEvent(data.instance));
                }
            }
        }
    }
}