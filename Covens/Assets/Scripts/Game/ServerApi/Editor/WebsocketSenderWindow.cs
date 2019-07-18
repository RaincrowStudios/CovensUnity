using UnityEngine;
using UnityEditor;

public class WebsocketSenderWindow : EditorWindow
{

    #region attrs

	private bool m_bShowCommands = true;
    private bool m_bShowCommands1 = true;
    SocketClient m_pClient;
    SocketClient Client
    {
        get
        {
            if (m_pClient == null)
            {
                m_pClient = FindObjectOfType<SocketClient>();
            }
            return m_pClient;
        }
    }

    private const string LastCommandResponseKey = "WebsocketSenderWindow.LastCommandResponseKey";
    private const string LastCommandResponseDataKey = "WebsocketSenderWindow.LastCommandDataResponseKey";

    public CommandResponse LastCommandResponse
    {
        get
        {            
            CommandResponse response = new CommandResponse()
            {
                Command = EditorPrefs.GetString(LastCommandResponseKey, string.Empty),
                Data = EditorPrefs.GetString(LastCommandResponseDataKey, string.Empty),
            };
            return response;
        }
        set
        {

            EditorPrefs.SetString(LastCommandResponseKey, value.Command);
            EditorPrefs.SetString(LastCommandResponseDataKey, value.Data);
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
        CommandResponse response = LastCommandResponse;

        EditorGUI.BeginChangeCheck();

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Command Response");
            response.Command = EditorGUILayout.TextField(response.Command);

            EditorGUILayout.LabelField("Command Response Data");
            response.Data = EditorGUILayout.TextArea(response.Data);
        }        

        if (EditorGUI.EndChangeCheck())
        {
            LastCommandResponse = response;
        }

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

		if (GUILayout.Button ("Send Command"))
        {
            SocketClient.Instance.AddMessage(LastCommandResponse);
		}

		var style = new GUIStyle(GUI.skin.button);
		style.normal.textColor = Color.yellow;
//		if (GUILayout.Button("Send",style))
//		{
//			Client.ParseJson(LastCommnand);  
//		}
		EditorGUILayout.EndHorizontal();

        // send pre-made commnads
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        m_bShowCommands = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShowCommands, "Player Stats", true);
        EditorGUILayout.EndHorizontal();
		if (m_bShowCommands&& LoginAPIManager.characterLoggedIn && Application.isPlaying) 
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			var pData = PlayerDataManager.playerData; 
			EditorGUILayout.LabelField("PlayerName : " + pData.name );
			EditorGUILayout.LabelField("Coordinate : " + MapsAPI.Instance.position.y + " , " + MapsAPI.Instance.position.x);
			EditorGUILayout.LabelField("Instance : " + pData.instance);
			EditorGUILayout.LabelField("State : " + pData.state );
			EditorGUILayout.LabelField("Energy : " + pData.energy);
			EditorGUILayout.LabelField("XP : " + pData.xp );
			EditorGUILayout.LabelField("Level : " + pData.level );
			//if(pData.coven!="")
			//EditorGUILayout.LabelField("Coven : " + pData.coven);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // send pre-made commnads
        //EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        //m_bShowCommands1 = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), m_bShowCommands1, "Selected Token Stats", true);
        //EditorGUILayout.EndHorizontal();
        //if (m_bShowCommands1 && LoginAPIManager.loggedIn)
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Space(15);
        //    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //    var mData = MarkerSpawner.SelectedMarker;
        //    if (MapSelection.currentView == CurrentView.IsoView)
        //    {
        //        EditorGUILayout.LabelField("PlayerName : " + mData.displayName);
        //        EditorGUILayout.LabelField("Type : " + mData.type);
        //        EditorGUILayout.LabelField("Instance : " + mData.instance);
        //        EditorGUILayout.LabelField("State : " + mData.state);
        //        EditorGUILayout.LabelField("Energy : " + mData.energy);
        //        EditorGUILayout.LabelField("XP : " + mData.xp);
        //        EditorGUILayout.LabelField("Level : " + mData.level);
        //        EditorGUILayout.LabelField("Conditions : ");
        //        foreach (var item in MarkerSpawner.SelectedMarker.conditions)
        //        {
        //            EditorGUILayout.LabelField(item.id);
        //        }
        //    }
        //    else
        //    {
        //        EditorGUILayout.LabelField("Select a token on Map . . .");
        //    }
        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.EndHorizontal();
        //}
    }

}