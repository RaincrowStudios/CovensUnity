using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MoveTokenHandler : IGameEventHandler
    {
        public struct MoveEventData
        {
            [JsonProperty("_id")]
            public string instance;
            public float longitude;
            public float latitude;
            public double timestamp;
        }

        public string EventName => "move";
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
                    marker.Coords = new Vector2(data.longitude, data.latitude);

                Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(data.longitude, data.latitude);
                double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.Coords, new Vector2(data.longitude, data.latitude));

                //triger the move events
                if (distance < PlayerDataManager.DisplayRadius)
                {
                    OnTokenMove?.Invoke(data.instance, targetPos);

                    if (marker != null)
                        OnMarkerMove?.Invoke(marker, targetPos);
                }
                //remove if the token is too far away
                else 
                {
                    RemoveTokenHandler.OnTokenRemove(data.instance);
                    if (marker != null)
                    {
                        RemoveTokenHandler.OnMarkerRemove(marker);

                        MarkerSpawner.DeleteMarker(data.instance);
                        //animate it 
                        if (marker.inMapView)
                        {
                            marker.Interactable = false;

                            Vector3 direction = (targetPos - marker.GameObject.transform.position).normalized * 100;
                            targetPos = marker.GameObject.transform.position + direction;

                            marker.SetWorldPosition(targetPos, 2f);
                            marker.SetAlpha(0, 1);
                        }
                    }
                }
            }
        }
    }
}