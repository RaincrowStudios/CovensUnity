using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class WebsocketSenderWindow : EditorWindow
{

    #region attrs

	private bool m_bShowCommands = true;
    private bool m_bShowCommands1 = true;
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
    
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal(EditorStyles.miniButtonMid);
		if (GUILayout.Button ("Copy InstanceID")) {
			TextEditor te = new TextEditor();
			te.content = new GUIContent(PlayerDataManager.playerData.instance);
			te.SelectAll();
			te.Copy();
		}

		if (GUILayout.Button ("Copy TargetID")) {
			Debug.Log (MarkerSpawner.instanceID);
			TextEditor te = new TextEditor();
			te.content = new GUIContent( MarkerSpawner.instanceID);
			te.SelectAll();
			te.Copy();
		}
		var style = new GUIStyle(GUI.skin.button);
		style.normal.textColor = Color.yellow;
		if (GUILayout.Button("Send",style))
		{
			Client.ParseJson(LastCommnand);
		}
		EditorGUILayout.EndHorizontal();

        // send pre-made commnads
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        m_bShowCommands = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShowCommands, "Spell Carousel Commands", true);
        EditorGUILayout.EndHorizontal();
        if (m_bShowCommands)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//            for (int i = 0; i < m_sCommandList.Length; i++)
//            {
//                if (GUILayout.Button("Send " + m_sCommandList[i]))
//                {
//                    string sCommand = APIManagerLocal.SendCommand(m_sCommandList[i]);
//                    LastCommnand = sCommand;
//                }
//            }

		
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

		// send pre-made commnads
//		EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
//		m_bShowCommands1 = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShowCommands1, "Condition Commands", true);
//		EditorGUILayout.EndHorizontal();
//		if (m_bShowCommands1)
//		{
//			
//			EditorGUILayout.EndVertical();
//			EditorGUILayout.EndHorizontal();
//		}
    }

}