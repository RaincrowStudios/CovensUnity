using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class LoginAPIManager
{
    public static bool characterLoggedIn { get { return PlayerDataManager.playerData != null; } }
    public static bool accountLoggedIn { get { return !(string.IsNullOrEmpty(loginToken) || string.IsNullOrEmpty(wssToken)) ; } }
    
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
#endif
            PlayerPrefs.SetString("Password", value);
        }
    }

    public static event System.Action OnCharacterReceived;

    public static void RefreshTokens(System.Action<bool> callback)
    {
        loginToken = "";
        wssToken = "";

        //try autologin with stored user
        if (string.IsNullOrEmpty(StoredUserName) == false && string.IsNullOrEmpty(StoredUserPassword) == false)
        {
            Debug.Log("Refreshing tokens with user \"" + StoredUserName + "\"");
            Login(StoredUserName, StoredUserPassword, (result, response) =>
            {
                Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                bool hasCharacter = (bool)responseData["hasCharacter"];

                callback?.Invoke(result == 200 && hasCharacter);
            });
        }
        else
        {
            callback?.Invoke(false);
        }
    }

    public static void Login(System.Action<int, string> callback)
    {
        //check for saved tokens
        if (accountLoggedIn)
        {
            Debug.Log("Login skiped (Token already set)");
            callback?.Invoke(200, "");
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
        callback?.Invoke(400, "4100");
    }

    public static void Login(string username, string password, System.Action<int, string> callback)
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
                    Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    loginToken = (string)responseData["game"];
                    wssToken = (string)responseData["socket"];
                }

                callback?.Invoke(result, response);
            }, 
            false, 
            false);
    }

    public static void CreateAccount(string username, string password, string email, System.Action<int, string> callback)
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
                    callback?.Invoke(result, response);
                }
            },
            false,
            false);
    }

    public static void CreateCharacter(string name, int bodyType, bool male, System.Action<int, string> callback)
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
                    PlayerDataManager.playerData = ParsePlayerData(response);
                    OnCharacterReceived?.Invoke();
                }

                callback?.Invoke(result, response);
            });
    }

    public static void GetCharacter(System.Action<int, string> callback)
    {
        APIManager.Instance.Get("character/me", "", (response, result) =>
        {
            if (result == 200)
            {
                PlayerDataManager.playerData = ParsePlayerData(response);
                OnCharacterReceived?.Invoke();
            }

            callback?.Invoke(result, response);
        });
    }

    private static PlayerData ParsePlayerData(string json)
    {
        PlayerData player = JsonConvert.DeserializeObject<PlayerData>(json);

        //setup the ingredient dictionary so it work with the old implementation
        player.ingredients = new Ingredients
        {
            gemsDict = new Dictionary<string, CollectableItem>(),
            toolsDict = new Dictionary<string, CollectableItem>(),
            herbsDict = new Dictionary<string, CollectableItem>(),
        };

        foreach (CollectableItem item in player.gems)
            player.ingredients.gemsDict.Add(item.collectible, item);

        foreach (CollectableItem item in player.herbs)
            player.ingredients.herbsDict.Add(item.collectible, item);

        foreach (CollectableItem item in player.tools)
            player.ingredients.toolsDict.Add(item.collectible, item);
        
        Debug.LogError("TODO: GET DAILIES");
        player.dailies = new Dailies
        {
            explore = new Explore { },
            gather = new Gather { },
            spellcraft = new Spellcraft { }
        };

        Debug.LogError("TODO: GET BLESSINGS");
        player.blessing = new Blessing { };
        
        Debug.LogError("TODO: WATCHED VIDEOS");
        player.firsts = new Firsts { };

        return player;
    }

    public static void WebSocketConnected()
    {
        Debug.LogError("TODO: REPLACE FOR EVENT");
    }

    public static void initiateLogin()
    {
        Debug.LogError("TODO: REPLACE FOR EVENT");
    }

    private struct GameConfig
    {
        public float displayRadius;
        public int tribunal;
        public double daysRemaining;

        public Sun sun;
        public MoonData moon;
    }

    public static void GetConfigurations(float longitude, float latitude, System.Action<int, string> callback)
    {
        APIManager.Instance.GetRaincrow($"configurations?latitude={latitude.ToString().Replace(',','.')}&longitude={longitude.ToString().Replace(',', '.')}", "", (response, result) =>
        {
            if (result == 200)
            {
                GameConfig data = JsonConvert.DeserializeObject<GameConfig>(response);

                PlayerDataManager.DisplayRadius = data.displayRadius / 1000;
                PlayerDataManager.moonData = data.moon;
                PlayerDataManager.sunData = data.sun;
                PlayerDataManager.tribunal = data.tribunal;
                PlayerDataManager.tribunalDaysRemaining = data.daysRemaining;
            }

            callback?.Invoke(result, response);
        });
    }
}