using UnityEngine;
using UnityEditor;
using System.Collections;


public class SelectLanguage : EditorWindow
{
    static int lng;

    string[] language = new string[] { "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };

    static int GetLanguage
    {
        get { return PlayerPrefs.GetInt("Language", 0); }
        set { PlayerPrefs.SetInt("Language", value); }
    }

    [MenuItem("Tools/Select Language")]
    public static void ShowWindow()
    {

        var t = EditorWindow.GetWindow(typeof(SelectLanguage));
        t.minSize = new Vector2(450, 19);
        t.maxSize = new Vector2(450, 19);
    }

    void OnGUI()
    {
        lng = GUILayout.Toolbar(GetLanguage, language);
        if (lng != GetLanguage)
        {
            GetLanguage = lng;
            LocalizationManager.CallChangeLanguage();
        }
    }
}
