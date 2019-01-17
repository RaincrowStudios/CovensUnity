using System.Collections;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public static class FlightAnalytics
    {
        public static void StartFlying()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
            };
            AnalyticsAPI.Instance.LogEvent("fly_start", data);
        }

        public static void Land()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
            };
            AnalyticsAPI.Instance.LogEvent("fly_end", data);
        }
    }
}
