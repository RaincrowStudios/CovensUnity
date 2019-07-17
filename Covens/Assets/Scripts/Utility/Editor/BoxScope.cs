using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoxScope : System.IDisposable
{
    private readonly string title;
    private readonly bool indent;
    private static GUIStyle boxScopeStyle;

    public static GUIStyle BoxScopeStyle
    {
        get
        {
            if (boxScopeStyle == null)
            {
                boxScopeStyle = new GUIStyle(EditorStyles.helpBox);
                RectOffset p = boxScopeStyle.padding;
                p.right += 6;
                p.top += 1;
                p.left += 3;
                p.bottom += 2;
            }

            return boxScopeStyle;
        }
    }

    public BoxScope(string title = "", bool indent = false)
    {
        this.indent = indent;
        EditorGUILayout.BeginVertical(BoxScopeStyle);
        if (indent) EditorGUI.indentLevel++;

        if (!string.IsNullOrWhiteSpace(title))
        {
            GUIStyle guiStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField(title, guiStyle, GUILayout.ExpandWidth(true));
        }
    }

    public void Dispose()
    {
        if (indent) EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}