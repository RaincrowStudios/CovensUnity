using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public static class CastSpellEvent
    {
        public static void Trigger(string spell, string target)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "spell", spell },
                { "target", target }
            };

            AnalyticsAPI.Instance.LogEvent(data);
        }
    }
}