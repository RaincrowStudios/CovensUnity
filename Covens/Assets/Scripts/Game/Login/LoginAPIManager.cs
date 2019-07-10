using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class LoginAPIManager
{
    public static bool loggedIn { get; private set; }
    public static bool accountLoggedIn { get { return !(string.IsNullOrEmpty(loginToken) || string.IsNullOrEmpty(wssToken)) ; } }

    public static string systemLanguage;

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

    public static string StoredUserName
    {
        get { return PlayerPrefs.GetString("Username", ""); }
        set { PlayerPrefs.SetString("Username", value); }
    }
    public static string StoredUserPassword
    {
        get { return PlayerPrefs.GetString("Password", ""); }
        set { PlayerPrefs.SetString("Password", value); }
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

        //try autologin with stored user
        if (string.IsNullOrEmpty(StoredUserName) == false && string.IsNullOrEmpty(StoredUserPassword) == false)
        {
            Debug.Log("Loging in with stored user " + StoredUserName);
            Login(StoredUserName, StoredUserPassword, callback);
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

        APIManager.Instance.Post("login", JsonConvert.SerializeObject(data), 
            (response, result) =>
            {
                if (result == 200)
                {
                    StoredUserName = username;
                    StoredUserPassword = password;
                    Dictionary<string, string> responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                    loginToken = responseData["game"];
                    wssToken = responseData["socket"];
                }

                callback(result, response);
            }, 
            false, 
            false);
    }

    public static void CreateAccount(string username, string password, System.Action<int, string> callback)
    {
        Debug.LogError("TODO: CREATE ACCOUNT");
    }

    public static void GetCharacter(System.Action<int, string> callback)
    {
        APIManager.Instance.GetCoven("character/me", "", (response, result) =>
        {
            if (result == 200)
            {
                PlayerDataManager.playerData = ParsePlayerData(response);
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

        //

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