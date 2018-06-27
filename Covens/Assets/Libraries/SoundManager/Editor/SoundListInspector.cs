#if UNITY_EDITOR

using UnityEngine;
using System;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;


[CustomEditor(typeof(SoundList))]
public class SoundListInspector : Editor
{
    private ReorderableList m_pSoundList;
    private SoundList m_Myself;

    private void OnEnable()
    {
        m_Myself = (SoundList)this.target;
        m_pSoundList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_SoundName"), true, true, true, true);
        m_pSoundList.drawElementCallback = DrawElement;
        m_pSoundList.onSelectCallback = OnSelectItem;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        m_pSoundList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        DropAreaGUI();

    }

    private void OnSelectItem(ReorderableList list)
    {
        StopAllClips();
        var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
        string sName = element.FindPropertyRelative("m_Name").stringValue;
        AudioClip pSfx = m_Myself.FindAudio(sName);
        if (pSfx)
            PlayClip(pSfx);
    }
    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = m_pSoundList.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;
        float fNameSize = 120;
        float fPlaySize =30 ;
        EditorGUI.PropertyField(new Rect(rect.x, rect.y, fNameSize, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_Name"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(rect.x + fNameSize, rect.y, rect.width - fNameSize - fPlaySize, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_AudioClip"), GUIContent.none);
        //EditorGUI.PropertyField(new Rect(rect.width - fPlaySize, rect.y, fPlaySize, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("m_AudioClip"), GUIContent.none);
        if (GUI.Button(new Rect(rect.width +5, rect.y, fPlaySize, EditorGUIUtility.singleLineHeight), ">"))
        {
            StopAllClips();
            //myScript.BuildObject();
            //PlayClip((AudioClip) );
            string sName = element.FindPropertyRelative("m_Name").stringValue;
            AudioClip pSfx = m_Myself.FindAudio(sName);
            if (pSfx)
                PlayClip(pSfx);
        }
    }


    public void DropAreaGUI()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Add Sound to DB");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                    {
                        m_Myself.AddSound((AudioClip)dragged_object);
                        // Do On Drag Stuff here
                    }
                }
                break;
        }
    }


    public static void PlayClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null, new System.Type[] { typeof(AudioClip) }, null
        );
        method.Invoke(
            null,
            new object[] { clip }
        );
    }

    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            BindingFlags.Static | BindingFlags.Public,
            null, new System.Type[] { }, null
        );
        method.Invoke(
            null,
            new object[] { }
        );
    }
}

#endif