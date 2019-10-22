using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LocalizationManager : MonoBehaviour
{
    public static Dictionary<string, string> LocalizeDictionary = new Dictionary<string, string>();
    public static HashSet<string> localizeIDs = new HashSet<string>();
    public delegate void ChangeLanguage();
    public static ChangeLanguage OnChangeLanguage;

    class LocalizationData
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    public static void CallChangeLanguage(string version, bool updateDictionary = true)
    {
        Debug.Log("LanguageChanged");
        if (updateDictionary)
        {
            DictionaryManager.GetLocalisationDictionary(version, 
                onDicionaryReady: () =>
                {
                    OnChangeLanguage?.Invoke();
                },
                onDownloadError: (code, response) => 
                {
                    Debug.LogError($"Failed to download new dictionary:\n[{code}] {response}");
                },
                onParseError: () =>
                {
                    Debug.LogError("Failed to parse dictionary");
                });
        }
        else
        {
            OnChangeLanguage?.Invoke();
        }
    }
}


