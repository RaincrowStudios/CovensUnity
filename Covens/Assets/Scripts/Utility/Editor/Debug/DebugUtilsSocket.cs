using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using Newtonsoft.Json;
using System;

[System.Serializable]
public class DebugUtilsSocket
{
    const string RESOURCE_PATH = "LocalApi/Socket";

    private (string, TextAsset)[] m_AvailableEvents = null;
    private string[] m_TempEvents;

    [SerializeField] private string m_FilterString = "";
    [SerializeField] private string m_CustomEventName = "custom.event";
    [SerializeField] private string m_CustomEventMessage = "{\n\t\n}";
    [SerializeField] private Vector2 m_CustomEventScroll;
    [SerializeField] private Vector2 m_MainScroll;

    public DebugUtilsSocket()
    {
        //EditorApplication.projectChanged -= LoadAvailableEvents;
        //EditorApplication.projectChanged += LoadAvailableEvents;   
    }

    ~DebugUtilsSocket()
    {
        //EditorApplication.projectChanged -= LoadAvailableEvents;
    }

    public void Draw()
    {
        if (GUILayout.Button("Reload resources"))
            LoadAvailableEvents();

        DrawCustomEvent();

        if (m_AvailableEvents == null)
            LoadAvailableEvents();

        using (new BoxScope("Resources/" + RESOURCE_PATH))
        {
            //filter
            using (new BoxScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Filter", GUILayout.Width(40));
                    m_FilterString = EditorGUILayout.TextField(m_FilterString);
                }
            }
            //socket event list
            using (var scroll = new EditorGUILayout.ScrollViewScope(m_MainScroll))
            {
                m_MainScroll = scroll.scrollPosition;
                for (int i = 0; i < m_AvailableEvents.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(m_FilterString) || m_AvailableEvents[i].Item1.Contains(m_FilterString))
                        DrawSocketEvent(i);
                }
            }
        }
    }

    public void LoadAvailableEvents()
    {
        if (m_AvailableEvents != null)
        {
            foreach (var entry in m_AvailableEvents)
                Resources.UnloadAsset(entry.Item2);
        }

        TextAsset[] availableAssets = Resources.LoadAll<TextAsset>(RESOURCE_PATH);
        m_TempEvents = new string[availableAssets.Length];

        List<(string, TextAsset)> aux = new List<(string, TextAsset)>();
        for (int i = 0; i < availableAssets.Length; i++)
        {
            TextAsset item = availableAssets[i];

            string eventname = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(item));
            m_TempEvents[i] = item.text;
            aux.Add((eventname, item));
        }

        m_AvailableEvents = aux.ToArray();
    }

    public void Save(int idx)
    {
        //get the full path
        string path = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(m_AvailableEvents[idx].Item2);

        //unload the asset so it can be overwritten
        Resources.UnloadAsset(m_AvailableEvents[idx].Item2);

        try
        {
            //make sure its indented
            string content = JsonConvert.SerializeObject((JsonConvert.DeserializeObject<Dictionary<string, object>>(m_TempEvents[idx])), Formatting.Indented);
            //overwrite it
            m_TempEvents[idx] = content;
            File.WriteAllText(path, content);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }

        //reload it
        m_AvailableEvents[idx].Item2 = Resources.Load<TextAsset>(RESOURCE_PATH + "/" + m_AvailableEvents[idx].Item1);
    }

    public void Reload(int idx)
    {
        m_TempEvents[idx] = m_AvailableEvents[idx].Item2.text;
    }

    private bool GetFoldout(string eventName)
    {
        return EditorPrefs.GetBool("DebugSocket." + eventName, false);
    }

    private void SetFoldout(string eventName, bool value)
    {
        EditorPrefs.SetBool("DebugSocket." + eventName, value);
    }

    private void DrawSocketEvent(int idx)
    {
        using (new BoxScope())
        {
            string eventName = m_AvailableEvents[idx].Item1;
            bool previousValue = GetFoldout(eventName);
            bool show = EditorGUILayout.Foldout(previousValue, eventName, true);

            if (show != previousValue)
                SetFoldout(eventName, show);

            if (show)
            {
                m_TempEvents[idx] = GUILayout.TextArea(m_TempEvents[idx]);

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Save"))
                        Save(idx);
                    if (GUILayout.Button("Reload"))
                        Reload(idx);
                }

                using (new EditorGUI.DisabledGroupScope(SocketClient.Instance == null))
                {
                    if (GUILayout.Button("Trigger event"))
                    {
                        TriggerEvent(eventName, m_TempEvents[idx]);
                    }
                }
            }
        }
    }

    private void DrawCustomEvent()
    {
        using (new EditorGUI.DisabledGroupScope(!Application.isPlaying || SocketClient.Instance == null || !SocketClient.Instance.IsConnected()))
        {
            using (new BoxScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Name:", GUILayout.Width(40));
                    m_CustomEventName = EditorGUILayout.TextField(m_CustomEventName);
                }

                using (new GUILayout.HorizontalScope())
                {
                    //GUILayout.Label("Content:", GUILayout.Width(40));
                    using (var scroll = new EditorGUILayout.ScrollViewScope(m_CustomEventScroll, GUILayout.Height(100)))
                    {
                        m_CustomEventScroll = scroll.scrollPosition;
                        m_CustomEventMessage = EditorGUILayout.TextArea(m_CustomEventMessage, GUILayout.ExpandHeight(true));
                    }
                }

                if (GUILayout.Button("Trigger custom event"))
                {
                    TriggerEvent(m_CustomEventName, m_CustomEventMessage);
                }
            }
        }
    }

    private void TriggerEvent(string command, string data)
    {
        while(data.Contains("<1minfromnow>"))
        {
            data.Replace("<1minfromnow>", GetUnixTimestamp(DateTime.Now.Add(new TimeSpan(0, 1, 0))).ToString());
        }
        CommandResponse response = new CommandResponse()
        {
            Command = command,
            Data = data
        };
        SocketClient.Instance.ManageData(response);
    }

    private static double GetUnixTimestamp(DateTime dateTime)
    {
        return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
