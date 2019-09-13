using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Raincrow.FTF
{
    [CustomEditor(typeof(FTFPointerHand))]
    public class FTFPointerHandInspector : Editor
    {
        private string m_PointerString = null;
        private FTFPointerHand m_PointerObject;
        [SerializeField] private bool m_Indented = false;
        [SerializeField] private Vector2 m_ScrollPosition;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);

            if (m_PointerObject == null)
            {
                m_PointerObject = target as FTFPointerHand;
                GenerateString();
            }

            if (m_PointerObject.GetComponent<RectTransform>().hasChanged)
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
                    EditorGUILayout.TextArea(m_PointerString, GUILayout.MinHeight(50));
                }
            }
        }

        public void GenerateString()
        {
            FTFPointData pointer = new FTFPointData();
            RectTransform rect = m_PointerObject.GetComponent<RectTransform>();

            pointer.show = true;
            pointer.anchorMin = rect.anchorMin;
            pointer.anchorMax = rect.anchorMax;
            pointer.position = rect.anchoredPosition;
            pointer.scale = rect.localScale.x;

            m_PointerString = "\"pointer\":" + Newtonsoft.Json.JsonConvert.SerializeObject(pointer, m_Indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }
    }
}
