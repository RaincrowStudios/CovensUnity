using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public class PurchaseAnalytics
    {
        public static void PurchaseItem(string id, bool isCosmetic)
        {
            var data = new Dictionary<string, object>
            {
                { "id", id },
                { "cosmetic", isCosmetic }
            };
            AnalyticsAPI.Instance.LogEvent("purchase_item", data);
        }

        public static void StartIAP(string id)
        {
            var data = new Dictionary<string, object>
            {
                { "id", id }
            };
            AnalyticsAPI.Instance.LogEvent("iap_start", data);
        }

        public static void CompleteIAP(string id)
        {
            var data = new Dictionary<string, object>
            {
                { "id", id }
            };
            AnalyticsAPI.Instance.LogEvent("iap_finish", data);
        }
    }
}