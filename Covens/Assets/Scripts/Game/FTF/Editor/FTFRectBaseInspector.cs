using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Raincrow.FTF
{
    //[CustomEditor(typeof(FTFRectBase))]
    public abstract class FTFRectBaseInspector : Editor
    {
        private string m_HighlightString = null;
        private FTFRectBase m_RectObject;
        [SerializeField] private bool m_Indented = false;
        [SerializeField] private Vector2 m_ScrollPosition;

        public abstract string FieldName { get; }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(10);

            if (m_RectObject == null)
            {
                m_RectObject = target as FTFRectBase;
                GenerateString();
            }

            if (m_RectObject.GetComponent<RectTransform>().hasChanged)
                GenerateString();

            using (new BoxScope())
            {
                EditorGUI.BeginChangeCheck();

                m_Indented = EditorGUILayout.Toggle("Indented", m_Indented);

                if (EditorGUI.EndChangeCheck())
                    GenerateString();

                using (var scroll = new GUILayout.ScrollViewScope(m_ScrollPosition))
                {
                    m_ScrollPosition = scroll.scrollPosition;
                    EditorGUILayout.TextArea(m_HighlightString, GUILayout.MinHeight(50));
                }
            }
        }

        private void GenerateString()
        {
            FTFRectData area = new FTFRectData();
            RectTransform rect = m_RectObject.GetComponent<RectTransform>();

            area.show = true;
            area.anchorMin = rect.anchorMin;
            area.anchorMax = rect.anchorMax;
            area.position = rect.anchoredPosition;
            area.size = rect.sizeDelta;

            m_HighlightString = "\"" +FieldName+"\":" + Newtonsoft.Json.JsonConvert.SerializeObject(area, m_Indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }
    }
}