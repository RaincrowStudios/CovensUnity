using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public static class GameplayAnalytics
    {
        public static void CastSpell(string spell, string target, List<spellIngredientsData> ingredients)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "spell", spell },
                { "target", target },
                { "ingredients", ingredients }
            };
            AnalyticsAPI.Instance.LogEvent("cast_spell", data);
        }

        public static void SummonSpirit(string id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "id", id }
            };
            AnalyticsAPI.Instance.LogEvent("summon", data);
        }

        public static void CollectItem(MarkerSpawner.MarkerType type)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "type", type.ToString() }
            };
            AnalyticsAPI.Instance.LogEvent("collect", data);
        }
    }
}