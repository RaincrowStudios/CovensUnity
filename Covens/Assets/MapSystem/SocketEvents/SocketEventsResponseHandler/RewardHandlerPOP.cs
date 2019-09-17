using Newtonsoft.Json;

using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class RewardHandlerPOP : IGameEventHandler
    {
        public string EventName => "reward.pop";
        public static event System.Action<RewardPOPData> LocationReward;

        public struct RewardPOPData
        {
            public int gold;
            public int silver;
            public int xp;
            public double timestamp;
        }


        public void HandleResponse(string eventData)
        {
            RewardPOPData data = JsonConvert.DeserializeObject<RewardPOPData>(eventData);
            LocationReward?.Invoke(data);
        }
    }
}
