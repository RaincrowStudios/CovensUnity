using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Oktagon.Localization

{
    [CustomEditor(typeof(LocalizedText))]
    public class LocalizedTextInspector : Editor
    {
        private LocalizedText m_pCurrent;
        private static string m_sSearch = "";
        private static Dictionary<string, string> m_pLocalizationDict;

        public Dictionary<string, string> LocalizationDict
        {
            get
            {
                if (m_pLocalizationDict == null)
                {
                    SetLocalizationDict(m_sSearch);
                }
                return m_pLocalizationDict;
            }
        }

        public void SetLocalizationDict(string sFilter)
        {
            if (string.IsNullOrEmpty(sFilter))
            {
                m_pLocalizationDict = Lokaki.Data;
                return;
            }
            sFilter = sFilter.ToLower();
            m_pLocalizationDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> values in Lokaki.Data)
            {

                if (
                    values.Key.ToLower().Contains(sFilter) ||
                    values.Value.ToLower().Contains(sFilter)
                    )
                {
                    m_pLocalizationDict.Add(values.Key, values.Value);
                }
            }
        }

        protected void OnEnable()
        {
            m_pCurrent = (LocalizedText)target;
            UnityEngine.UI.Text pText = m_pCurrent.GetComponent<UnityEngine.UI.Text>();
            if(pText != null)
            {
                m_sSearch = pText.text;
                SetLocalizationDict(m_sSearch);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawEditorMode();
        }

        private void DrawEditorMode()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(30);

            GUILayout.Label("---- Filter:");
            string sFind = GUILayout.TextField(m_sSearch);
            if (sFind != m_sSearch)
            {
                m_sSearch = sFind;
                SetLocalizationDict(sFind);
            }

            GUILayout.Label("---- Localization:");

            // set button label style
            GUIStyle pButtonLabel = new GUIStyle("button");
            pButtonLabel.alignment = TextAnchor.MiddleLeft;

            // draw all localizations
            foreach (KeyValuePair<string, string> values in LocalizationDict)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(values.Key.ToString(), pButtonLabel, GUILayout.MaxWidth(150)))
                {
                    m_pCurrent.SetKey(values.Key);
                }
                GUILayout.Label(values.Value);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }


    }

}