using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Raincrow.FTF
{
    [CustomEditor(typeof(FTFHighlight))]
    public class FTFHighlightInspector : FTFRectBaseInspector
    {
        public override string FieldName => "highlight";
        
        protected override void OnDrawGenerateBox()
        {
            bool disable = Application.isPlaying == false;

            EditorGUI.BeginDisabledGroup(disable);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show"))
                {
                    FTFRectData data = JsonConvert.DeserializeObject<FTFRectData>(m_JsonString.Replace("\"" + FieldName + "\":", ""));
                    (target as FTFHighlight).Show(data);
                }
                if (GUILayout.Button("Hide"))
                {
                    (target as FTFHighlight).Hide();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}