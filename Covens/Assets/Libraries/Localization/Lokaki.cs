
/// <summary>
/// when defined, will read the database of language from a txt file database.
/// This is slower than default mode, but more flexibe. You can even download the text asset and load it.
/// The purpose of this define is to be able to modify the texts without having to compile the entire project.
/// </summary>
#define DYNAMIC_READ_SOURCE // no more support for this not defined. 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Oktagon.Utils;
using Patterns;
using Source = Oktagon.Localization.LokakiJson;


namespace Oktagon.Localization
{


    public class LokakiID : System.Attribute
    {
        public string m_vId;

        public LokakiID(string vId)
        {
            m_vId = vId;
        }
    }

    public class Lokaki : SingletonClass<Lokaki>
    {
        const string PrefsLanguage = "Lokaki.CurrentLanguage";

        public enum LANGUAGES
        {
            None = 0,
            // Note: LokakiID is project specific!
            [LokakiID("Enum_English")]
            English = 1,
            [LokakiID("Enum_Chinese")]
            Chinese,
            [LokakiID("Enum_Japanese")]
            Japanese,
            [LokakiID("Enum_Korean")]
            Korean,
            [LokakiID("Enum_French")]
            French,
            [LokakiID("Enum_Italian")]
            Italian,
            [LokakiID("Enum_German")]
            German,
            [LokakiID("Enum_Russian")]
            Russian,
            [LokakiID("Enum_Spanish")]
            Spanish,
            [LokakiID("Enum_Portuguese")]
            Portuguese,
        }

        public ILokakiSource m_Source;
        public ILokakiSource Source
        {
            get
            {
                if(m_Source == null)
                {
                    m_Source = new Source();
                }
                return m_Source;
            }
        }

        public Lokaki()
        {
            Source.Load(CurrentLanguage);
        }

        public override string ToString()
        {
            return Source.ToString();
        }


        #region statics

        public static Dictionary<string, string > Data
        {
            get
            {
                return Instance.Source.Data;
            }
        }
        public static string GetText(string sID)
        {
            return Instance.Source.GetText(sID);
        }
        public static bool Contains(string sID)
        {
            return Instance.Source.Contains(sID);
        }

        public static Lokaki.LANGUAGES CurrentLanguage
        {
            get
            {
                string sLang = PlayerPrefs.GetString(PrefsLanguage, Lokaki.LANGUAGES.English.ToString());
                try
                {
                    Lokaki.LANGUAGES eValue = (Lokaki.LANGUAGES)System.Enum.Parse(typeof(Lokaki.LANGUAGES), sLang, true);
                    return eValue;
                }catch(System.Exception e) { }
                return Lokaki.LANGUAGES.English;
            }
            private set
            {
                PlayerPrefs.SetString(PrefsLanguage, value.ToString());
            }
        }
        
        public static void SetLanguage(Lokaki.LANGUAGES eLang)
        {
            Instance.Source.Load(eLang);
            CurrentLanguage = eLang;
        }
        public static void SetLanguage(Lokaki.LANGUAGES eLang, string sContent)
        {
            Instance.Source.Setup(eLang, sContent);
            CurrentLanguage = eLang;
        }

        #endregion


        /// <summary>
        /// Gets the name of a Enum to show in UI.
        /// e.g.: Language.Chinese may return " 中文 ".
        /// </summary>
        public static string GetEnumLokakiText(System.Enum eEnum)
        {
            string sValue = null;
            if (eEnum != null)
            {
                string iLokakiId = null;
                var type = eEnum.GetType();
                var memInfo = type.GetMember(eEnum.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(LokakiID), false);

                if (attributes.Length > 0)
                {
                    if (attributes[0] is LokakiID)
                    {
                        iLokakiId = ((LokakiID)attributes[0]).m_vId;
                    }
                }

                if (iLokakiId == null)
                {
                    sValue = eEnum.ToString();
                }
                else
                {
                    sValue = Lokaki.GetText(iLokakiId);
                }

            }
            else
            {
                sValue = "(null)";
            }

            return sValue;
        }


        #region editor
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Raincrow/Lokaki/Load json")]
        public static void Dump()
        {
            Debug.Log(Lokaki.Instance.ToString());
            //LokakiJson j = new LokakiJson();
            //j.Load(Lokaki.LANGUAGES.English);
            //Debug.Log(j.ToString());
        }
        [UnityEditor.MenuItem("Raincrow/Lokaki/Set English")]
        public static void SetEnglish()
        {
            Lokaki.SetLanguage(LANGUAGES.English);
            Dump();
        }
        [UnityEditor.MenuItem("Raincrow/Lokaki/Set Portuguese")]
        public static void SetPortuguese()
        {
            Lokaki.SetLanguage(LANGUAGES.Portuguese);
            Dump();
        }
        [UnityEditor.MenuItem("Raincrow/Lokaki/Set Japanese")]
        public static void SetJapanese()
        {
            Lokaki.SetLanguage(LANGUAGES.Japanese);
            Dump();
        }
#endif

        #endregion
    }
}