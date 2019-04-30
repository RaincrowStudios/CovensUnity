using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class DebugUtils : EditorWindow
{
    [MenuItem("Tools/DebugUtils")]
    static void Init()
    {
        DebugUtils window = (DebugUtils)EditorWindow.GetWindow(typeof(DebugUtils), false, "Debug");
    }

    private int m_CurrentTab = 0;
    private string[] m_TabOptions = new string[] { "Users", "Others" };
    private Vector2 m_ScrollPosition = Vector2.zero;
    private Vector3 m_Vector3;
    private float m_Float1;
    private float m_Float2;
    private float m_Float3;

    private void OnGUI()
    {
        m_CurrentTab = GUILayout.Toolbar(m_CurrentTab, m_TabOptions);

        using (var scrollScope = new GUILayout.ScrollViewScope(m_ScrollPosition))
        {
            m_ScrollPosition = scrollScope.scrollPosition;

            switch (m_CurrentTab)
            {
                case 0:
                    Users(); break;
                case 1:
                    Others(); break;
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

        if (user.Username == LoginAPIManager.StoredUserName && user.Password == LoginAPIManager.StoredUserPassword)
        {
            GUI.backgroundColor = Color.green;
        }

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


    private string m_sWsData = "{}";
    private string m_sItemData = "{}";
    private double m_JavascriptDate = 0;
    private float m_Longitude;
    private float m_Latitude;

    private void Others()
    {
        EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

        using (new BoxScope())
        {
            CentralizedLabel("GPS");
            using (new GUILayout.HorizontalScope())
            {
                bool interactable = Application.isPlaying && GetGPS.instance != null;
                EditorGUI.BeginDisabledGroup(!interactable);

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

                EditorGUI.EndDisabledGroup();
            }
        }

        using (new BoxScope())
        {
            CentralizedLabel("Editor");

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

        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying == false || SceneManager.GetActiveScene().name.Contains("Main") == false);

        using (new BoxScope())
        {
            CentralizedLabel("Websocket");

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("data:", GUILayout.Width(40));
                m_sWsData = EditorGUILayout.TextField(m_sWsData);
            }
            if (GUILayout.Button("Send fakeWS"))
            {
                WSData data = JsonConvert.DeserializeObject<WSData>(m_sWsData);
                data.timeStamp = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
                WebSocketClient.Instance.ManageData(data);
            }
        }

        GUILayout.Space(10);

        using (new BoxScope())
        {
            CentralizedLabel("Items");

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("data:", GUILayout.Width(40));
                m_sItemData = EditorGUILayout.TextField(m_sItemData);
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Add cosmetic"))
            {
                ApparelData data = JsonConvert.DeserializeObject<ApparelData>(m_sItemData);
                PlayerDataManager.playerData.inventory.cosmetics.Add(data);
            }

            if (GUILayout.Button("Owned consumables"))
            {
                List<StoreDictData> storeData = new List<StoreDictData>();
                List<ConsumableItem> consumableData = new List<ConsumableItem>();

                foreach (ConsumableItem item in PlayerDataManager.playerData.inventory.consumables)
                {
                    consumableData.Add(item);
                    if (DownloadedAssets.storeDict.ContainsKey(item.id))
                    {
                        storeData.Add(DownloadedAssets.storeDict[item.id]);
                    }
                }
                Debug.Log(SerializeObj(storeData));
                Debug.LogError(SerializeObj(consumableData));
            }

            if (GUILayout.Button("Owned cosmetics"))
            {
                List<StoreDictData> storeData = new List<StoreDictData>();
                List<ApparelData> apparelData = new List<ApparelData>();
                foreach (ApparelData item in PlayerDataManager.playerData.inventory.cosmetics)
                {
                    apparelData.Add(item);
                    if (DownloadedAssets.storeDict.ContainsKey(item.id))
                    {
                        storeData.Add(DownloadedAssets.storeDict[item.id]);
                    }
                }
                Debug.Log(SerializeObj(storeData));
                Debug.LogError(SerializeObj(apparelData));
            }
        }

        EditorGUI.EndDisabledGroup();

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
                            "notifcation dsaoidh aso´bd hsaodh saodh aso dhasohsoádhas oídha odha dohas dosadgoás",
                            spr
                        );
                    });
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Skip tutorial"))
            {
                APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
                {
                    PlayerManager.Instance.initStart();
                });
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Create 30 player"))
            {
                if (EditorApplication.isPlaying == false)
                {
                    Debug.LogError("not in playmode");
                    return;
                }


                int start = 60;
                int end = 150;


                System.Action<int> createAcc = (idx) => { };

                System.Action<int, double, double> createChar = (idx, lng, lat) =>
                {
                    var crateCharacterData = new PlayerCharacterCreateAPI();
                    crateCharacterData.displayName = "fake " + idx.ToString("000");
                    crateCharacterData.latitude = lat;
                    crateCharacterData.longitude = lng;
                    crateCharacterData.male = Random.Range(0, 2) == 0 ? true : false;
                    crateCharacterData.characterSelection = (new string[] 
                    {
                        "femaleAfrican",
                        "femaleEuropean",
                        "femaleOriental",
                        "maleAfrican",
                        "maleEuropean",
                        "maleOriental"
                    })[Random.Range(0,6)];

                    APIManager.Instance.Put("create-character",
                        JsonConvert.SerializeObject(crateCharacterData),
                        (_response, _result) =>
                        {
                            if (_result == 200)
                            {
                                Debug.Log("character \"" + crateCharacterData.displayName + "\" created");
                            }
                            else
                            {
                                Debug.Log("failed creating character \"" + crateCharacterData.displayName + $"\". error[{_result}] " + _response);
                            }

                            if (idx < end)
                                createAcc(idx + 1);

                        }, true, false);
                };

                createAcc = (idx) =>
                {
                    float range = 1f / 300f;
                    float lng = GetGPS.longitude + Random.Range(-range, range);
                    range = 1f / 450f;
                    float lat = GetGPS.latitude + Random.Range(-range, range);

                    var createAccountdata = new PlayerLoginAPI();
                    createAccountdata.username = "fake" + idx.ToString("000");
                    createAccountdata.password = "password";
                    createAccountdata.game = "covens";
                    createAccountdata.language = Application.systemLanguage.ToString();
                    createAccountdata.latitude = lat;
                    createAccountdata.longitude = lng;
                    createAccountdata.UID = SystemInfo.deviceUniqueIdentifier;

                    APIManager.Instance.Put(
                        "create-account",
                        JsonConvert.SerializeObject(createAccountdata),
                        (response, result) =>
                        {
                            if (result == 200)
                            {
                                Debug.Log("account " + createAccountdata.username + " created");
                                var responseData = JsonConvert.DeserializeObject<PlayerLoginCallback>(response);
                                LoginAPIManager.loginToken = responseData.token;
                                LoginAPIManager.wssToken = responseData.wsToken;

                                createChar(idx, createAccountdata.longitude, createAccountdata.latitude);
                            }
                            else
                            {
                                Debug.Log("failed creating account " + createAccountdata.username + ". error " + response);
                                
                                APIManager.Instance.Post("login", 
                                    JsonConvert.SerializeObject(createAccountdata), (_response, _result) =>
                                    {
                                        if (_result == 200)
                                        {
                                            Debug.Log("logged in as " + createAccountdata.username);
                                            var responseData = JsonConvert.DeserializeObject<PlayerLoginCallback>(_response);
                                            LoginAPIManager.loginToken = responseData.token;
                                            LoginAPIManager.wssToken = responseData.wsToken;
                                            createChar(idx, createAccountdata.longitude, createAccountdata.latitude);
                                        }
                                        else
                                        {
                                            Debug.Log("failed logging in to account " + createAccountdata.username + ". error " + _response);
                                        }
                                    }, false, false);

                            }
                        }, false, false);
                };


                //start creating
                createAcc(start);
            }
        }
    }

    private string SerializeObj(object obj)
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
}
