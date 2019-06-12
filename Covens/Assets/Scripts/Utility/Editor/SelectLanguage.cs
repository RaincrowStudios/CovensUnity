using UnityEngine;
using UnityEditor;
using System.Collections;


public class SelectLanguage : EditorWindow
{   
    private string dictionaryVersion = string.Empty;
    private int languageIndex;

    [MenuItem("Tools/Select Language")]
    public static void ShowWindow()
    {
        SelectLanguage t = GetWindow<SelectLanguage>();
        t.minSize = new Vector2(450, 38);
        t.maxSize = new Vector2(450, 38);
        t.dictionaryVersion = PlayerPrefs.GetString(DictionaryManager.DictionaryVersionPlayerPrefsKey, string.Empty);
        t.languageIndex = PlayerPrefs.GetInt(DictionaryManager.LanguageIndexPlayerPrefsKey, 0);
    }

    void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Dictionary Version
        using (new GUILayout.HorizontalScope())
        {            
            EditorGUILayout.LabelField("Dictionary Version: ", GUILayout.MaxWidth(120));
            dictionaryVersion = EditorGUILayout.TextField(dictionaryVersion);
        }

        // Language Index
        using (new GUILayout.HorizontalScope())
        {
            languageIndex = GUILayout.Toolbar(languageIndex, DictionaryManager.Languages);
        }            

        if (EditorGUI.EndChangeCheck())
        {
            PlayerPrefs.SetString(DictionaryManager.DictionaryVersionPlayerPrefsKey, dictionaryVersion);
            PlayerPrefs.SetInt(DictionaryManager.LanguageIndexPlayerPrefsKey, languageIndex);

            LocalizationManager.CallChangeLanguage(dictionaryVersion);
        }
    }
}
