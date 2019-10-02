using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ListBundleAssets
{
    [MenuItem("Assets/List bundle content", false, 30)]
    private static void PrintContents()
    {
        if (Selection.activeObject == null)
        {
            Debug.LogError("select an asset bundle file first");
            return;
        }

        AssetBundle bundle = AssetBundle.LoadFromFile(Application.dataPath + AssetDatabase.GetAssetPath(Selection.activeObject).Remove(0, 6));

        if (bundle != null)
        {
            SerializedObject so = new SerializedObject(bundle);
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            //str.Append("PreloadTable\n");
            //foreach (SerializedProperty d in so.FindProperty("m_PreloadTable"))
            //{
            //    if (d.objectReferenceValue != null)
            //        str.Append("\t" + d.objectReferenceValue.name + "\t" + d.objectReferenceValue.GetType().ToString() + "\n");
            //}

            str.Append(bundle.name + "\nContainer:\n");
            foreach (SerializedProperty d in so.FindProperty("m_Container"))
                str.Append("\t" + d.displayName + "\n");

            Debug.Log(str.ToString());
            bundle.Unload(false);
        }
    }
}
