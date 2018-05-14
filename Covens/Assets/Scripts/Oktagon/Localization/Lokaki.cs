
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

namespace Oktagon.Localization
{
    public interface ILokakiText
    {
        void UpdateText();

        void RemoveInstance();
        // removes this instance from the lokaki text list.
    }


    public class LokakiID : System.Attribute
    {
        public int[] m_vId;

        public LokakiID(params int[] vId)
        {
            m_vId = vId;
        }
    }

    public class Lokaki
    {

        public enum LANGUAGES
        {
            // Note: LokakiID is project specific!
            [LokakiID(510)]
            English = 0,
            [LokakiID(511)]
            Chinese,
            [LokakiID(512)]
            Japanese,
            [LokakiID(513)]
            Korean,
            [LokakiID(514)]
            French,
            [LokakiID(515)]
            Italian,
            [LokakiID(516)]
            German,
            [LokakiID(517)]
            Russian,
            [LokakiID(518)]
            Spanish,
            [LokakiID(519)]
            Portuguese,
        }



        private const string ASSET_PATH = "GameSettings/Lokakit/Lokakit";

        private const string SEPARATOR_NEW_LANGUAGE = ";_Language=";
        private const string SEPARATOR_NEW_ID = ";_ID=";
        private const string SEPARATOR_NEW_TEXT = ";_Text=";

        private static double m_fLatestRefreshTime;
        // only to not refresh every second

        private static string m_sLanguageDataBase;
        // the language databse. the file we get all language id's. When null, tries to get from Resources.

        // all SpriteText and GUI that uses Lokaki texts...
        private static List<ILokakiText> m_lTextLabels = new List<ILokakiText>();
        private static TextData[] m_vCurrentText = GetLanguageTexts(LANGUAGES.English);
        protected static LANGUAGES m_eCurrentLanguage = LANGUAGES.English;
        protected static LANGUAGES m_eUsingLanguage;
        // actually using language. May differ from m_eCurrentLanguage if no support is found for the given language

        protected static LANGUAGES[] m_vIgnoredLanguage = new LANGUAGES[0];
        // language we are not going to give support to (even if has)

        protected static readonly List<DynamicTag> m_lDynamicTags = new List<DynamicTag>();


        public static void AddText(ILokakiText pTextLabel)
        {
            m_lTextLabels.Add(pTextLabel);
        }

        public static void RemoveText(ILokakiText pTextLabel)
        {
            m_lTextLabels.Remove(pTextLabel);
        }

        /// <summary>
        /// Gets a text from lokaki. returns a "id: invalid" if id is not in the array
        /// </summary>
        /// <param name="id">index of lokaki. starts from 0</param>
        /// <param name="bRawText">When true, will get the raw text value, without parsing dynamic tags</param>
        public static string GetText(int id, bool bRawText)
        {
            if (m_vCurrentText == null)
            {
                Debug.LogWarning("texts are null!!!");
                return "error, no language set.";
            }
            if (id < 0 || id >= m_vCurrentText.Length)
            {
                return id + ": invalid";
            }
            else
            {
                string sText;

                TextData pTextData = m_vCurrentText[id];
                if (pTextData == null)
                {
                    sText = id + ": not defined";
                }
                else
                {
                    sText = pTextData.GetValue();
                    if (bRawText)
                    {
                    }
                    else
                    {
                        // first time retrieving this text?
                        if (!pTextData.HasCheckedDynamicTag())
                        {
                            // yes, then check if has tags:
                            bool bContains = Lokaki.ContainsDynamicTags(pTextData.GetValue());
                            pTextData.SetContainsDynamicTag(bContains);
                        }

                        if (pTextData.HasDynamicTag())
                        {
                            sText = ReplaceDynamicTags(sText);
                        }

                    }
                }
                return sText;
            }
        }

        public static string GetText(int id)
        {
            return GetText(id, false);
        }

