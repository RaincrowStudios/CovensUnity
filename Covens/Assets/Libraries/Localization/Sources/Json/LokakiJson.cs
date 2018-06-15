using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Oktagon.Localization
{
    public class LokakiJson : ILokakiSource
    {
        private const string ASSET_PATH = "GameSettings/Lokakit/";
        private const string FileExtension = ".json";

        private Lokaki.LANGUAGES m_eLang = Lokaki.LANGUAGES.None;
        private Dictionary<string, string> m_dData = new Dictionary<string, string>();
        public Dictionary<string, string> Data
        {
            get { return m_dData; }
        }


        public bool Contains(string sID)
        {
            return m_dData.ContainsKey(sID);
        }

        public string GetText(string sID)
        {
            if (Contains(sID))
            {
                return m_dData[sID];
            }
            return "undefined[" + sID + "]";
        }

        public bool Load(Lokaki.LANGUAGES eLang)
        {
            if (m_eLang == eLang)
                return true;

            Debug.Log("Load: " + eLang);
            string sFilePath = ASSET_PATH + eLang.ToString() + FileExtension;
            TextAsset pTextFile = (TextAsset)Resources.Load(sFilePath, typeof(TextAsset));
            if (pTextFile != null)
            {
                string sFileText = pTextFile.text;
                Setup(eLang, sFileText);
                return true;
            }
            Debug.LogError("File not loaded: " + sFilePath);
            return false;
        }

        public void Setup(Lokaki.LANGUAGES eLang, string sContext)
        {
            if (m_eLang == eLang)
                return;

            Debug.Log("Setup: " + eLang);
            m_dData = new Dictionary<string, string>();
            LokakiJsonData pData = JsonUtility.FromJson<LokakiJsonData>(sContext);
            foreach (LokakiJsonItem pItem in pData.items)
            {
                if (m_dData.ContainsKey(pItem.key))
                {
                    Debug.LogError("Key[" + pItem.key + "] already exists");
                    continue;
                }
                m_dData.Add(pItem.key, pItem.value);
            }
            m_eLang = eLang;
        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<string, string> hash in Data)
            {
                s += hash.Key + ": " + hash.Value + "\n";
            }

            return s;
        }


    }
}
