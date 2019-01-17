using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public static class EquipmentAnalytics
    {
        public static void ChangeEquipment()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
            };
            AnalyticsAPI.Instance.LogEvent("equipment_change", data);
        }
    }
}