using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class LoginAPIManager
{
    public static bool characterLoggedIn { get { return PlayerDataManager.playerData != null; } }
    public static bool accountLoggedIn { get { return !(string.IsNullOrEmpty(loginToken) || string.IsNullOrEmpty(wssToken)) ; } }
    
    public static string loginToken
    {
        get { return PlayerPrefs.GetString("authToken", ""); }
        set { PlayerPrefs.SetString("authToken", value); }
    }

    public static string wssToken
    {
        get { return PlayerPrefs.GetString("wssToken", ""); }
        set { PlayerPrefs.SetString("wssToken", value); }
    }

    public static event System.Action OnCharacterReady;

    public static void Login(System.Action<int, string> callback)
    {
        //check for saved tokens
        if (accountLoggedIn)
        {
            Debug.Log("Login skiped (Token already set)");
            callback(200, "");
            return;
        }

        //send a login failed with error [4100] USER_USERNAME_PASSWORD_NULL_OR_EMPTY
        callback(400, "4100");
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
                    //StoredUserName = username;
                    //StoredUserPassword = password;
                    Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                    loginToken = responseData["game"];
                    wssToken = responseData["socket"];
                }

                callback(result, response);
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
                    callback(result, response);
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
                    OnCharacterReady?.Invoke();
                }

                callback(result, response);
            });
    }

    public static void GetCharacter(System.Action<int, string> callback)
    {
        APIManager.Instance.Get("character/me", "", (response, result) =>
        {
            if (result == 200)
            {
                PlayerDataManager.playerData = ParsePlayerData(response);
                OnCharacterReady?.Invoke();
            }

            callback(result, response);
        });
    }

    private static PlayerDataDetail ParsePlayerData(string json)
    {
        PlayerDataDetail player = JsonConvert.DeserializeObject<PlayerDataDetail>(json);

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

        Debug.LogError("TODO: USE ACTUAL INVENTORY");
        player.inventory = new Inventory
        {
            consumables = new List<Item>(),
            cosmetics = new List<ApparelData>()
        };

        Debug.LogError("TODO: GET DAILIES");
        player.dailies = new Dailies
        {
            explore = new Explore { },
            gather = new Gather { },
            spellcraft = new Spellcraft { }
        };

        Debug.LogError("TODO: GET BLESSINGS");
        player.blessing = new Blessing { };

        Debug.LogError("TODO: GET KNOWN SPIRITS");
        player.knownSpirits = new List<KnownSpirits> { };

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
}