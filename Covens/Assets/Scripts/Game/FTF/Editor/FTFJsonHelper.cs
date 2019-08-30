using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Raincrow.Test;
using Newtonsoft.Json;

namespace Raincrow.FTF
{
    public class FTFJsonHelper : EditorWindow
    {
        private static FTFJsonHelper m_Window;

        [MenuItem("Tools/FTF Helper")]
        private static void Init()
        {
            m_Window = (FTFJsonHelper)GetWindow(typeof(FTFJsonHelper), false, "FTF Step Helper");
        }

        [SerializeField] private Vector2 m_MainScrollPosition = Vector2.zero;
        [SerializeField] private Vector2 m_JsonScrollPosition = Vector2.zero;
        [SerializeField] private string m_Json = "{}";
        [SerializeField] private FTFStepData m_Step;

        [SerializeField] private bool m_ExpandOnEnter = false;
        [SerializeField] private bool m_ExpandOnExit = false;
        [SerializeField] private string m_HighlightAreaJson = "\"highlight\":{}";
        [SerializeField] private string m_PointerJson = "\"pointer\":{}";

        [SerializeField] private bool m_Indented = false;

        private void OnGUI()
        {
            using (var scrollScope = new GUILayout.ScrollViewScope(m_MainScrollPosition, GUILayout.ExpandWidth(true)))
            {
                m_MainScrollPosition = scrollScope.scrollPosition;

                m_Step = StepField(m_Step);

                GUILayout.Space(10);

                using (new BoxScope())
                {
                    m_Indented = EditorGUILayout.Toggle("Indented", m_Indented);

                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Generate JSON"))
                        {
                            GenerateJson(m_Step);
                        }

                        if (GUILayout.Button("Read JSON"))
                        {
                            m_Step = JsonConvert.DeserializeObject<FTFStepData>(m_Json, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore });
                        }
                    }

                    using (var scroll = new EditorGUILayout.ScrollViewScope(m_JsonScrollPosition, GUILayout.MinHeight(100), GUILayout.MaxHeight(1000)))
                    {
                        m_JsonScrollPosition = scroll.scrollPosition;
                        m_Json = EditorGUILayout.TextArea(m_Json, GUILayout.ExpandHeight(true));
                    }
                }
            }
        }

        public FTFStepData StepField(FTFStepData step)
        {
            using (new BoxScope())
            {
                GUILayout.Space(2);

                using (new BoxScope())
                {
                    step.highlight.show = EditorGUILayout.Toggle("Highlight area", step.highlight.show);
                    if (step.highlight.show)
                    {
                        step.highlight = HighlightField(step.highlight);
                    }
                }
                GUILayout.Space(4);

                using (new BoxScope())
                {
                    step.pointer.show = EditorGUILayout.Toggle("Pointer", step.pointer.show);
                    if (step.pointer.show)
                    {
                        step.pointer = PointerField(step.pointer);
                    }
                }
                GUILayout.Space(4);

                using (new BoxScope())
                {
                    step.button.show = EditorGUILayout.Toggle("Button area", step.button.show);
                    if (step.button.show)
                    {
                        step.button = HighlightField(step.button);
                    }
                }
                GUILayout.Space(4);

                //on enter
                using (new BoxScope())
                {
                    m_ExpandOnEnter = DebugUtils.Foldout(m_ExpandOnEnter, "onEnter (" + step.onEnter?.Count + " actions)");
                    if (m_ExpandOnEnter)
                    {   
                        for (int i = 0; i < step.onEnter?.Count; i++)
                        {
                            Separator("[" + i + "]");
                            EditorGUI.indentLevel++;
                            step.onEnter[i] = ActionField(step.onEnter[i]);
                            EditorGUI.indentLevel--;
                        }

                        Separator();

                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Add"))
                            {
                                if (step.onEnter == null)
                                    step.onEnter = new List<FTFActionData>();
                                step.onEnter.Add(new FTFActionData());
                            }
                            if (GUILayout.Button("Remove last"))
                            {
                                if (step.onEnter != null)
                                {
                                    if (step.onEnter.Count > 0)
                                        step.onEnter.RemoveAt(step.onEnter.Count - 1);
                                    if (step.onEnter.Count == 0)
                                        step.onEnter = null;
                                }
                            }
                        }
                    }
                }
                GUILayout.Space(4);

                //onexit
                using (new BoxScope())
                {
                    m_ExpandOnExit = DebugUtils.Foldout(m_ExpandOnExit, "onExit (" + step.onExit?.Count + " actions)");
                    if (m_ExpandOnExit)
                    {
                        for (int i = 0; i < step.onExit?.Count; i++)
                        {
                            Separator("[" + i + "]");
                            EditorGUI.indentLevel++;
                            step.onExit[i] = ActionField(step.onExit[i]);
                            EditorGUI.indentLevel--;
                        }
                        Separator();
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Add"))
                            {
                                if (step.onExit == null)
                                    step.onExit = new List<FTFActionData>();
                                step.onExit.Add(new FTFActionData());
                            }
                            if (GUILayout.Button("Remove last"))
                            {
                                if (step.onExit != null)
                                {
                                    if (step.onExit.Count > 0)
                                        step.onExit.RemoveAt(step.onExit.Count - 1);
                                    if (step.onExit.Count == 0)
                                        step.onExit = null;
                                }
                            }
                        }
                    }
                    GUILayout.Space(2);

                    return step;
                }
            }
        }

        public FTFRectData HighlightField(FTFRectData area)
        {
            area.position = EditorGUILayout.Vector2Field("anchoredPosition", area.position);
            area.size = EditorGUILayout.Vector2Field("size", area.size);
            area.anchorMin = EditorGUILayout.Vector2Field("anchorMin", area.anchorMin);
            area.anchorMax = EditorGUILayout.Vector2Field("anchorMax", area.anchorMax);

            Separator();

            using (new GUILayout.HorizontalScope())
            {
                m_HighlightAreaJson = EditorGUILayout.TextArea(m_HighlightAreaJson);
                if (GUILayout.Button("Read", GUILayout.Width(100)))
                {
                    return JsonConvert.DeserializeObject<FTFRectData>(m_HighlightAreaJson.Replace("\"highlight\":", ""));
                }
            }

            return area;
        }

        public FTFPointData PointerField(FTFPointData pointer)
        {
            pointer.position = EditorGUILayout.Vector2Field("anchoredPosition", pointer.position);
            pointer.anchorMin = EditorGUILayout.Vector2Field("anchorMin", pointer.anchorMin);
            pointer.anchorMax = EditorGUILayout.Vector2Field("anchorMax", pointer.anchorMax);

            Separator();

            using (new GUILayout.HorizontalScope())
            {
                m_PointerJson = EditorGUILayout.TextArea(m_PointerJson);
                if (GUILayout.Button("Read", GUILayout.Width(100)))
                {
                    return JsonConvert.DeserializeObject<FTFPointData>(m_PointerJson.Replace("\"pointer\":", ""));
                }
            }

            return pointer;
        }

        private FTFActionData ActionField(FTFActionData action)
        {
            float labelWidth = 80;

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("method", GUILayout.Width(labelWidth));
                action.method = EditorGUILayout.TextField(action.method);
            }

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("params", GUILayout.Width(labelWidth));

                //GUILayout.FlexibleSpace();

                if (GUILayout.Button("+str", GUILayout.Width(45)))
                {
                    if (action.parameters == null)
                        action.parameters = new List<object>();
                    action.parameters.Add("string");
                }
                if (GUILayout.Button("+num", GUILayout.Width(45)))
                {
                    if (action.parameters == null)
                        action.parameters = new List<object>();
                    action.parameters.Add(0);
                }
                if (GUILayout.Button("X", GUILayout.Width(45)))
                {
                    if (action.parameters != null)
                    {
                        if (action.parameters.Count > 0)
                            action.parameters.RemoveAt(action.parameters.Count - 1);
                        if (action.parameters.Count == 0)
                            action.parameters = null;
                    }
                }
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < action.parameters?.Count; i++)
            {
                if (action.parameters[i] is float)
                    action.parameters[i] = EditorGUILayout.IntField("param " + i + ". ", (int)(float)action.parameters[i]);
                else if (action.parameters[i] is int)
                    action.parameters[i] = EditorGUILayout.IntField("param " + i + ". ", (int)action.parameters[i]);
                else if (action.parameters[i] is string)
                    action.parameters[i] = EditorGUILayout.TextField("param " + i + ". ", (string)action.parameters[i]);
            }
            EditorGUI.indentLevel--;

            return action;
        }

        public static void Separator(string label = "")
        {
            if (string.IsNullOrEmpty(label))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Space(5);
                    GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.Height(1));
                    GUILayout.Space(8);
                }
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent(label)).x)))
                {
                    //GUILayout.Space(5);
                    GUILayout.Label(label);
                    //GUILayout.Space(10);
                }

                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Space(EditorStyles.label.lineHeight/2);
                    GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.Height(1));
                    //GUILayout.Space(10);
                }
            }
        }

        public void GenerateJson(FTFStepData step)
        {
            if (step.highlight.show == false)
                step.highlight = new FTFRectData();

            if (step.onEnter?.Count == 0)
                step.onEnter = null;

            if (step.onExit?.Count == 0)
                step.onExit = null;

            if (step.pointer.show == false)
                step.pointer = new FTFPointData();

            if (step.button.show == false)
                step.button = new FTFRectData();
                        
            m_Json = JsonConvert.SerializeObject(step,
                                new JsonSerializerSettings()
                                {
                                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                                    NullValueHandling = NullValueHandling.Ignore,
                                    Formatting = m_Indented ? Formatting.Indented : Formatting.None
                                });
        }
    }
}