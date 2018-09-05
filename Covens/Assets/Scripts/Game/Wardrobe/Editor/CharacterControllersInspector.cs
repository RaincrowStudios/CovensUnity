using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CharacterControllers))]
public class CharacterControllersInspector : Editor
{
    CharacterControllers m_pCurrent;
    protected void OnEnable()
    {
        m_pCurrent = (CharacterControllers)target;
        //if (!ItemDB.Instance.IsItemLoaded)
        ItemDB.Instance.LoadDB();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawEditorMode();
    }
    public bool m_bShow;
    private void DrawEditorMode()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(30);

        //GUILayout.Label("---- Filter:");
        //string sFind = GUILayout.TextField(m_sSearch);
        //if (sFind != m_sSearch)
        //{
        //    m_sSearch = sFind;
        //    SetLocalizationDict(sFind);
        //}

        GUILayout.Label("---- ItemDB:");
        m_bShow = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShow, "Show Equip Actions", true);

        if (m_bShow)
        {
            // set button label style
            GUIStyle pButtonLabel = new GUIStyle("button");
            pButtonLabel.alignment = TextAnchor.MiddleLeft;

            // draw buttons
            DrawEquip("Default", pButtonLabel);
            DrawEquip("Random", pButtonLabel);
            foreach (var pItem in ItemDB.Instance.GetAllItems(m_pCurrent.m_eGender))
            {
                DrawEquip(pItem.ID, pButtonLabel);
            }
        }

        GUILayout.EndVertical();
    }


    void DrawEquip(string sId, GUIStyle pButtonLabel)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(sId, pButtonLabel))
        {
            if(sId == "Default")
            {
                m_pCurrent.SetDefaultBody();
            }
            else if (sId == "Random")
            {
                List<WardrobeItemModel> vRandomItens = m_pCurrent.GetRandomItens(ItemDB.Instance.GetAllItems(m_pCurrent.m_eGender));
                m_pCurrent.Equip(vRandomItens);
            }
            else
            {
                m_pCurrent.Equip(sId);
            }
            m_pCurrent.GetComponent<CharacterView>().SetupChar();
        }
        //GUILayout.Label(pItem.Value);
        GUILayout.EndHorizontal();

    }
}
