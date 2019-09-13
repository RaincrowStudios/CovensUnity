using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.FTF
{
    public struct FirstTapEntry
    {
        public string title;
        public string description;
        public FTFRectData highlight;
        public List<FTFPointData> glow;
    }

    public static class FirstTapManager
    {
        private static Dictionary<string, FirstTapEntry> m_FirstTapDict;

        private static Dictionary<string, FirstTapEntry> FirstTapDict
        {
            get
            {
                if (m_FirstTapDict == null)
                    m_FirstTapDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, FirstTapEntry>>(Resources.Load<TextAsset>("FirstTap").text);

                return m_FirstTapDict;
            }
        }

        public static bool IsFirstTime(string id)
        {
            return PlayerPrefs.GetInt("first." + id + "." + PlayerDataManager.playerData.instance, PlayerDataManager.playerData.firsts.Contains(id) ? 0 : 1) == 1;
        }
        
        public static void SetFirstTime(string id, bool value)
        {
            PlayerPrefs.SetInt("first." + id + "." + PlayerDataManager.playerData.instance, value ? 1 : 0);
        }

        public static void Show(string id, System.Action onComplete)
        {
            if (FirstTapDict.ContainsKey(id) == false)
            {
                Debug.LogError("first tap entry \"" + id + "\" not found");
                onComplete?.Invoke();
                return;
            }

            UIFirstTap.Show(id, FirstTapDict[id], () =>
            {
                SetFirstTime(id, false);
                onComplete?.Invoke();
            });
        }

        public static bool CompletedAll()
        {
            foreach (string id in m_FirstTapDict.Keys)
            {
                if (IsFirstTime(id))
                    return false;
            }

            return true;
        }

        public static void ResetFirsts()
        {
            foreach (string id in FirstTapDict.Keys)
                SetFirstTime(id, true);
        }
    }
}