using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//todo: make it a simple class and load it from the server as done with the DictMatrixData content
public class KytelerData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    //public string iconId;
    //public string artId;

    public Sprite icon;
    public Sprite art;
}

#if UNITY_EDITOR
public class KytelerEditorExtension
{
    [UnityEditor.MenuItem("Assets/Create/Kyteler Data")]
    public static void CreateScriptable()
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        string fileName = "NewRing";
        string extra = "";

        if (path == "")
        {
            path = "Assets/";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
        }else
        {
            path += "/";
        }

        int aux = 1;
        while (UnityEditor.AssetDatabase.LoadAssetAtPath<KytelerData>(path + fileName + extra + ".asset") != null)
        {
            extra = " (" + aux + ")";
            aux += 1;
        }

        KytelerData asset = ScriptableObject.CreateInstance<KytelerData>();
        UnityEditor.AssetDatabase.CreateAsset(asset, path + fileName + extra + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();

        UnityEditor.EditorUtility.FocusProjectWindow();

        UnityEditor.Selection.activeObject = asset;
    }
}
#endif


