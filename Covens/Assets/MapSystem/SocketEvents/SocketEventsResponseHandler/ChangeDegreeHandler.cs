using Newtonsoft.Json;
using Raincrow.Maps;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class ChangeDegreeHandler : IGameEventHandler
    {
        public struct ChangeDegreeData
        {
            [JsonProperty("_id")]
            public string instance;
            public int degree;
            public double timestamp;
        }

        public string EventName => "change.degree";

        public void HandleResponse(string eventData)
        {
            ChangeDegreeData data = JsonConvert.DeserializeObject<ChangeDegreeData>(eventData);

            if (data.instance == PlayerDataManager.playerData.instance)
            {
                int oldDegree = PlayerDataManager.playerData.degree;
                if (oldDegree != data.degree)
                {
                    PlayerDataManager.playerData.degree = data.degree;
                    PlayerManagerUI.Instance.playerDegreeChanged();
                    UIDegreeChanged.Instance.Show(oldDegree, data.degree);
                }
            }
            else
            {
                WitchMarker marker = MarkerManager.GetMarker(data.instance) as WitchMarker;
                if (marker != null)
                {
                    WitchToken token = marker.customData as WitchToken;
                    if (token != null)
                    {
                        token.degree = data.degree;
                        marker.SetRingAmount();
                    }
                }
            }
        }
    }
}