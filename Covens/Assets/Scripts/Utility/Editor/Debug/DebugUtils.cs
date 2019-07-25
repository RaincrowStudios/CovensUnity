using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Raincrow.Chat.UI;

namespace Raincrow.Test
{
    public partial class DebugUtils : EditorWindow
    {
        [MenuItem("Tools/DebugUtils")]
        static void Init()
        {
            DebugUtils window = (DebugUtils)GetWindow(typeof(DebugUtils), false, "Debug Utils");
        }

        private int m_CurrentTab = 0;
        private string[] m_TabOptions = new string[] { "Users", "Others", "Chat", "Coven" };
        private Vector2 m_ScrollPosition = Vector2.zero;
        private Vector3 m_Vector3;
        private float m_Float1;
        private float m_Float2;
        private float m_Float3;
        private string m_SpellId = "spell_hex";

        private void OnGUI()
        {
            m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_TabOptions);

            using (var scrollScope = new GUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollScope.scrollPosition;

                switch (m_CurrentTab)
                {
                    case 0:
                        Users();
                        break;
                    case 1:
                        Others();
                        break;
                    case 2:
                        Chat();
                        break;
                    case 3:
                        // Check DebugUtilsCovenManagement file
                        //ShowCovenDebug();
                        break;
                }
            }
        }

        //player debug
        private GUILayoutOption m_LabelWidth = GUILayout.Width(75);

        private class StoredUser
        {
            public string Username;
            public string Password;
            public string Comment;
            bool Visible;
        }

        private StoredUser[] m_StoredUsers;
        private StoredUser[] StoredUsers
        {
            get
            {
                if (m_StoredUsers == null)
                {
                    string json = EditorPrefs.GetString("DebugUtils.StoredUsers", "[]");
                    m_StoredUsers = JsonConvert.DeserializeObject<StoredUser[]>(json);
                }
                return m_StoredUsers;
            }
            set
            {
                m_StoredUsers = value;
                EditorPrefs.SetString("DebugUtils.StoredUsers", JsonConvert.SerializeObject(StoredUsers));
            }
        }

        public bool ExpandTokens
        {
            get { return EditorPrefs.GetBool("DebugUtils.ExpandTokens", false); }
            set { EditorPrefs.SetBool("DebugUtils.ExpandTokens", value); }
        }

        public bool ExpandCurrentUser
        {
            get { return EditorPrefs.GetBool("DebugUtils.ExpandCurrentUser", false); }
            set { EditorPrefs.SetBool("DebugUtils.ExpandCurrentUser", value); }
        }

        public bool ExpandStoredUsers
        {
            get { return EditorPrefs.GetBool("DebugUtils.ExpandStoredUsers", false); }
            set { EditorPrefs.SetBool("DebugUtils.ExpandStoredUsers", value); }
        }

        private void Users()
        {
            ExpandTokens = Foldout(ExpandTokens, "Tokens");
            if (ExpandTokens)
            {
                using (new BoxScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Game:", m_LabelWidth);
                        LoginAPIManager.loginToken = EditorGUILayout.TextField(LoginAPIManager.loginToken);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Socket:", m_LabelWidth);
                        LoginAPIManager.wssToken = EditorGUILayout.TextField(LoginAPIManager.wssToken);
                    }
                }
            }

