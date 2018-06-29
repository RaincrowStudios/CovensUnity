using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIManager))]
public class UIManagerInspector : Editor
{

    //List<UIBase> m_UIList = null;
    /*List<UIBase> List
    {
        get
        {
            if(m_UIList == null || m_UIList.Count == 0)
            {
                m_UIList = new List<UIBase>(GameObject.FindObjectsOfType<UIBase>());
            }
            return m_UIList;
        }
    }*/


    List<UIBase> GetUIs()
    {
        List<UIBase> vUIList = new List<UIBase>();// GameObject.FindObjectsOfType<UIBase>());
        Canvas[] vCanvas = GameObject.FindObjectsOfType<Canvas>();
        foreach(var pCanvas in vCanvas)
        {
            MonoBehaviour[] vMono = pCanvas.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var pMono in vMono)
            {
                if(pMono is UIBase && !vUIList.Contains((UIBase)pMono))
                {
                    vUIList.Add((UIBase)pMono);
                }
            }
        }
        return vUIList;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        List<UIBase> vUIList = GetUIs();// new List<UIBase>(GameObject.FindObjectsOfType<UIBase>());
        if (vUIList == null)
            return;

        GUILayout.BeginVertical();
        GUILayout.Label("---- UI List:");

        GameObject pGORef = null;
        foreach (UIBase pUI in vUIList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            pGORef = pUI.gameObject;
            if (pUI.m_Target.activeInHierarchy)
                EditorGUILayout.LabelField(pUI.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
            else
                EditorGUILayout.LabelField(pUI.ToString(), GUILayout.MaxWidth(200), GUILayout.MinWidth(50));
            EditorGUILayout.ObjectField(""/*pUI.ToString()*/, pGORef, typeof(MonoBehaviour), true, GUILayout.MinWidth(100));

            if (GUILayout.Button("Focus", GUILayout.MaxWidth(80)))
            {
                FocusUI(pUI, vUIList);
            }
            if (GUILayout.Button("O", GUILayout.MaxWidth(30)))
            {
                pUI.m_Target.SetActive(true);
            }
            if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
            {
                pUI.m_Target.SetActive(false);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void FocusUI(UIBase pGO, List<UIBase> vUIList)
    {
        foreach (UIBase pUI in vUIList)
        {
            if (pUI is UIBase)
            {
                if (pUI.m_Target != pUI.gameObject)
                    pUI.gameObject.SetActive(true);
                pUI.m_Target.SetActive(pUI == pGO);
            }
        }
    }
}