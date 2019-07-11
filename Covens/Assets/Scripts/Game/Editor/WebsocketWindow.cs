using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class WebsocketWindow : EditorWindow
{
	SocketClient m_pClient;
	SocketClient Client
	{
		get
		{
			if (m_pClient == null)
			{
				m_pClient = GameObject.FindObjectOfType<SocketClient>();
			}
			return m_pClient;
		}
	}
	public List<WSData> messages = new List<WSData>();

	[MenuItem("Raincrow/Tools/Websocket Monitor %#W")]
	static void Init()
	{
		WebsocketWindow window;
		window = EditorWindow.CreateInstance(typeof(WebsocketWindow)) as WebsocketWindow;
		window.titleContent = new GUIContent("Websocket Monitor");
		window.Show();
	}

	protected void OnGUI()
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Received Commands");
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
		bool bSortByIdx = EditorGUILayout.ToggleLeft("Sort by index", true);
		bool bShowCallStack = EditorGUILayout.ToggleLeft("Show Call Stack", false);
		bool bShowRequest = EditorGUILayout.ToggleLeft("Show Requests", false);
		bool bShowResponse = EditorGUILayout.Toggle("Show Responses", false);
		bool bShowKey = EditorGUILayout.ToggleLeft("Show Key", false);
		EditorGUILayout.EndHorizontal();
	}
}