        /// <summary>
        /// Returns true if contains the text id
        /// </summary>
        public static bool HasText(int id)
        {

            if (m_vCurrentText == null)
            {
                return false;
            }
            else
            {
                if (id < 0 || id >= m_vCurrentText.Length)
                {
                    return false;
                }
                else
                {
                    TextData pTextData = m_vCurrentText[id];
                    if (pTextData == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (!pTextData.HasText())
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the total amount of language IDs we have.
        /// </summary>
        public static int GetTextCount()
        {
            if (m_vCurrentText != null)
            {
                return m_vCurrentText.Length;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the array of texts based on a langage.
        /// Don't call every frame, it creates a new array when it is called
        /// </summary>
        public static TextData[] GetLanguageTexts(LANGUAGES eLanguage)
        {

            TextData[] vOutput = null;

            if (IsLanguageIgnored(eLanguage))
            {
                eLanguage = LANGUAGES.English;
            }


            string sFileText = GetDefaultDatabase();

            //		TextAsset pTextFile = (TextAsset) Resources.Load(ASSET_PATH, typeof(TextAsset));

            if (sFileText != null)
            {

                // find the language we want. The same of Split(SEPARATOR_NEW_LANGUAGE) and check the first line of each. But faster.
                vOutput = GetLanguageTexts(eLanguage, sFileText);
            }

            if (vOutput == null)
            {
                if (eLanguage != LANGUAGES.English)
                {
                    // gets english by default.
                    vOutput = GetLanguageTexts(LANGUAGES.English);
                }
                else
                {
                    Debug.LogWarning("English language does not exists! creating a fake Lokaki with 256 indices");
                    vOutput = new TextData[256];

                    for (int i = 0; i < vOutput.Length; i++)
                    {
                        vOutput[i] = new TextData(i, "unknown" + i);
                    }

                }
            }
            else
            {
                m_eUsingLanguage = eLanguage;
            }


            return vOutput;
        }

        /// <summary>
        /// Gets the language texts as string array, instead of TextData.
        /// Created for use by editor windows.
        /// </summary>
        public static string[] GetLanguageTextsAsString(LANGUAGES eLanguage)
        {
            TextData[] vTextData = GetLanguageTexts(eLanguage);
            string[] vText = new string[vTextData.Length];
            for (int i = 0; i < vText.Length; i++)
            {
                vText[i] = vTextData[i].GetValue();
            }
            return vText;
        }

        /// <summary>
        /// returns true if the given language is in the text;
        /// if sFileText is null, checks the Asset resource
        /// </summary>
        public static bool HasLanguage(LANGUAGES eLanguage, string sFileText = null)
        {

            if (sFileText == null)
            {
                sFileText = GetDefaultDatabase();
            }

            if (string.IsNullOrEmpty(sFileText))
            {
                return false;
            }
            else
            {
                return sFileText.Contains(SEPARATOR_NEW_LANGUAGE + eLanguage.ToString());
            }


        }


        /// <summary>
        /// Sets the language database to load.
        /// set sDatabase null to load from resources.
        /// When bIgnoreRefresh is true, will not update the database. however, if a new language is set, will update.
        /// </summary>
        public static void SetLanguageDatabase(string sDatabase, bool bIgnoreRefresh = false)
        {

            m_sLanguageDataBase = sDatabase;

            // update texts?
            if (!bIgnoreRefresh)
            {
                // yes.
                m_vCurrentText = GetLanguageTexts(GetCurrentLanguage());
                UpdateLabels();

            }


        }

        /// <summary>
        /// Gets the default languges items database.
        /// if m_sLanguageDataBase is not set, returns the Assets file.
        /// </summary>
        public static string GetDefaultDatabase()
        {

            string sFileText = null;

            if (m_sLanguageDataBase == null)
            {
                TextAsset pTextFile = (TextAsset)Resources.Load(ASSET_PATH, typeof(TextAsset));

                if (pTextFile != null)
                {
                    sFileText = pTextFile.text;
                }
            }

            if (sFileText == null)
            {
                sFileText = m_sLanguageDataBase;
            }

            return sFileText;

        }


        /// <summary>
        /// Returns the array of texts based on a langage.
        /// Don't call every frame, it creates a new array when it is called
        /// </summary>
        /// <returns>
        /// The language texts of eLanguage.
        /// </returns>
        /// <param name='eLanguage'>
        /// A language to look for
        /// </param>
        /// <param name='sFileText'>
        /// The string database to find the texts.
        /// </param>
        public static TextData[] GetLanguageTexts(LANGUAGES eLanguage, string sFileText)
        {
            TextData[] vOutput = null;

            int iStartIndex = 0;
            int iFinalIndex = -1;
            string sValues = null;

            int _antiLoop = 99999;
            while (_antiLoop-- > 9 && sValues == null)
            {
                iStartIndex = sFileText.IndexOf(SEPARATOR_NEW_LANGUAGE, iStartIndex);
                if (iStartIndex == -1)
                {
                    break; // ?
                }
                iStartIndex = iStartIndex + SEPARATOR_NEW_LANGUAGE.Length;
                iFinalIndex = sFileText.IndexOf('\n', iStartIndex);
                string sLanguage = sFileText.Substring(iStartIndex, iFinalIndex - iStartIndex);
                sLanguage = sLanguage.TrimEnd('\r');
                // found language?
                if (sLanguage == eLanguage.ToString())
                {
                    // yes, now lets get all text value..
                    iStartIndex = iFinalIndex + 1; // +1 is the '\n'
                    iFinalIndex = sFileText.IndexOf(SEPARATOR_NEW_LANGUAGE, iFinalIndex);


                    if (iFinalIndex == -1)
                    {
                        // get the rest of text file
                        sValues = sFileText.Substring(iStartIndex);
                    }
                    else
                    {
                        // get until the next language
                        sValues = sFileText.Substring(iStartIndex, iFinalIndex - iStartIndex);
                    }

                }
            }

            if (sValues != null)
            {
                //			Debug.Log("Language found!\n" + sValues);

                string[] vTexts = System.Text.RegularExpressions.Regex.Split(sValues, SEPARATOR_NEW_ID);
                int iTextCount = 0;
                { // get text count based on last text index. allows jumping text indices in file.
                    string sLastText = vTexts[vTexts.Length - 1];
                    iStartIndex = sLastText.IndexOf(SEPARATOR_NEW_TEXT);
                    string sId = sLastText.Substring(0, iStartIndex);
                    if (int.TryParse(sId, out iTextCount))
                    {

                    }
                    else
                    {
                        iTextCount = vTexts.Length;
                    }
                }

                vOutput = new TextData[iTextCount + 1];
                for (int i = 1; i < vTexts.Length; i++)
                { // i dunno why the first is empty...
                    string sText = vTexts[i];
                    iStartIndex = sText.IndexOf(SEPARATOR_NEW_TEXT);
                    string sId = sText.Substring(0, iStartIndex);

                    int iId;
                    if (int.TryParse(sId, out iId))
                    {

                        if (iId < 0 || iId >= vOutput.Length)
                        {
                            Debug.LogWarning("Don't have index " + iId + "!");
                        }
                        else
                        {
                            // ok, everything ok, now just get the value.

                            iStartIndex += SEPARATOR_NEW_TEXT.Length;
                            iFinalIndex = sText.LastIndexOf('\n'); // becuz the generator always add an breakline
                            string sValue;
                            if (iFinalIndex == -1)
                            {
                                sValue = sText.Substring(iStartIndex);
                            }
                            else
                            {
                                if (iFinalIndex > 1 && sText[iFinalIndex - 1] == '\r')
                                {
                                    iFinalIndex--;  // fixes: bug that appending lines in run time would bug how textsprites displays each character;
                                }
                                sValue = sText.Substring(iStartIndex, iFinalIndex - iStartIndex);
                            }
                            TextData pTextData = new TextData(iId, sValue);
                            pTextData.m_bLoaded = true;
                            vOutput[iId] = pTextData;

                        }

                    }
                    else
                    {
                        Debug.LogWarning("Error when converting \"" + sId + "\" to integer");
                    }


                }

            }

            return vOutput;
        }



        /// <summary>
        /// Set the current language and updates all texts (only if changed the language type);
        /// </summary>
        /// <param name="eNewLanguage">
        /// A <see cref="LANGUAGES"/>
        /// </param>
        public static void SetCurrentLanguage(LANGUAGES eNewLanguage)
        {
            // language changed?
            if (eNewLanguage == m_eCurrentLanguage)
            {
                // no, then do nothing..
                return;
            }
            m_eCurrentLanguage = eNewLanguage;
            m_vCurrentText = GetLanguageTexts(eNewLanguage);

            UpdateLabels();
        }

        /// <summary>
        /// Updates ALL labels.
        /// If you want to change language, use SetCurrentLanguage instead.
        /// </summary>
        public static void UpdateLabels()
        {

            ILokakiText pText;
            // update labels
            for (int i = 0; i < m_lTextLabels.Count; i++)
            {
                pText = m_lTextLabels[i];
                if (pText != null)
                {
                    try
                    {
                        pText.UpdateText();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Gets the system's language as Lokaki's LANGUAGE.
        /// </summary>
        public static LANGUAGES GetSystemLanguage()
        {
            string sSystemLanguage = Application.systemLanguage.ToString();
#if UNITY_ANDROID
            if (sSystemLanguage.Equals("Unknown"))
            {
                // android bug..
                AndroidJavaObject locale = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault");
                string sLanguage = locale.Call<string>("getISO3Language");
                System.Array vEnums = System.Enum.GetValues(typeof(Lokaki.LANGUAGES));
                for (int i = 0; i < vEnums.Length; i++)
                {
                    if (vEnums.GetValue(i).ToString().ToLower().Contains(sLanguage.ToLower()))
                    {
                        sSystemLanguage = vEnums.GetValue(i).ToString();
                        break;
                    }
                }
            }
#endif
            if (sSystemLanguage.Equals("Unknown"))
            {
                sSystemLanguage = Lokaki.LANGUAGES.English.ToString();
            }


            if (System.Enum.IsDefined(typeof(Lokaki.LANGUAGES), sSystemLanguage))
            {
                LANGUAGES eLanguage = (Lokaki.LANGUAGES)System.Enum.Parse(typeof(Lokaki.LANGUAGES), sSystemLanguage);

                if (IsLanguageIgnored(eLanguage))
                {
                    eLanguage = LANGUAGES.English;
                }

                return eLanguage;
            }
            else
            {
                return Lokaki.LANGUAGES.English;
            }

        }

        /// <summary>
        /// Adds a language to ignore.
        /// This removes permanently a Language from game.
        /// </summary>
        public static void AddIgnoreLanguage(params LANGUAGES[] vLanguage)
        {

            if (vLanguage == null || vLanguage.Length == 0)
            {
                return;
            }

            for (int i = 0; i < vLanguage.Length; i++)
            {
                //				bool bIsIgnored = false;
                LANGUAGES eLanguage = vLanguage[i];

                if (!IsLanguageIgnored(eLanguage))
                {
                    m_vIgnoredLanguage = OktArrayUtility.Merge(m_vIgnoredLanguage, new LANGUAGES[] { eLanguage });
                    if (GetUsingLanguage() == eLanguage)
                    {
                        // update language if got ignored
                        SetCurrentLanguage(GetSystemLanguage());
                    }
                }
            }

        }


        /// <summary>
        /// Returns true if the language has been banned from game
        /// </summary>
        public static bool IsLanguageIgnored(LANGUAGES eLanguage)
        {
            if (m_vIgnoredLanguage == null)
            {
                return false;
            }
            for (int i = 0; i < m_vIgnoredLanguage.Length; i++)
            {
                if (m_vIgnoredLanguage[i] == eLanguage)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the ignored language set, and returns to current set language
        /// </summary>
        public static void ClearIgnoredLanguage()
        {
            m_vIgnoredLanguage = new LANGUAGES[0];
            SetCurrentLanguage(GetCurrentLanguage());
        }

        /// <summary>
        /// Gets the currently language.
        /// Even if the set language is not supported, will return it.
        /// </summary>
        public static LANGUAGES GetCurrentLanguage()
        {
            return m_eCurrentLanguage;
        }

        /// <summary>
        /// Gets the using language.
        /// This is the actually loaded texts content's Language.
        /// May return English if the language set is not supported.
        /// </summary>
        public static LANGUAGES GetUsingLanguage()
        {
            return m_eUsingLanguage;
        }


        /// <summary>
        /// Refresh the value of current texts.
        /// </summary>
        public static void Refresh()
        {

            double fCurTime = System.TimeSpan.FromTicks(System.DateTime.Now.Ticks).TotalMilliseconds;

            if ((m_fLatestRefreshTime - fCurTime) < 0.2f)
            {
                m_vCurrentText = GetLanguageTexts(m_eCurrentLanguage);
                m_fLatestRefreshTime = System.TimeSpan.FromTicks(System.DateTime.Now.Ticks).TotalMilliseconds;
            }

        }

        public static void dump()
        {

        }




        /// <summary>
        /// Gets the name of a Enum to show in UI.
        /// e.g.: Language.Chinese may return " 中文 ".
        /// </summary>
        public static string GetEnumLokakiText(System.Enum eEnum, int iIndex)
        {
            string sValue = null;
            if (eEnum != null)
            {
                int iLokakiId = -1;
                var type = eEnum.GetType();
                var memInfo = type.GetMember(eEnum.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(LokakiID), false);

                if (attributes.Length > 0)
                {
                    if (attributes[0] is LokakiID)
                    {
                        int[] vId = ((LokakiID)attributes[0]).m_vId;
                        if (vId != null)
                        {
                            if (iIndex >= 0 && iIndex < vId.Length)
                            {
                                iLokakiId = vId[iIndex];
                            }
                        }
                    }
                }

                if (iLokakiId == -1)
                {
#if UNITY_EDITOR
                    Debug.LogError("Attempt to access localized iIndex [" + iIndex + "], but \"" + (eEnum.GetType().Name + "." + eEnum.ToString()) + "\" is not defining it's LokakiID!");
#endif
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

        public static string GetEnumLokakiText(System.Enum eEnum)
        {
            return GetEnumLokakiText(eEnum, 0);
        }

        #region Language Supports




        /// <summary>
        /// Gets the supported languages in the current database.
        /// Useful to only show in the UI the supported languages.
        /// </summary>
        public static LANGUAGES[] GetSupportedLanguages()
        {

            LANGUAGES[] vLanguages = System.Enum.GetValues(typeof(LANGUAGES)) as LANGUAGES[];

            var lLanguages = new System.Collections.Generic.List<LANGUAGES>();

            for (int i = 0; i < vLanguages.Length; i++)
            {
                LANGUAGES eLanguage = vLanguages[i];

                if (IsLanguageIgnored(eLanguage))
                {

                }
                else
                {
                    if (HasLanguage(eLanguage))
                    {
                        lLanguages.Add(eLanguage);
                    }
                }

            }

            return lLanguages.ToArray();

            //			return new LANGUAGES[0];
        }

        #endregion



        #region Dynamic Tag

        /// <summary>
        /// Removes all dynamic tags.
        /// </summary>
        public static void ClearDynamicTags()
        {
            m_lDynamicTags.Clear();
        }

        /// <summary>
        /// Adds a dynamic tag by instance reference.
        /// </summary>
        public static void AddDynamicTag(DynamicTag pDynamicTag)
        {
            m_lDynamicTags.Add(pDynamicTag);
        }

        /// <summary>
        /// Includes a new tag to replace dynamically.
        /// e.g.: AddDynamicTag("<dPlayerName>", delegate() {return "Foo";});
        /// the example above will replace all <dPlayerName> occurrences by "Foo".
        /// </summary>
        public static DynamicTag AddDynamicTag(string sReplacement, System.Func<string> pFuntion)
        {
            DynamicTag pDynamicTag = new DynamicTag(sReplacement, sReplacement, pFuntion);
            AddDynamicTag(pDynamicTag);
            return pDynamicTag;
        }

        /// <summary>
        /// Includes a new tag to replace dynamically with a parameter.
        /// Use =N to use N as string parameter in your delegate.
        /// e.g.: AddDynamicTag("<dClan=N>", delegate(string sName) {return sParam + "Foo";});
        /// the example above will replace all <dClan=N> occurrences by N + "Foo".
        /// e.g.: "Clan <dClan=A> vs <dClan=B>" = "Clan AFoo vs BFoo>"
        /// </summary>
        public static DynamicTag AddDynamicTag(string sReplacement, System.Func<string, string> pFuntion)
        {
            DynamicTag pDynamicTag = new DynamicTag(sReplacement, sReplacement, pFuntion);
            AddDynamicTag(pDynamicTag);
            return pDynamicTag;
        }

        /// <summary>
        /// Removes a dynamic tag replacement from the lokaki
        /// </summary>
        public static bool RemoveDynamicTag(string sName)
        {
            int iIndex = m_lDynamicTags.FindIndex(delegate (DynamicTag pItem)
            {
                return pItem.m_sName == sName;
            });
            bool bRemoved;
            if (iIndex == -1)
            {
                bRemoved = false;
            }
            else
            {
                bRemoved = true;
                m_lDynamicTags.RemoveAt(iIndex);
            }

            return bRemoved;
        }

        /// <summary>
        /// Gets the dynamic tag instance based on it's name.
        /// Returns null if not found.
        /// </summary>
        public static DynamicTag GetDynamicTag(string sName)
        {
            DynamicTag pDynamicTag = m_lDynamicTags.Find(delegate (DynamicTag pItem)
            {
                return pItem.m_sName == sName;
            });
            return pDynamicTag;
        }

        /// <summary>
        /// Replaces the dynamic tags on the given sText and returns the new text.
        /// </summary>
        public static string ReplaceDynamicTags(string sText)
        {

            for (int i = 0; i < m_lDynamicTags.Count; i++)
            {
                sText = m_lDynamicTags[i].Replace(sText);
            }
            return sText;
        }

        /// <summary>
        /// Returns true if the given sText has any of the Dynamic tags setup.
        /// Used for smart replace.
        /// </summary>
        public static bool ContainsDynamicTags(string sText)
        {
            bool bContains = false;
            for (int i = 0; i < m_lDynamicTags.Count; i++)
            {
                //				if (sText.Contains(m_lDynamicTags [i].m_sReplacement)) {
                if (m_lDynamicTags[i].Matches(sText))
                {
                    bContains = true;
                    break;
                }
            }
            return bContains;
        }

        #endregion




    }
}