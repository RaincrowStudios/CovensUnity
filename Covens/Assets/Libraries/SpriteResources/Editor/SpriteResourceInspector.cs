using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

/// <summary>
/// sprite resources inspector
/// </summary>
[CustomEditor(typeof(SpriteResources))]
public class SpriteResourceInspector : Editor
{

    private ReorderableList m_pList;


    #region unity methods

    private void OnEnable()
    {
        m_pList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("SpriteResoruces"),
                true, true, true, true);

        if (m_pList == null)
        {
            Debug.LogError("noooo");
        }
        else {
            m_pList.drawHeaderCallback = DrawHeader;
            m_pList.drawElementCallback = DrawElementRect;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        m_pList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
    #endregion


    #region drawing

    private void DrawHeader(Rect pRect)
    {
        EditorGUI.LabelField(pRect, "Sprite Resources");
    }

    private void DrawElementRect(Rect pRect, int iIndex, bool bIsActive, bool bIsFocused)
    {
        SerializedProperty pElement = m_pList.serializedProperty.GetArrayElementAtIndex(iIndex);

        pRect.y += 2;
        float fWidth = pRect.width;
        float fNameWidth = fWidth * .5f;
        float fSourceWidth = fWidth * .35f;
        float fSourceImageWidth = EditorGUIUtility.singleLineHeight + 5;// fWidth * .15f;
        float fHeight = EditorGUIUtility.singleLineHeight + 5;

        // draw name
        EditorGUI.PropertyField(
            new Rect(pRect.x, pRect.y, fNameWidth, EditorGUIUtility.singleLineHeight),
            pElement.FindPropertyRelative("Name"), GUIContent.none);

        // draw source
        EditorGUI.PropertyField(
            new Rect(pRect.x + fNameWidth, pRect.y, fSourceWidth, EditorGUIUtility.singleLineHeight),
            pElement.FindPropertyRelative("Source"), GUIContent.none);

        

        // draw 
        try
        {
            Sprite pSprt = pElement.FindPropertyRelative("Source").objectReferenceValue as Sprite;
            Texture pText = pSprt.texture;
            EditorGUI.DrawPreviewTexture(
                new Rect(pRect.x + fNameWidth + fSourceWidth, pRect.y, fSourceImageWidth, fSourceImageWidth),
                pText);
        }catch(System.Exception e) { }
        //EditorGUI.PropertyField(
        //    new Rect(pRect.x + fNameWidth, pRect.y, fSourceImageWidth, EditorGUIUtility.singleLineHeight),
        //    pElement.FindPropertyRelative("Source"), GUIContent.none);
    }

    #endregion

}