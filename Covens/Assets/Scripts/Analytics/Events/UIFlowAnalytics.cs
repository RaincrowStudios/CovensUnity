using UnityEngine;
using System.Collections.Generic;

namespace Raincrow.Analytics.Events
{
    public class UIFlowAnalytics
    {
        public static void OpenWardrobe()
        {
            AnalyticsAPI.Instance.LogEvent("ui_wardrobe");
        }

        public static void OpenKytelers()
        {
            AnalyticsAPI.Instance.LogEvent("ui_kytelers");
        }

        public static void OpenLeaderboards(string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_leaderboards", data);
        }

        public static void OpenDailies(string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_dailies", data);
        }

        public static void OpenEventLog()
        {
            AnalyticsAPI.Instance.LogEvent("ui_eventlog");
        }

        public static void OpenInventory()
        {
            AnalyticsAPI.Instance.LogEvent("ui_inventory");
        }

        public static void OpenApothecary()
        {
            AnalyticsAPI.Instance.LogEvent("ui_apothecary");
        }

        public static void OpenCoven(bool isOwn, string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "own", isOwn },
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_coven", data);
        }

        public static void OpenChat(string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_coven");
        }

        public static void OpenStore(string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_coven");
        }

        public static void OpenSpirits(string tab)
        {
            var data = new Dictionary<string, object>
            {
                { "tab", tab }
            };
            AnalyticsAPI.Instance.LogEvent("ui_spirits", data);
        }

        public static void OpenSummon()
        {
            AnalyticsAPI.Instance.LogEvent("ui_summons");
        }

        public static  void OpenSpellbook()
        {
            AnalyticsAPI.Instance.LogEvent("ui_spellbook");
        }

        public static void OpenWitchSchool()
        {
            AnalyticsAPI.Instance.LogEvent("ui_witchschool");
        }
    }
}