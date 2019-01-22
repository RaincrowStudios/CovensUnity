using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public class PoPAnalytics
    {
        public static void EnterPoP(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "id", id },
            };

            AnalyticsAPI.Instance.LogEvent("enter_pop", data);
        }
    }
}