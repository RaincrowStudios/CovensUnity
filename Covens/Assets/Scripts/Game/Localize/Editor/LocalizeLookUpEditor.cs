using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LocalizeLookUp))]
public class LocalizeLookUpEditor : Editor
{
    private LocalizeLookUp lookUpInstance;
    private string search = "";
    List<string> matchedIDs = new List<string>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawEditorMode();
    }

    void DrawEditorMode()
    {
        GUIStyle ButtonLabel = new GUIStyle("button");
        ButtonLabel.alignment = TextAnchor.MiddleLeft;

        GUILayout.BeginVertical();
        GUILayout.Space(15);

        //if (GUILayout.Button("Refresh Local IDs", ButtonLabel, GUILayout.MaxWidth(150)))
        //{
        //    LocalizationManager.RefreshIDS();
        //}

        GUILayout.Label("Filter: ");
        string filerID = GUILayout.TextField(search);
        GUILayout.Space(10);
        if (filerID != search)
        {
            search = filerID;
            SetMatchedList();
        }

        if (filerID == "" && matchedIDs.Count > 0)
        {
            Debug.Log("clearing");
            matchedIDs.Clear();
        }


        foreach (var item in matchedIDs)
        {
            if (GUILayout.Button(item, ButtonLabel, GUILayout.MaxWidth(150)))
            {
                lookUpInstance.id = item;
            }
        }
        GUILayout.EndVertical();
    }

    void SetMatchedList()
    {
        search = search.ToLower();
        matchedIDs.Clear();
        foreach (var item in LocalizationManager.localizeIDs)
        {
            if (item.Contains(search))
            {
                matchedIDs.Add(item);
            }
        }

    }

    void OnEnable()
    {
        lookUpInstance = (LocalizeLookUp)target;
    }
}
