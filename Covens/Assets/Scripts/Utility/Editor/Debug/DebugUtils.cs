using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Raincrow.Chat.UI;
using UnityEditor.Callbacks;
using Raincrow.FTF;

namespace Raincrow.Test
{
    public partial class DebugUtils : EditorWindow
    {
        private static DebugUtils m_Window;

        [MenuItem("Tools/DebugUtils")]
        static void Init()
        {
            m_Window = (DebugUtils)GetWindow(typeof(DebugUtils), false, "Debug Utils");
        }

        private int m_CurrentTab = 0;
        private string[] m_TabOptions = new string[] { "Users", "Socket", "Others", "Spells" };
        private Vector2 m_ScrollPosition = Vector2.zero;
        private Vector3 m_Vector3;
        private float m_Float1;
        private float m_Float2;
        private float m_Float3;
        private string m_SpellId = "spell_hex";

        [SerializeField] DebugUtilsSocket socketDebug = null;
        [SerializeField] SpellsSheetHelper spellsHelper = null;

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
                        if (socketDebug == null)
                            socketDebug = new DebugUtilsSocket();
                        socketDebug.Draw();
                        break;
                    case 2:
                        Others();
                        break;
                    case 3:
                        if (spellsHelper == null)
                            spellsHelper = new SpellsSheetHelper();
                        spellsHelper.DrawGUI();
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
            ExpandTokens = BoxFoldout(ExpandTokens, "Tokens");
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

            ExpandCurrentUser = BoxFoldout(ExpandCurrentUser, "Current User");
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

            ExpandStoredUsers = BoxFoldout(ExpandStoredUsers, "Stored Users");
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


        [SerializeField] private string m_sCommandResponse = "{}";
        [SerializeField] private string m_sCommandResponseData = "{}";
        [SerializeField] private string m_sItemData = "{}";
        [SerializeField] private double m_JavascriptDate = 0;
        [SerializeField] private float m_Longitude;
        [SerializeField] private float m_Latitude;

        public static bool UseLocalGameDictionary
        {
            get => EditorPrefs.GetBool("DebugUtils.UseLocalGameDict", false);
            set => EditorPrefs.SetBool("DebugUtils.UseLocalGameDict", value);
        }

        [SerializeField] private Vector2 m_ChatPlayerScroll = Vector2.zero;
        private Raincrow.Chat.ChatPlayer m_ChatDebugPlayer = null;
        private Raincrow.Chat.ChatMessage m_ChatDebugMessage;

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
                if (GUILayout.Button("daily blessing"))
                {
                    BlessingManager.CheckDailyBlessing();
                }
                if (GUILayout.Button("Reset Firsts"))
                {
                    FirstTapManager.ResetFirsts();
                }
                if (GUILayout.Button("spirit discovered"))
                {
                    UISpiritDiscovered.Instance.Show("spirit_barghest");
                }
                if(GUILayout.Button("summon success"))
                {
                    UISummonSuccess.Instance.Show("spirit_barghest");
                }
                if (GUILayout.Button("Trigger low memory"))
                {
                    DownloadedAssets.Instance.OnApplicationLowMemory();
                }

                EditorGUI.EndDisabledGroup();
            }

            using (new BoxScope())
            {
                CentralizedLabel("Editor");
                
                if (GUILayout.Button("persistentDataPath"))
                {
                    EditorUtility.RevealInFinder(Application.persistentDataPath);
                }

                if (GUILayout.Button("?"))
                {
                    var names = AssetDatabase.GetAllAssetBundleNames();
                    foreach (string name in names)
                        Debug.Log("Asset Bundle: " + name);
                }

                GUILayout.Space(5);
                GUILayout.Label("Dictionary");
                UseLocalGameDictionary = EditorGUILayout.ToggleLeft("Use local game.json (\"Editor Default Resources/game.json\"", UseLocalGameDictionary);
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);

            DrawRequestDebug();
            
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

                if (GUILayout.Button("utc now"))
                {
                    Debug.Log(System.DateTime.UtcNow);
                }
            }

            using (new BoxScope())
            {
                CentralizedLabel("Others");

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying == false);

                GUILayout.Space(10);

                if (GUILayout.Button("Login"))
                {
                    DownloadManager.OnDownloadsComplete += () => LoginAPIManager.Login((result, response) => LoginAPIManager.GetCharacter(null));
                    DownloadManager.DownloadAssets(null);
                }
                
                if (GUILayout.Button("Log PlayerData"))
                {
                    string debug = "[" + PlayerDataManager.playerData.instance + "] " + PlayerDataManager.playerData.name + "\n";
                    debug += SerializeObj(PlayerDataManager.playerData);
                    Debug.Log(debug);
                }

                if (GUILayout.Button("Pop Notification"))
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

                if (GUILayout.Button("Skip tutorial"))
                {
                    FTFManager.SkipFTF();
                }

                //m_SpellId = EditorGUILayout.TextField("spell id", m_SpellId);
                //if (GUILayout.Button(m_SpellId.Replace("spell_", "").ToUpper() + " EVERYONE!"))
                //{
                //    SpellData spell = new List<SpellData>(DownloadedAssets.spellDictData.Values).Find(_spell => _spell.id.ToLower().Contains(m_SpellId));

                //    if (spell != null)
                //    {
                //        foreach (var markers in MarkerSpawner.Markers.Values)
                //        {
                //            if (Spellcasting.CanCast(spell, markers[0]) == Spellcasting.SpellState.CanCast)
                //            {
                //                if (markers[0].Type == MarkerSpawner.MarkerType.SPIRIT || markers[0].Type == MarkerSpawner.MarkerType.WITCH)
                //                    LeanTween.value(0, 0, 0.05f).setOnComplete(() => Spellcasting.CastSpell(spell, markers[0], new List<spellIngredientsData>(), null, null));
                //            }
                //        }
                //    }
                //    else
                //    {
                //        Debug.LogError(m_SpellId + " not found");
                //    }
                //}

                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);
                CentralizedLabel("Tutorial");
                int ftfStartingStep = EditorPrefs.GetInt("FTFManager.StartFrom", 0);
                ftfStartingStep = EditorGUILayout.IntField("Start from", ftfStartingStep);
                if (ftfStartingStep != EditorPrefs.GetInt("FTFManager.StartFrom", 0))
                    EditorPrefs.SetInt("FTFManager.StartFrom", ftfStartingStep);
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

        public static bool BoxFoldout(bool value, string content)
        {
            using (new BoxScope())
            {
                value = EditorGUILayout.Foldout(value, content, true);
            }
            return value;
        }

        public static bool Foldout(bool value, string content)
        {
            return EditorGUILayout.Foldout(value, content, true);
        }

        public static void CentralizedLabel(string text)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(text);
                GUILayout.FlexibleSpace();
            }
        }

        [SerializeField] private string m_DebugRequest;
        [SerializeField] private string m_DebugData = "{}";
        [SerializeField] private string m_DebugResponse;
        [SerializeField] private string m_RequestType = "GET";
        [SerializeField] private bool m_RequireAuth = true;
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
            m_ShowRequestDebug = BoxFoldout(m_ShowRequestDebug, "Debug requests");
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