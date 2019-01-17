using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public static class SpiritFormAnalytics
    {
        public static void EnterSpiritForm()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
            };
            AnalyticsAPI.Instance.LogEvent("spiritform_enter", data);
        }

        public static void LeaveSpiritForm()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
            };
            AnalyticsAPI.Instance.LogEvent("spiritform_exit", data);
        }
    }
}