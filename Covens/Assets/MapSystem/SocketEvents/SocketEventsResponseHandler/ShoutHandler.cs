using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class ShoutHandler : IGameEventHandler
    {
        public const string EventName = "shout";

        public struct ShoutEventData
        {
            public string message;
            public double timestamp;
        }

        public void HandleResponse(string eventData)
        {
            ShoutEventData data = JsonConvert.DeserializeObject<ShoutEventData>(eventData);
            throw new System.Exception("TODO/IS ID STILL MISSING?");

            //if (data.instance == PlayerDataManager.playerData.instance)
            //{
            //    ShoutBoxData shoutBox = m_ShoutPool.Spawn().GetComponent<ShoutBoxData>();
            //    PlayerManager.marker.AddChild(shoutBox.transform, PlayerManager.marker.characterTransform, m_ShoutPool);
            //    shoutBox.Setup(PlayerManager.marker, data.displayName, data.shout, () => PlayerManager.marker.RemoveChild(shoutBox.transform));
            //    shoutBox.transform.localPosition = new Vector3(0, 32, 0);
            //}
            //else
            //{
            //    if (MarkerManager.Markers.ContainsKey(data.instance))
            //    {
            //        IMarker marker = MarkerManager.GetMarker(data.instance);
            //        if (marker != null)
            //        {
            //            marker.SpawnFX(m_ShoutPool, true, 5, false, (boxTransform) =>
            //            {
            //                ShoutBoxData shoutBox = boxTransform.GetComponent<ShoutBoxData>();
            //                shoutBox.Setup(marker, data.displayName, data.shout, null);
            //                shoutBox.transform.localPosition = new Vector3(0, 32, 0);
            //            });
            //        }
            //    }
            //}
        }
    }
}