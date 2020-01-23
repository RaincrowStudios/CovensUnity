using Newtonsoft.Json;
using Oktagon.Analytics;
using Raincrow.Analytics;
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
                PushAnalyticEvent(data.degree);
                
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
                WitchMarker marker = MarkerSpawner.GetMarker(data.instance) as WitchMarker;
                if (marker != null)
                {
                    WitchToken token = marker.witchToken;
                    if (token != null)
                    {
                        token.degree = data.degree;
                        marker.SetRingColor();
                    }
                }
            }
        }

        private void PushAnalyticEvent(int degree)
        {
            string witchType = "";

            if (degree < 0)
                witchType = "shadow";
            else if (degree > 0)
                witchType = "white";
            else
                witchType = "grey";

            System.Collections.Generic.Dictionary<string, object> eventParams = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "clientVersion", Application.version },
                    { "alignment", witchType},
                    { "degree", Mathf.Abs(degree)}
                };

            OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.DegreeChange, eventParams);
        }
    }
}