using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class ShoutHandler : IGameEventHandler
    {
        public string EventName => "shout";
        public static SimplePool<ShoutBoxData> m_ShoutPool = new SimplePool<ShoutBoxData>("UI/Shoutbox");

        public struct ShoutEventData
        {
            public string id;
            public string message;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            ShoutEventData data = JsonConvert.DeserializeObject<ShoutEventData>(eventData);
            SpawnShoutbox(data.id, data.message);
        }

        public static void SpawnShoutbox(string character, string message)
        {
            IMarker marker = null;
            string name = "";

            if (character == PlayerDataManager.playerData.instance)
            {
                marker = PlayerManager.marker;
                name = PlayerDataManager.playerData.name;
            }
            else
            {
                if (MarkerManager.Markers.ContainsKey(character))
                {
                    marker = MarkerManager.GetMarker(character);
                    if (marker is WitchMarker)
                        name = (marker as WitchMarker).witchToken.displayName;
                    else if (marker is SpiritMarker)
                        name = (marker as SpiritMarker).spiritData.Name;
                }
            }

            if (marker != null)
            {
                ShoutBoxData shoutBox = m_ShoutPool.Spawn(marker.AvatarTransform).GetComponent<ShoutBoxData>();
                shoutBox.Setup(marker, name, message, () => m_ShoutPool.Despawn(shoutBox));
                shoutBox.transform.localPosition = new Vector3(0, 32, 0);
            }
        }
    }
}