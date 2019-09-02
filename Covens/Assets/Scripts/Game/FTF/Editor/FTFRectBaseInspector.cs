using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Raincrow.FTF
{
    //[CustomEditor(typeof(FTFRectBase))]
    public abstract class FTFRectBaseInspector : Editor
    {
        private FTFRectBase m_RectObject;
        [SerializeField] protected static string m_JsonString = "";
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
            }
            
            using (new BoxScope())
            {
                m_Indented = EditorGUILayout.Toggle("Indented", m_Indented);

                using (var scroll = new GUILayout.ScrollViewScope(m_ScrollPosition))
                {
                    m_ScrollPosition = scroll.scrollPosition;
                    m_JsonString = EditorGUILayout.TextArea(m_JsonString, GUILayout.MinHeight(50));
                }

                if (GUILayout.Button("Serialize"))
                    m_JsonString = GenerateString();

                OnDrawGenerateBox();
            }
        }

        protected string GenerateString()
        {
            FTFRectData area = new FTFRectData();
            RectTransform rect = m_RectObject.RectTransform;

            area.show = true;
            area.anchorMin = rect.anchorMin;
            area.anchorMax = rect.anchorMax;
            area.position = rect.anchoredPosition;
            area.size = rect.sizeDelta;

            return "\"" +FieldName+"\":" + Newtonsoft.Json.JsonConvert.SerializeObject(area, m_Indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }

        protected virtual void OnDrawGenerateBox()
        {

        }
    }
}