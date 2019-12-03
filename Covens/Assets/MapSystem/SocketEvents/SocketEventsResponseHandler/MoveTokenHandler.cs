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
            HandleEvent(data.instance, data.longitude, data.latitude);
        }

        public static void HandleEvent(string instance, float longitude, float latitude)
        {
            if (MarkerSpawner.Markers.ContainsKey(instance))
            {
                IMarker marker = MarkerSpawner.GetMarker(instance);

                if (marker == null)
                    marker.Coords = new Vector2(longitude, latitude);

                Vector3 targetPos = MapsAPI.Instance.GetWorldPosition(longitude, latitude);
                double distance = MapsAPI.Instance.DistanceBetweenPointsD(PlayerManager.marker.Coords, new Vector2(longitude, latitude));

                //triger the move events
                if (distance < PlayerDataManager.DisplayRadius)
                {
                    OnTokenMove?.Invoke(instance, targetPos);

                    if (marker != null)
                        OnMarkerMove?.Invoke(marker, targetPos);
                }
                //remove if the token is too far away
                else
                {
                    RemoveTokenHandler.OnTokenRemove?.Invoke(instance);
                    if (marker != null)
                    {
                        RemoveTokenHandler.OnMarkerRemove?.Invoke(marker);

                        MarkerSpawner.DeleteMarker(instance);
                        //animate it 
                        if (marker.inMapView)
                        {
                            marker.Interactable = false;

                            Vector3 direction = (targetPos - marker.GameObject.transform.position).normalized * 100;
                            targetPos = marker.GameObject.transform.position + direction;

                            marker.SetWorldPosition(targetPos, 2f);
                            //marker.SetAlpha(0, 1);
                        }
                    }
                }
            }
        }
    }
}