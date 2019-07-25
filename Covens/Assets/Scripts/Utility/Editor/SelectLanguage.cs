using UnityEngine;
using UnityEditor;
using System.Collections;


public class SelectLanguage : EditorWindow
{   
    //private string dictionaryVersion = null;

    [MenuItem("Tools/Select Language")]
    public static void ShowWindow()
    {
        SelectLanguage t = GetWindow<SelectLanguage>();
        t.minSize = new Vector2(450, 38);
        t.maxSize = new Vector2(450, 38);
    }

    protected void OnGUI()
    {
        //if (dictionaryVersion == null)
        //{
        //    dictionaryVersion = PlayerPrefs.GetString(DictionaryManager.DictionaryVersionPlayerPrefsKey, string.Empty);
        //} 

        EditorGUI.BeginChangeCheck();

        //// Dictionary Version
        //using (new GUILayout.HorizontalScope())
        //{            
        //    EditorGUILayout.LabelField("Dictionary Version: ", GUILayout.MaxWidth(120));
        //    dictionaryVersion = EditorGUILayout.TextField(dictionaryVersion);
        //}

        // Language Index
        using (new GUILayout.HorizontalScope())
        {
            DictionaryManager.languageIndex = GUILayout.Toolbar(DictionaryManager.languageIndex, DictionaryManager.Languages);
        }            

        if (EditorGUI.EndChangeCheck())
        {
            //if (dictionaryVersion != null)
            //{
            //    PlayerPrefs.SetString(DictionaryManager.DictionaryVersionPlayerPrefsKey, dictionaryVersion);
            //}
            //if (languageIndex.HasValue)
            //{
            //    PlayerPrefs.SetInt(DictionaryManager.LanguageIndexPlayerPrefsKey, languageIndex.Value);
            //}

            //LocalizationManager.CallChangeLanguage(dictionaryVersion);
        }
    }
}