            ExpandCurrentUser = Foldout(ExpandCurrentUser, "Current User");
            if (ExpandCurrentUser)
            {
                using (new BoxScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Username:", m_LabelWidth);
                        LoginAPIManager.StoredUserName = EditorGUILayout.TextField(LoginAPIManager.StoredUserName);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Password:", m_LabelWidth);
                        LoginAPIManager.StoredUserPassword = EditorGUILayout.TextField(LoginAPIManager.StoredUserPassword);
                    }
                }
            }

            ExpandStoredUsers = Foldout(ExpandStoredUsers, "Stored Users");
            if (ExpandStoredUsers)
            {
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < StoredUsers.Length; i++)
                {
                    if (i < StoredUsers.Length)
                    {
                        DrawStoredUser(StoredUsers[i]);
                        GUILayout.Space(5);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    StoredUsers = m_StoredUsers;
                }

                using (new BoxScope())
                {
                    if (GUILayout.Button("Add"))
                    {
                        StoredUser newUser = new StoredUser();
                        List<StoredUser> aux = new List<StoredUser>(StoredUsers);
                        aux.Add(newUser);
                        StoredUsers = aux.ToArray();
                    }
                }
            }
        }

        private void DrawStoredUser(StoredUser user)
        {
            Color previousColor = GUI.backgroundColor;

            //if (user.Username == LoginAPIManager.StoredUserName && user.Password == LoginAPIManager.StoredUserPassword)
            //{
            //    GUI.backgroundColor = Color.green;
            //}

            using (new BoxScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Username:", m_LabelWidth);
                    user.Username = EditorGUILayout.TextField(user.Username);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Password:", m_LabelWidth);
                    user.Password = EditorGUILayout.TextField(user.Password);
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (string.IsNullOrEmpty(user.Comment))
                        user.Comment = "commentary";
                    user.Comment = EditorGUILayout.TextField(user.Comment, new GUIStyle("Label"));
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Set current"))
                    {
                        LoginAPIManager.StoredUserName = user.Username;
                        LoginAPIManager.StoredUserPassword = user.Password;
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        List<StoredUser> aux = new List<StoredUser>(StoredUsers);
                        aux.Remove(user);
                        StoredUsers = aux.ToArray();
                    }
                }
            }

            GUI.backgroundColor = previousColor;
        }


        private string m_sCommandResponse = "{}";
        private string m_sCommandResponseData = "{}";
        private string m_sItemData = "{}";
        private double m_JavascriptDate = 0;
        private float m_Longitude;
        private float m_Latitude;

        private Vector2 m_ChatPlayerScroll = Vector2.zero;
        private Raincrow.Chat.ChatPlayer m_ChatDebugPlayer = null;
        private Raincrow.Chat.ChatMessage m_ChatDebugMessage;
        public Raincrow.SceneManager.Scene m_StartScene = Raincrow.SceneManager.Scene.GAME;

        private void Others()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            using (new BoxScope())
            {
                CentralizedLabel("GPS");
                bool interactable = Application.isPlaying && GetGPS.instance != null;
                EditorGUI.BeginDisabledGroup(!interactable);

                using (new GUILayout.HorizontalScope())
                {
                    if (interactable)
                    {
                        m_Longitude = EditorGUILayout.FloatField("LON", GetGPS.longitude);
                        m_Latitude = EditorGUILayout.FloatField("LAT", GetGPS.latitude);

                        if (m_Longitude != GetGPS.longitude)
                            GetGPS.longitude = m_Longitude;
                        if (m_Latitude != GetGPS.latitude)
                            GetGPS.latitude = m_Latitude;
                    }
                    else
                    {
                        m_Longitude = EditorGUILayout.FloatField("LON", 0);
                        m_Latitude = EditorGUILayout.FloatField("LAT", 0);
                    }
                }
                if (GUILayout.Button("flyto phys"))
                {
                    MarkerManagerAPI.GetMarkers(m_Longitude, m_Latitude, null, true, true, true);
                }
                if (GUILayout.Button("flyto random"))
                {
                    MarkerManagerAPI.GetMarkers(Random.Range(-170, 170), Random.Range(-75, 75), null, true, true, true);
                }
                EditorGUI.EndDisabledGroup();
            }

            using (new BoxScope())
            {
                CentralizedLabel("Editor");

                Raincrow.SceneManager.Scene scene = (Raincrow.SceneManager.Scene)EditorGUILayout.EnumPopup("start scene", m_StartScene);
                if (scene != m_StartScene)
                {
                    m_StartScene = scene;
                    PlayerPrefs.SetInt("DEBUGSCENE", (int)m_StartScene);
                }

                bool debugLocation = true;
#if DEBUG_LOCATION == false
                debugLocation = false;
#endif

                string sDebugLocationLabel = "DebugLocation[" + (debugLocation ? "ON" : "OFF") + "]";
                if (EditorApplication.isCompiling)
                {
                    sDebugLocationLabel = "compiling";
                    for (int i = 0; i <= ((int)EditorApplication.timeSinceStartup) % 3; i++)
                        sDebugLocationLabel += ".";
                }

                if (GUILayout.Button(sDebugLocationLabel))
                {
                    string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                    List<string> allDefines = new List<string>(definesString.Split(';'));

                    if (debugLocation)
                        allDefines.Remove("DEBUG_LOCATION");
                    else
                        allDefines.Add("DEBUG_LOCATION");

                    definesString = string.Join(";", allDefines.ToArray());
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definesString);
                }

                if (GUILayout.Button("persistentDataPath"))
                {
                    EditorUtility.RevealInFinder(Application.persistentDataPath);
                }
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);

            DrawRequestDebug();

            using (new EditorGUI.DisabledGroupScope(SocketClient.Instance == null || !SocketClient.Instance.IsConnected()))
            {
                using (new BoxScope())
                {
                    CentralizedLabel("Socket");

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Command Reponse:", GUILayout.Width(40));
                        m_sCommandResponse = EditorGUILayout.TextField(m_sCommandResponse);
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Command Reponse Data:", GUILayout.Width(40));
                        m_sCommandResponseData = EditorGUILayout.TextField(m_sCommandResponseData);
                    }

                    if (GUILayout.Button("Send Fake Command Response"))
                    {
                        //WSData data = JsonConvert.DeserializeObject<WSData>(m_sCommandResponse);
                        //data.timestamp = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
                        CommandResponse response = new CommandResponse()
                        {
                            Command = m_sCommandResponse,
                            Data = m_sCommandResponseData
                        };
                        SocketClient.Instance.ManageData(response);
                    }
                }

                GUILayout.Space(10);

                using (new BoxScope())
                {
                    //CentralizedLabel("Items");

                    //using (new GUILayout.HorizontalScope())
                    //{
                    //    GUILayout.Label("data:", GUILayout.Width(40));
                    //    m_sItemData = EditorGUILayout.TextField(m_sItemData);
                    //}

                    GUILayout.Space(5);

                    if (GUILayout.Button("Add cosmetic"))
                    {
                        CosmeticData data = JsonConvert.DeserializeObject<CosmeticData>(m_sItemData);
                        PlayerDataManager.playerData.inventory.cosmetics.Add(data);
                    }
                }
            }

            using (new BoxScope())
            {
                CentralizedLabel("DateTime");

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("java timestamp:", GUILayout.Width(40));
                    m_JavascriptDate = EditorGUILayout.DoubleField(m_JavascriptDate);
                }

                GUILayout.Space(5);

                if (GUILayout.Button("To DateTime"))
                {
                    Debug.Log(m_JavascriptDate + " > " + Utilities.FromJavaTime(m_JavascriptDate));
                }

                if (GUILayout.Button("To TimeSpan"))
                {
                    System.TimeSpan timeSpan = Utilities.TimespanFromJavaTime(m_JavascriptDate);

                    string debugString = "javatime > " + m_JavascriptDate + "\n";
                    debugString += timeSpan.Days + "days\n";
                    debugString += timeSpan.Hours + "hours\n";
                    debugString += timeSpan.Minutes + "minutes\n";
                    debugString += timeSpan.Seconds + "seconds\n";

                    Debug.Log(debugString);
                }
            }

            using (new BoxScope())
            {
                CentralizedLabel("Others");

                GUILayout.Space(10);
                if (GUILayout.Button("SpiritForm?"))
                {
                    Debug.Log(PlayerManager.inSpiritForm);
                }

                GUILayout.Space(10);

                if (GUILayout.Button("notification"))
                {
                    Sprite spr = null;
                    AvatarSpriteUtil.Instance.GeneratePortrait(
                        PlayerDataManager.playerData.male,
                        PlayerDataManager.playerData.equipped, _spr =>
                        {
                            spr = _spr;

                            PlayerNotificationManager.Instance.ShowNotification(
                                Random.Range(0f, 100000f) + " notifcation dsaoidh aso´bd hsaodh saodh aso dhasohsoádhas oídha odha dohas dosadgoás",
                                spr
                            );
                        });
                }

                GUILayout.Space(10);
                if (GUILayout.Button("Skip tutorial"))
                {
                    FTFManager.SkipFTF();
                }

                GUILayout.Space(5);

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying == false);
                m_SpellId = EditorGUILayout.TextField("spell id", m_SpellId);
                if (GUILayout.Button(m_SpellId.Replace("spell_", "").ToUpper() + " EVERYONE!"))
                {
                    SpellData spell = new List<SpellData>(DownloadedAssets.spellDictData.Values).Find(_spell => _spell.id.ToLower().Contains(m_SpellId));

                    if (spell != null)
                    {
                        foreach (var markers in MarkerSpawner.Markers.Values)
                        {
                            if (Spellcasting.CanCast(spell, markers[0]) == Spellcasting.SpellState.CanCast)
                            {
                                if (markers[0].type == MarkerSpawner.MarkerType.SPIRIT || markers[0].type == MarkerSpawner.MarkerType.WITCH)
                                    LeanTween.value(0, 0, 0.05f).setOnComplete(() => Spellcasting.CastSpell(spell, markers[0], new List<spellIngredientsData>(), null, null));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError(m_SpellId + " not found");
                    }
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);
                if (GUILayout.Button("Create 30 player"))
                {
                    Debug.LogError("DISABLED!");
                    //if (EditorApplication.isPlaying == false)
                    //{
                    //    Debug.LogError("not in playmode");
                    //    return;
                    //}


                    //int start = 60;
                    //int end = 150;


                    //System.Action<int> createAcc = (idx) => { };

                    //System.Action<int, double, double> createChar = (idx, lng, lat) =>
                    //{
                    //    var crateCharacterData = new PlayerCharacterCreateAPI();
                    //    crateCharacterData.displayName = "fake " + idx.ToString("000");
                    //    crateCharacterData.latitude = lat;
                    //    crateCharacterData.longitude = lng;
                    //    crateCharacterData.male = Random.Range(0, 2) == 0 ? true : false;
                    //    crateCharacterData.characterSelection = (new string[] 
                    //    {
                    //        "femaleAfrican",
                    //        "femaleEuropean",
                    //        "femaleOriental",
                    //        "maleAfrican",
                    //        "maleEuropean",
                    //        "maleOriental"
                    //    })[Random.Range(0,6)];

                    //    APIManager.Instance.Put("create-character",
                    //        JsonConvert.SerializeObject(crateCharacterData),
                    //        (_response, _result) =>
                    //        {
                    //            if (_result == 200)
                    //            {
                    //                Debug.Log("character \"" + crateCharacterData.displayName + "\" created");
                    //            }
                    //            else
                    //            {
                    //                Debug.Log("failed creating character \"" + crateCharacterData.displayName + $"\". error[{_result}] " + _response);
                    //            }

                    //            if (idx < end)
                    //                createAcc(idx + 1);

                    //        }, true, false);
                    //};

                    //createAcc = (idx) =>
                    //{
                    //    float range = 1f / 300f;
                    //    float lng = GetGPS.longitude + Random.Range(-range, range);
                    //    range = 1f / 450f;
                    //    float lat = GetGPS.latitude + Random.Range(-range, range);

                    //    var createAccountdata = new PlayerLoginAPI();
                    //    createAccountdata.username = "fake" + idx.ToString("000");
                    //    createAccountdata.password = "password";
                    //    createAccountdata.game = "covens";
                    //    createAccountdata.language = Application.systemLanguage.ToString();
                    //    createAccountdata.latitude = lat;
                    //    createAccountdata.longitude = lng;
                    //    createAccountdata.UID = SystemInfo.deviceUniqueIdentifier;

                    //    APIManager.Instance.Put(
                    //        "create-account",
                    //        JsonConvert.SerializeObject(createAccountdata),
                    //        (response, result) =>
                    //        {
                    //            if (result == 200)
                    //            {
                    //                Debug.Log("account " + createAccountdata.username + " created");
                    //                var responseData = JsonConvert.DeserializeObject<PlayerLoginCallback>(response);
                    //                LoginAPIManager.loginToken = responseData.token;
                    //                LoginAPIManager.wssToken = responseData.wsToken;

                    //                createChar(idx, createAccountdata.longitude, createAccountdata.latitude);
                    //            }
                    //            else
                    //            {
                    //                Debug.Log("failed creating account " + createAccountdata.username + ". error " + response);

                    //                APIManager.Instance.Post("login", 
                    //                    JsonConvert.SerializeObject(createAccountdata), (_response, _result) =>
                    //                    {
                    //                        if (_result == 200)
                    //                        {
                    //                            Debug.Log("logged in as " + createAccountdata.username);
                    //                            var responseData = JsonConvert.DeserializeObject<PlayerLoginCallback>(_response);
                    //                            LoginAPIManager.loginToken = responseData.token;
                    //                            LoginAPIManager.wssToken = responseData.wsToken;
                    //                            createChar(idx, createAccountdata.longitude, createAccountdata.latitude);
                    //                        }
                    //                        else
                    //                        {
                    //                            Debug.Log("failed logging in to account " + createAccountdata.username + ". error " + _response);
                    //                        }
                    //                    }, false, false);

                    //            }
                    //        }, false, false);
                    //};
                    
                    ////start creating
                    //createAcc(start);
                }

                GUILayout.Space(5);
                if (GUILayout.Button("get playerdata from server"))
                {
                    APIManager.Instance.Get("character/me", (result, response) =>
                    {
                        if (response == 200)
                            result = Newtonsoft.Json.JsonConvert.SerializeObject(Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerData>(result), Formatting.Indented);
                        Debug.LogError(result);
                    });
                }
            }
        }

        private void Chat()
        {
            if (m_ChatDebugPlayer == null)
            {
                m_ChatDebugPlayer = new Raincrow.Chat.ChatPlayer
                {
                    id = "local:7457a139-8e5c-4989-85b3-6c8eeead577a",
                    name = "lucas 002",
                    level = 99,
                    degree = 1,
                    avatar = 0
                };
            }

            //draw chat player editor
            using (var scroll = new EditorGUILayout.ScrollViewScope(m_ChatPlayerScroll, GUILayout.Height(100)))
            {
                m_ChatPlayerScroll = scroll.scrollPosition;

                string debugPlayerString = SerializeObj(m_ChatDebugPlayer);

                EditorGUI.BeginChangeCheck();
                debugPlayerString = EditorGUILayout.TextArea(debugPlayerString, GUILayout.ExpandHeight(true));
                if (EditorGUI.EndChangeCheck())
                    m_ChatDebugPlayer = JsonConvert.DeserializeObject<Raincrow.Chat.ChatPlayer>(debugPlayerString);
            }

            GUILayout.Space(2);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying == false);
            {
                using (new BoxScope())
                {
                    //connected info
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        GUILayout.Toggle(Raincrow.Chat.ChatManager.Connected, "Connected");
                        GUILayout.Toggle(Raincrow.Chat.ChatManager.IsConnected(Raincrow.Chat.ChatCategory.WORLD), "ConnectedToWorld");
                        EditorGUI.EndDisabledGroup();
                    }

                    if (GUILayout.Button("Init chat"))
                    {
                        Raincrow.Chat.ChatManager.InitChat(m_ChatDebugPlayer);
                    }

                    if (GUILayout.Button("Show UI"))
                    {
                        UIChat uiChat = FindObjectOfType<UIChat>();
                        uiChat.Show();
                    }
                }

                GUILayout.Space(5);

                using (new BoxScope())
                {
                    //draw message editor
                    if (m_ChatDebugMessage == null)
                        m_ChatDebugMessage = new Raincrow.Chat.ChatMessage();
                    m_ChatDebugMessage = DrawChatMessage(m_ChatDebugMessage);

                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Send (World)"))
                        {
                            Raincrow.Chat.ChatManager.SendMessage(Raincrow.Chat.ChatCategory.WORLD, m_ChatDebugMessage);
                        }
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        public Raincrow.Chat.ChatMessage DrawChatMessage(Raincrow.Chat.ChatMessage message)
        {
            using (new BoxScope())
            {
                var previousType = message.type;
                message.type = (Raincrow.Chat.MessageType)EditorGUILayout.EnumPopup("Type", message.type);

                if (message.type != previousType)
                    message.data = new Raincrow.Chat.ChatMessageData();

                if (message.type == Raincrow.Chat.MessageType.TEXT)
                {
                    message.data.message = EditorGUILayout.TextField("Message", message.data.message);
                }
                else if (message.type == Raincrow.Chat.MessageType.LOCATION)
                {
                    message.data.longitude = EditorGUILayout.DoubleField("Longitude", message.data.longitude);
                    message.data.latitude = EditorGUILayout.DoubleField("Latitude", message.data.latitude);
                }
            }

            return message;
        }

        private static string SerializeObj(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        private bool Foldout(bool value, string content)
        {
            using (new BoxScope())
            {
                value = EditorGUILayout.Foldout(value, content, true);
            }
            return value;
        }

        private void CentralizedLabel(string text)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(text);
                GUILayout.FlexibleSpace();
            }
        }

        private string m_DebugRequest;
        private string m_DebugData = "{}";
        private string m_DebugResponse;
        private string m_RequestType = "GET";
        private bool m_RequireAuth = true;
        private static List<string> m_RequestTypeOptions = new List<string>
        {
            "GET",
            "POST",
            "PUT",
            "DELETE",
            "PATCH",
        };

        private Vector2 m_DataScroll;
        private Vector2 m_ResponseScroll;
        private bool m_ShowRequestDebug
        {
            get { return EditorPrefs.GetBool("DebugUtils.RequestDebug", false); }
            set { EditorPrefs.SetBool("DebugUtils.RequestDebug", value); }
        }

        private void DrawRequestDebug()
        {
            m_ShowRequestDebug = Foldout(m_ShowRequestDebug, "Debug requests");
            if (m_ShowRequestDebug)
            {
                using (new BoxScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Request:", m_LabelWidth);
                        m_DebugRequest = EditorGUILayout.TextField(m_DebugRequest);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Method:", m_LabelWidth);
                        int indexOf = m_RequestTypeOptions.IndexOf(m_RequestType);
                        indexOf = EditorGUILayout.Popup(indexOf, m_RequestTypeOptions.ToArray());
                        m_RequestType = m_RequestTypeOptions[indexOf];
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Authorization:", m_LabelWidth);
                        m_RequireAuth = EditorGUILayout.Toggle(m_RequireAuth);
                    }

                    GUILayout.Label($"Data:");
                    using (var scroll = new EditorGUILayout.ScrollViewScope(m_DataScroll, GUILayout.Height(100)))
                    {
                        m_DataScroll = scroll.scrollPosition;
                        m_DebugData = EditorGUILayout.TextArea(m_DebugData, GUILayout.ExpandHeight(true));
                    }

                    GUILayout.Label($"Response:");
                    using (var scroll = new EditorGUILayout.ScrollViewScope(m_ResponseScroll, GUILayout.Height(100)))
                    {
                        m_ResponseScroll = scroll.scrollPosition;
                        m_DebugResponse = EditorGUILayout.TextArea(m_DebugResponse, GUILayout.ExpandHeight(true));
                    }

                    if (GUILayout.Button("Send"))
                    {
                        APIManager.Instance.StartCoroutine(APIManagerServer.RequestServerRoutine(m_DebugRequest, m_DebugData, m_RequestType, m_RequireAuth, false, (response, result) =>
                        {
                            m_DebugResponse = response;
                        }));
                    }
                }
            }
        }
    }

}