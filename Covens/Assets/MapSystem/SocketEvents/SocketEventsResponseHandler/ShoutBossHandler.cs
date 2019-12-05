using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class ShoutBossHandler : IGameEventHandler
    {
        public string EventName => "shout.boss";
        public static SimplePool<ShoutBoxData> m_ShoutPool = new SimplePool<ShoutBoxData>("UI/Shoutbox");

        public struct ShoutEventData
        {
            public string id;
            public string shout;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            ShoutEventData data = JsonConvert.DeserializeObject<ShoutEventData>(eventData);
            
            if (MarkerSpawner.Markers.ContainsKey(data.id))
            {
                WorldBossMarker bossMarker = MarkerSpawner.GetMarker(data.id) as WorldBossMarker;
                string name = LocalizeLookUp.GetSpiritName(bossMarker.bossToken.spiritId);
                string message = LocalizeLookUp.GetText(data.shout);

                ShoutBoxData shoutBox = m_ShoutPool.Spawn(bossMarker.AvatarTransform).GetComponent<ShoutBoxData>();
                shoutBox.Setup(bossMarker, name, message, () => m_ShoutPool.Despawn(shoutBox));
                shoutBox.transform.localPosition = new Vector3(0, 30, 0);
            }
        }
    }
}