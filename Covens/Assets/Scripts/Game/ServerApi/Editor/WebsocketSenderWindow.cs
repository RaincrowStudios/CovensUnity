using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class WebsocketSenderWindow : EditorWindow
{

    #region attrs

    private bool m_bShowCommands = true;
    WebSocketClient m_pClient;
    WebSocketClient Client
    {
        get
        {
            if (m_pClient == null)
            {
                m_pClient = GameObject.FindObjectOfType<WebSocketClient>();
            }
            return m_pClient;
        }
    }
    public const string LastCommandKey = "WebsocketSenderWindow.LastCommnand";
    public string LastCommnand
    {
        get
        {
            return EditorPrefs.GetString(LastCommandKey, "");
        }
        set
        {
            EditorPrefs.SetString(LastCommandKey, value);
        }
    }
    public string[] m_sCommandList = new string[]
    {
        "coven_member_ally",
        "coven_member_disband",
        "coven_member_join",
        "coven_member_kick",
        "coven_member_leave",
        "coven_member_promote",
        "coven_member_request",
        "coven_member_unally",
        "coven_request_invite",
        "coven_title_change",
        "coven_was_allied",
        "coven_was_unallied",
        "map_portal_add"
    };
    #endregion



    [MenuItem("Raincrow/Tools/Websocket Sender")]
    static void Init()
    {
        WebsocketSenderWindow window;
        window = EditorWindow.CreateInstance(typeof(WebsocketSenderWindow)) as WebsocketSenderWindow;
        window.titleContent = new GUIContent("Websocket Sender Window");
        //window.titleContent = window.titleContent = new GUIContent("Net Monitor", pIcon);
        window.Show();
    }


    protected void OnGUI()
    {
        // command layoult
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Command");
        LastCommnand = EditorGUILayout.TextArea(LastCommnand);
        if (GUILayout.Button("Send"))
        {
            Client.ParseJson(LastCommnand);
        }
        EditorGUILayout.EndHorizontal();



        GUILayout.Space(10);



        // send pre-made commnads
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        m_bShowCommands = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShowCommands, "Show Premade commands", true);
        EditorGUILayout.EndHorizontal();
        if (m_bShowCommands)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            for (int i = 0; i < m_sCommandList.Length; i++)
            {
                if (GUILayout.Button("Send " + m_sCommandList[i]))
                {
                    string sCommand = APIManagerLocal.SendCommand(m_sCommandList[i]);
                    LastCommnand = sCommand;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

}