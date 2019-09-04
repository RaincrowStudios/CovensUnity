using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class LoginAPIManager
{
    public struct LoginResponse
    {
        public string game;
        public string socket;
        public bool? hasCharacter;
        public string error;
    }

    public static bool characterLoggedIn { get { return PlayerDataManager.playerData != null; } }
    public static bool accountLoggedIn { get { return !(string.IsNullOrEmpty(loginToken) || string.IsNullOrEmpty(wssToken)); } }

    public static string loginToken
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString("authToken", "");
#endif
            return PlayerPrefs.GetString("authToken", "");
        }
        set
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString("authToken", value);
            return;
#endif
            PlayerPrefs.SetString("authToken", value);
        }
    }

    public static string wssToken
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString("wssToken", "");
#endif
            return PlayerPrefs.GetString("wssToken", "");
        }
        set
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString("wssToken", value);
            return;
#endif
            PlayerPrefs.SetString("wssToken", value);
        }
    }

    public static string StoredUserName
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString("Username", "");
#endif
            return PlayerPrefs.GetString("Username", "");
        }
        set
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString("Username", value);
            return;
#endif
            PlayerPrefs.SetString("Username", value);
        }
    }
    public static string StoredUserPassword
    {
        get
        {
#if UNITY_EDITOR
            return UnityEditor.EditorPrefs.GetString("Password", "");
#endif
            return PlayerPrefs.GetString("Password", "");
        }
        set
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetString("Password", value);
            return;
#endif
            PlayerPrefs.SetString("Password", value);
        }
    }

    public static event System.Action OnCharacterReceived;

    public static void RefreshTokens(System.Action<bool> callback)
    {
        loginToken = "";
        wssToken = "";
        UnityEngine.Networking.UnityWebRequest.ClearCookieCache();
        PlayerPrefs.DeleteKey("cookie");

        //try autologin with stored user
        if (string.IsNullOrEmpty(StoredUserName) == false && string.IsNullOrEmpty(StoredUserPassword) == false)
        {
            Debug.Log("Refreshing tokens with user \"" + StoredUserName + "\"");
            Login(StoredUserName, StoredUserPassword, (result, response) =>
            {
                if (result == 200)
                {
                    callback?.Invoke(true);
                }
                else
                {
                    callback?.Invoke(false);
                }
            });
        }
        else
        {
            callback?.Invoke(false);
        }
    }

    public static void Login(System.Action<int, LoginResponse> callback)
    {
        APIManager.Instance.Put("place-of-power/leave", "{}", (s, r) => { Debug.Log(s); Debug.Log("TODO: Move pop leave logic"); });

        //check for saved tokens
        if (accountLoggedIn)
        {
            Debug.Log("Login skiped (Token already set)");
            callback?.Invoke(200, new LoginResponse()
            {
                game = loginToken,
                socket = wssToken,
                error = null
            });
            return;
        }

        //try autologin with stored user
        if (string.IsNullOrEmpty(StoredUserName) == false && string.IsNullOrEmpty(StoredUserPassword) == false)
        {
            Debug.Log("Loging in with stored user " + StoredUserName);
            Login(StoredUserName, StoredUserPassword, callback);
            return;
        }

        //send a login failed with error [4100] USER_USERNAME_PASSWORD_NULL_OR_EMPTY
        callback?.Invoke(400, new LoginResponse() { error = "4001" });
    }

    public static void Login(string username, string password, System.Action<int, LoginResponse> callback)
    {
        var plat = "";
        if (Application.platform == RuntimePlatform.Android)
            plat = "android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            plat = "ios";
        else
            plat = Application.platform.ToString().ToLower();

        var data = new
        {
            username = username,
            password = password,
            longitude = GetGPS.longitude,
            latitude = GetGPS.latitude,
            UID = SystemInfo.deviceUniqueIdentifier,
            game = "covens",
            platform = plat,
            notificationsEnabled = true
        };

        APIManager.Instance.PostRaincrow("login", JsonConvert.SerializeObject(data),
            (response, result) =>
            {
                if (result == 200)
                {
                    StoredUserName = username;
                    StoredUserPassword = password;

                    LoginResponse loginData = JsonConvert.DeserializeObject<LoginResponse>(response);
                    //REMOVE LATER
                    loginToken = loginData.game;
                    wssToken = loginData.socket;

                    callback(result, loginData);
                }
                else
                {
                    callback?.Invoke(result, new LoginResponse() { error = response });
                }
            },
            false,
            false);
    }

    public static void CreateAccount(string username, string password, string email, System.Action<int, LoginResponse> callback)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("username", username);
        data.Add("password", password);
        if (string.IsNullOrEmpty(email) == false)
            data.Add("email", email);

        //data.Add("game", "covens");
        data.Add("language", PlayerManager.SystemLanguage);
        data.Add("latitude", GetGPS.latitude);
        data.Add("longitude", GetGPS.longitude);
        //data.Add("UID", SystemInfo.deviceUniqueIdentifier);

        APIManager.Instance.PostRaincrow("account", JsonConvert.SerializeObject(data),
            (response, result) =>
            {
                if (result == 200)
                {
                    Login(username, password, callback);
                }
                else
                {
                    callback?.Invoke(result, new LoginResponse() { error = response });
                }
            },
            false,
            false);
    }

    public static void CreateCharacter(string name, int bodyType, bool male, System.Action<int, LoginResponse> callback)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("name", name);
        data.Add("bodyType", bodyType);
        data.Add("male", male);
        data.Add("longitude", GetGPS.longitude);
        data.Add("latitude", GetGPS.latitude);

        APIManager.Instance.Post("character", JsonConvert.SerializeObject(data),
            (response, result) =>
            {
                if (result == 200)
                {
                    //PlayerDataManager.playerData = ParsePlayerData(response);
                    //OnCharacterReceived?.Invoke();
                    //callback?.Invoke(result, response);
                    Debug.LogError("TEMP FIX - SOCKET NOT WORKING AFTER CREATING A NEW ACCOUNT/CHARACTER");

                    loginToken = null;
                    wssToken = null;

                    LoginAPIManager.Login((_loginCode, _loginResponse) =>
                    {
                        if (_loginCode == 200)
                        {
                            PlayerDataManager.playerData = ParsePlayerData(response);
                            OnCharacterReceived?.Invoke();
                        }
                        callback?.Invoke(_loginCode, _loginResponse);
                    });
                }
                else
                {
                    callback?.Invoke(result, new LoginResponse() { error = response });
                }
            });
    }

    public static void GetCharacter(System.Action<int, string> callback)
    {
        APIManager.Instance.Get("character/me", "", (response, result) =>
        {
            if (result == 200)
            {
                PlayerDataManager.playerData = ParsePlayerData(response);

                //TeamManager.GetCoven(null);
                OnCharacterReceived?.Invoke();

            }
            else if (result == 412 && response == "1001")
            {
                //no character
            }
            else
            {
                //unhandled error, force the user ti relogin
                loginToken = "";
                wssToken = "";

                //the character was already initialized
                //something went really wrong
                if (PlayerDataManager.playerData != null)
                    APIManager.ThrowCriticalError(null, "character/me", "");
            }
            callback?.Invoke(result, response);
        });
    }

    private static PlayerData ParsePlayerData(string json)
    {
        PlayerData player = JsonConvert.DeserializeObject<PlayerData>(json);
        player.Setup();

        return player;
    }

    public struct GameConfig
    {
        public struct DominionRank
        {
            [JsonProperty("character")]
            public string topPlayer;
            [JsonProperty("coven")]
            public string topCoven;
        }

        public float displayRadius;
        public int tribunal;
        public double daysRemaining;

        public string dominion;
        [JsonProperty("topRanking")]
        public DominionRank dominionRank;

        public Sun sun;
        public MoonData moon;
    }

    public static void GetConfigurations(float longitude, float latitude, System.Action<GameConfig, string> callback)
    {
        APIManager.Instance.GetRaincrow($"configurations?latitude={latitude.ToString().Replace(',', '.')}&longitude={longitude.ToString().Replace(',', '.')}", "", (response, result) =>
         {
             if (result == 200)
             {
                 GameConfig data = JsonConvert.DeserializeObject<GameConfig>(response);

                 PlayerDataManager.DisplayRadius = data.displayRadius / 1000;
                 PlayerDataManager.moonData = data.moon;
                 PlayerDataManager.sunData = data.sun;
                 PlayerDataManager.tribunal = data.tribunal;
                 PlayerDataManager.tribunalDaysRemaining = data.daysRemaining;

                 callback?.Invoke(data, null);
             }
             else
             {
                 callback?.Invoke(new GameConfig(), response);
             }
         });
    }
}