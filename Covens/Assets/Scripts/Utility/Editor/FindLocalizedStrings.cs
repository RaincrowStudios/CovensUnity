using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindLocalizedStrings : EditorWindow
{
    [MenuItem("Raincrow/Lokaki/Find localized prefabs")]
    private static void Open()
    {
        FindLocalizedStrings window = (FindLocalizedStrings)EditorWindow.GetWindow(typeof(FindLocalizedStrings));
        window.Show();
        window.position = new Rect(20, 80, 550, 500);
    }

    //[SerializeField] private bool m_Toggle
    private string m_Results;
    private Vector2 m_ResultScroll;


    private void OnGUI()
    {
        using (new BoxScope())
        {
            if (GUILayout.Button("Find on scripts"))
            {

            }

            if (GUILayout.Button("Find on prefabs"))
            {
                FindOnPrefabs();
            }
        }

        using (new BoxScope())
        {
            GUILayout.Label($"Result:");
            using (var scroll = new EditorGUILayout.ScrollViewScope(m_ResultScroll, GUILayout.ExpandHeight(true)))
            {
                m_ResultScroll = scroll.scrollPosition;
                m_Results = EditorGUILayout.TextArea(m_Results, GUILayout.ExpandHeight(true));
            }
        }
    }

    private void FindOnPrefabs()
    {
        var type = typeof(LocalizeLookUp);
        var assetsGUIDs = AssetDatabase.FindAssets("t:Prefab");
        List<string> assetPaths = new List<string>();
        HashSet<string> results = new HashSet<string>();

        foreach (var guid in assetsGUIDs)
            assetPaths.Add(AssetDatabase.GUIDToAssetPath(guid));
        
        foreach (var ass in assetPaths)
        {

        }
    }
}
