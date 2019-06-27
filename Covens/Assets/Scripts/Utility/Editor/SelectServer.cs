using UnityEngine;
using UnityEditor;

public class SelectServer : EditorWindow

{
    // string myString = "Hello World";
    // bool groupEnabled;
    // bool myBool = true;
    // float myFloat = 1.23f;
    // bool posGroupEnabled = true;
    // bool[] pos = new bool[3] { true, true, true };
    // Add menu item named "My Window" to the Window menu
    static int gameServerTab;
    static int wsServerTab;
    static int mapServerTab;
    static int chatServerTab;
    static int chatServerTabHTTP;


    string[] serverType = new string[] { "Release", "Staging", "Local", "Gustavo" };

    [MenuItem("Tools/Server Config")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(SelectServer));
    }
    // public static void UpdateUI()
    // {
    //     gameServerTab = getType("game");
    //     wsServerTab = getType("ws");
    //     mapServerTab = getType("map");
    //     chatServerTab = getType("chat");
    // }
    static int getType(string s)
    {
        string p = EditorPrefs.GetString(s);
        if (p == "Release") return 0;
        else if (p == "Staging") return 1;
        else if (p == "Gustavo") return 3;
        else return 2;
    }
    void OnGUI()
    {
        GUILayout.Label("Game Server", EditorStyles.boldLabel);
        gameServerTab = GUILayout.Toolbar(getType("game"), serverType);
        switch (gameServerTab)
        {
            case 0:
                EditorPrefs.SetString("game", "Release");
                break;
            case 1:
                EditorPrefs.SetString("game", "Staging");
                break;
            case 2:
                EditorPrefs.SetString("game", "Local");
                break;
            case 3:
                EditorPrefs.SetString("game", "Gustavo");
                break;
        }
        GUILayout.Label(CovenConstants.hostAddress);


        GUILayout.Label("WS Server", EditorStyles.boldLabel);
        wsServerTab = GUILayout.Toolbar(getType("ws"), serverType);
        switch (wsServerTab)
        {
            case 0:
                EditorPrefs.SetString("ws", "Release");
                break;
            case 1:
                EditorPrefs.SetString("ws", "Staging");
                break;
            case 2:
                EditorPrefs.SetString("ws", "Local");
                break;
            case 3:
                EditorPrefs.SetString("ws", "Gustavo");
                break;
        }
        GUILayout.Label(CovenConstants.wssAddress);

        GUILayout.Label("Map Server", EditorStyles.boldLabel);
        mapServerTab = GUILayout.Toolbar(getType("map"), serverType);
        switch (mapServerTab)
        {
            case 0:
                EditorPrefs.SetString("map", "Release");
                break;
            case 1:
                EditorPrefs.SetString("map", "Staging");
                break;
            case 2:
                EditorPrefs.SetString("map", "Local");
                break;
            case 3:
                EditorPrefs.SetString("map", "Gustavo");
                break;
        }
        GUILayout.Label(CovenConstants.wsMapServer);

        GUILayout.Label("Chat Server", EditorStyles.boldLabel);
        chatServerTab = GUILayout.Toolbar(getType("chat"), serverType);
        switch (chatServerTab)
        {
            case 0:
                EditorPrefs.SetString("chat", "Release");
                break;
            case 1:
                EditorPrefs.SetString("chat", "Staging");
                break;
            case 2:
                EditorPrefs.SetString("chat", "Local");
                break;
            case 3:
                EditorPrefs.SetString("chat", "Gustavo");
                break;
        }
        GUILayout.Label(CovenConstants.chatAddress);

        // GUILayout.Label("Chat Server", EditorStyles.boldLabel);
        // chatServerTab = GUILayout.Toolbar(chatServerTab, serverType);



        // groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        // myBool = EditorGUILayout.Toggle("Toggle", myBool);
        // myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup();
    }
}