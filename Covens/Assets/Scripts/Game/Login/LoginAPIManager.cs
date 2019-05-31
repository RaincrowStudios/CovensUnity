using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

public class LoginAPIManager : MonoBehaviour
{
    public static bool loggedIn = false;
    public static bool accountLoggedIn = false;
    static string token;
    public static string username;
    static string password;
    public static string loginToken;
    public static string wssToken;
    public static bool isNewAccount = true;
    static PlayerDataDetail rawData;
    public static bool sceneLoaded = false;
    public static bool hasCharacter = false;
    public static bool FTFComplete = false;
    public static bool isInFTF = false;
    public static string systemLanguage = "English";
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

    public static event System.Action OnCharacterInitialized;

    void Awake()
    {
        LeanTween.init(1200);
        //		PlayerPrefs.DeleteKey ("Username");
        //		PlayerPrefs.DeleteKey ("Password");
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {

        MapsAPI.Instance.InstantiateMap();
        OneSignal.StartInit("78e8aff3-7ce2-401f-9da0-2d41f287ebaf")
            .HandleNotificationOpened(HandleNotificationOpened)
            .Settings(new Dictionary<string, bool>() { { OneSignal.kOSSettingsAutoPrompt, true } })
            .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
    }

    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
    }

    public static void AutoLogin()
    {
        if (StoredUserName != "")
        {
            Debug.Log("Username: " + StoredUserName);
            ALogin(StoredUserName, StoredUserPassword);
        }
        else
        {
            if (APIManager.Instance != null)
                StartUpManager.Instance.DoSceneLoading();
            else
                LoginUIManager.Instance.BackToLogin();
        }
    }

    public static void ALogin(string Username, string Password)
    {
        isNewAccount = false;
        var data = new
        {
            username = Username,
            password = Password,
            longitude = PlayerDataManager.playerPos.x,
            latitude = PlayerDataManager.playerPos.y,
            UID = SystemInfo.deviceUniqueIdentifier,
            game = "covens"
        };
        APIManager.Instance.Post("login", JsonConvert.SerializeObject(data), ALoginCallback, false, false);


        var temp = false;
        /*
        if (OneSignal.GetPermissionSubscriptionState().permissionStatus.status == OSNotificationPermission.Authorized)
            temp = true;
        */
        if (OneSignal.GetPermissionSubscriptionState().subscriptionStatus.subscribed)
            temp = true;

        Debug.LogError(temp);

        var pushNotificationData = new
        {
            platform = Application.platform,
            notificationsEnabled = temp
        };
        Debug.LogError(JsonConvert.SerializeObject(pushNotificationData));
        APIManager.Instance.Post("login", JsonConvert.SerializeObject(pushNotificationData), NotificationCallback, false, false);
    }

    static void ALoginCallback(string result, int status)
    {
        if (status == 200)
        {
            if (Application.isEditor)
            {
                TextEditor te = new TextEditor();
                te.text = result;
                te.SelectAll();
                te.Copy();
            }
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;

            Debug.Log(result);
            FTFComplete = data.account.ftf;

            SetupConfig(data.config);
            if (data.account.character)
            {
                hasCharacter = true;
            }
            else
            {
                hasCharacter = false;
            }
            loggedIn = true;
            accountLoggedIn = true;
            OnLoginSuccess();
        }
        else if (status == 0)
        {
            accountLoggedIn = false;
            UIGlobalErrorPopup.ShowError(
                cancelAction: () => ALogin(StoredUserName, StoredUserPassword),
                txt: "Login error",
                cancelTxt: "Retry"
            );
        }
        else
        {
            accountLoggedIn = false;
            StartUpManager.Instance.DoSceneLoading();
        }
    }

    public void OneSignalPromptForPushNotificationsResponse(bool accepted)
    {
        Debug.LogError("OneSignal_promptForPushNotificationsResponse: " + accepted);
    }

    public static void Login(string Username, string Password)
    {

        var data = new
        {
            username = Username,
            password = Password,
            longitude = MapsAPI.Instance.physicalPosition.x,
            latitude = MapsAPI.Instance.physicalPosition.y,
            UID = SystemInfo.deviceUniqueIdentifier,
            game = "covens"
        };
        APIManager.Instance.Post("login", JsonConvert.SerializeObject(data), LoginCallback, false, false);


        var plat = "android";
#if UNITY_IOS
        OneSignal.PromptForPushNotificationsWithUserResponse(OneSignalPromptForPushNotificationsResponse);
        plat = "ios";
#endif

        //Matt's stuff for getting notifications
        var temp = false;
        /*
        if (OneSignal.GetPermissionSubscriptionState().permissionStatus.status == OSNotificationPermission.Authorized)
            temp = true;
        */
        if (OneSignal.GetPermissionSubscriptionState().subscriptionStatus.subscribed)
            temp = true;
        Debug.LogError(temp);


        var pushNotificationData = new
        {
            platform = plat,
            notificationsEnabled = temp
        };
        Debug.LogError(JsonConvert.SerializeObject(pushNotificationData));

        APIManager.Instance.Post("login", JsonConvert.SerializeObject(pushNotificationData), NotificationCallback, false, false);
    }

    static void LoginCallback(string result, int status)
    {
        //   Debug.Log("LoginCallBack:" + status + "  " + result);

        if (status == 200)
        {
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;
            SetupConfig(data.config);
            FTFComplete = data.account.ftf;
            //			loggedIn = true;
            OnLoginSuccess();
            accountLoggedIn = true;

        }
        else
        {
            Debug.Log(status);
            DownloadAssetBundle.Instance.gameObject.SetActive(false);
            LoginUIManager.Instance.WrongPassword();
            accountLoggedIn = false;
            // Debug.Log(status + "," + result);
        }
    }

    static void NotificationCallback(string result, int status)
    {
        if (status == 200)
        {
            var data = JsonConvert.DeserializeObject(result);
            Debug.LogError(result);
        }
        else
        {
            Debug.LogError("push notification data - post failure");
        }
    }

    public static void WebSocketConnected()
    {
        //   Debug.Log("WebSocketConnected");
        if (isNewAccount)
        {
            LoginUIManager.Instance.CreateAccountResponse(true, "");
        }
        else
        {
            GetCharacter(OnGetCharcterResponse);
        }
    }

    static void SetupConfig(Config data)
    {
        PlayerDataManager.config = data;
        PlayerDataManager.attackRadius = data.interactionRadius;
        PlayerDataManager.DisplayRadius = data.displayRadius;
        PlayerDataManager.idleTimeOut = data.idleTimeLimit;
        PlayerDataManager.moonData = data.moon;
        if (!sceneLoaded)
            StartUpManager.config = data;
        foreach (var item in data.summoningMatrix)
        {
            PlayerDataManager.summonMatrixDict[item.spirit] = item;
        }
        WebSocketClient.Instance.InitiateWSSCOnnection();
    }

    public static void GetCharacterReInit()
    {
        GetCharacter(OnGetCharcterInitResponse);
    }

    public static void OnGetCharcterInitResponse(string result, int response)
    {
        if (response == 200)
        {
            Debug.Log("reinit finished");
            rawData = JsonConvert.DeserializeObject<PlayerDataDetail>(result);
            PlayerDataManager.playerData = DictifyData(rawData);
            // PlayerDataManager.currentDominion = PlayerDataManager.playerData.dominion;
            // ChatConnectionManager.Instance.InitChat();

            APIManager.Instance.GetData("/location/leave", (string s, int r) =>
            {
                MarkerManagerAPI.GetMarkers(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, false, null, false, false, true);
            });
            QuestsController.GetQuests(null);
            PlayerManager.Instance.InitFinished();
            GetNewTokens();

            StoreManagerAPI.GetShopItems((string s, int r) =>
            {
                if (r == 200)
                {
                    //   Debug.Log(s);
                    PlayerDataManager.StoreData = JsonConvert.DeserializeObject<StoreApiObject>(s);
                    foreach (var item in PlayerDataManager.StoreData.cosmetics)
                    {
                        Utilities.SetCatagoryApparel(item);
                    }
                }
                else
                {
                    Debug.LogError("Failed to get the store Object : " + s);
                }
            });
            PlayerManagerUI.Instance.SetupUI();
            ConditionsManager.SetupConditions();
            MapView.Initialize();
            OnCharacterInitialized?.Invoke();
            Raincrow.Analytics.AnalyticsAPI.Instance.InitSession();
        }
    }

    static void GetNewTokens()
    {
        APIManager.Instance.GetDataRC("refresh-tokens", (string s, int r) =>
        {
            Debug.Log(s);
            if (r == 200)
            {
                var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(s);
                wssToken = data.wsToken;
                loginToken = data.token;
                Debug.Log("Reseting WSS");
                WebSocketClient.Instance.InitiateWSSCOnnection(true);
            }
        });
    }

    public static void GetCharacter(System.Action<string, int> callback)
    {
        //todo: initialize/update the local player here instead of the current callbacks (OnGetCharcterInitResponse, OnGetCharcterResponse)
        APIManager.Instance.GetData("character/get", (response, result) =>
        {
            callback?.Invoke(response, result);
        });
    }

    static void OnGetCharcterResponse(string result, int response)
    {
        Debug.Log(result);
        if (Application.isEditor)
        {
            TextEditor te = new TextEditor();
            te.text = result;
            te.SelectAll();
            te.Copy();
        }
        if (response == 200)
        {
            rawData = JsonConvert.DeserializeObject<PlayerDataDetail>(result);

            if (!sceneLoaded)
                StartUpManager.Instance.ShowTribunalTimer();
            else
            {
                InitiliazingPostLogin();
            }
            loggedIn = true;
        }
        else
        {
            if (!sceneLoaded)
            {
                loggedIn = false;
                StartUpManager.Instance.DoSceneLoading();
            }
            else
            {
                LoginUIManager.Instance.initiateLogin();
            }
            //   Debug.LogError(result);
        }

        if (LoginUIManager.Instance)
            LoginUIManager.Instance.EnableCanvasGroup(true);


        StoreManagerAPI.GetShopItems((string s, int r) =>
   {
       if (r == 200)
       {
           // Debug.Log(s);
           PlayerDataManager.StoreData = JsonConvert.DeserializeObject<StoreApiObject>(s);
           foreach (var item in PlayerDataManager.StoreData.cosmetics)
           {
               Utilities.SetCatagoryApparel(item);
           }
       }
       else
       {
           Debug.LogError("Failed to get the store Object : " + s);
       }
   });

    }

    public static void InitiliazingPostLogin()
    {
        PlayerDataManager.playerData = DictifyData(rawData);
        //PlayerDataManager.currentDominion = PlayerDataManager.config.dominion;
        //Debug.Log(PlayerDataManager.currentDominion);
        //ChatConnectionManager.Instance.InitChat();
        Raincrow.Chat.ChatManager.InitChat(new Raincrow.Chat.ChatPlayer
        {
            id = PlayerDataManager.playerData.instance,
            degree = PlayerDataManager.playerData.degree,
            level = PlayerDataManager.playerData.level,
            name = PlayerDataManager.playerData.displayName,
            avatar = PlayerDataManager.playerData.avatar,
        });
        LoginUIManager.Instance.mainUI.SetActive(true);
        ApparelManager.instance.SetupApparel();
        LoginUIManager.Instance.CorrectPassword();
        PushManager.InitPush();
        //		SettingsManager.Instance.FbLoginSetup ();
        CovenController.Load();

        QuestsController.GetQuests(null);
        if (FTFComplete)
        {
            APIManager.Instance.GetData("/location/leave", (string s, int r) =>
            {
                MarkerManagerAPI.GetMarkers(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, false, null, false, false, true);
            });
        }
        if (PlayerDataManager.playerData.dailyBlessing && FTFComplete)
        {
            if (PlayerDataManager.playerData.blessing.lunar > 0)
                MoonManager.Instance.SetupSavannaEnergy(true, PlayerDataManager.playerData.blessing.lunar);
            else
                MoonManager.Instance.SetupSavannaEnergy(false, PlayerDataManager.playerData.blessing.lunar);
            PlayerManagerUI.Instance.ShowBlessing();
        }
        else
        {
            if (!isNewAccount && FTFComplete)
            {
                MoonManager.Instance.Open();
                MoonManager.Instance.SetupSavannaEnergy(false);
            }
        }

        ConditionsManager.SetupConditions();
        MapView.Initialize();
        OnCharacterInitialized?.Invoke();
        Raincrow.Analytics.AnalyticsAPI.Instance.InitSession();
    }


    public static PlayerDataDetail DictifyData(PlayerDataDetail data)
    {
        ///tempfix
        ///I'm not sure the server is setting the characters longitude and latitude when the player crates a new character,
        ///so this makes sure its not empty
        if (data.longitude == 0)
            data.longitude = MapsAPI.Instance.physicalPosition.x;
        if (data.latitude == 0)
            data.latitude = MapsAPI.Instance.physicalPosition.y;

        if (data.coven == "")
            data.covenName = "";

        if (data.ingredients.gems != null)
        {
            foreach (var item in data.ingredients.gems)
            {
                if (!DownloadedAssets.ingredientDictData.ContainsKey(item.id))
                {
                    Debug.LogError(item.id);
                    continue;
                }
                item.name = DownloadedAssets.ingredientDictData[item.id].name;
                item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
                data.ingredients.gemsDict[item.id] = item;
            }
        }

        if (data.ingredients.tools != null)
        {

            foreach (var item in data.ingredients.tools)
            {
                if (!DownloadedAssets.ingredientDictData.ContainsKey(item.id))
                {
                    Debug.LogError(item.id);
                    continue;
                }

                item.name = DownloadedAssets.ingredientDictData[item.id].name;
                item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
                data.ingredients.toolsDict[item.id] = item;

            }
        }
        if (data.ingredients.herbs != null)
        {
            foreach (var item in data.ingredients.herbs)
            {
                if (!DownloadedAssets.ingredientDictData.ContainsKey(item.id))
                {
                    Debug.LogError(item.id);
                    continue;
                }

                item.name = DownloadedAssets.ingredientDictData[item.id].name;
                item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
                data.ingredients.herbsDict[item.id] = item;

            }
        }

        try
        {
            foreach (var item in data.spells)
            {
                if (item.id == "spell_magicDance" || item.id == "spell_confusion" || item.id == "spell_wail" || item.id == "spell_leech")
                {
                    continue;
                }
                item.school = DownloadedAssets.spellDictData[item.id].spellSchool;
                item.displayName = DownloadedAssets.spellDictData[item.id].spellName;
                item.description = DownloadedAssets.spellDictData[item.id].spellDescription;
                item.lore = DownloadedAssets.spellDictData[item.id].spellLore;
                //data.spellsDict.Add(item.id, item);
                item.herb = item.tool = item.gem = "";
                foreach (var ing in item.ingredients)
                {
                    if (DownloadedAssets.ingredientDictData[ing].type == "herb")
                    {
                        item.herb = ing;
                    }
                    else if (DownloadedAssets.ingredientDictData[ing].type == "gem")
                    {
                        item.gem = ing;
                    }
                    else
                    {
                        item.tool = ing;
                    }
                }

                PlayerDataManager.spells[item.id] = item;


            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        //try
        //{
        //    foreach (var item in data.cooldownList)
        //    {
        //        data.cooldownDict[item.instance] = item;
        //    }
        //}
        //catch
        //{
        //    // nothing to cooldown
        //}

        foreach (var item in data.inventory.cosmetics)
        {
            Utilities.SetCatagoryApparel(item);
        }

        foreach (var item in data.knownSpirits)
        {
            data.knownSpiritsDict.Add(item.id, item);
            //data.KnownSpiritsList.Add(item.id);
        }
        return data;
    }

    public static void CreateAccount(string Username, string Password, string Email)
    {
        isNewAccount = true;
        systemLanguage = Application.systemLanguage.ToString();
        var data = new PlayerLoginAPI();
        data.username = Username;
        data.password = Password;
        data.email = Email;
        data.game = "covens";
        data.language = systemLanguage;
        data.latitude = MapsAPI.Instance.physicalPosition.y;
        data.longitude = MapsAPI.Instance.physicalPosition.x;
        username = Username;
        data.UID = SystemInfo.deviceUniqueIdentifier;

        APIManager.Instance.Put("create-account", JsonConvert.SerializeObject(data), CreateAccountCallback, false, false);
    }

    static void CreateAccountCallback(string result, int status)
    {
        if (status == 200)
        {
            Debug.Log("Account Created");
            AppsFlyerAPI.CreatedAccount();
            //			Debug.Log (result);
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;
            SetupConfig(data.config);
            OnLoginSuccess();
        }
        else
        {
            //   Debug.LogError(result);
            //	LoginUIManager.Instance.WrongPassword ();	handle result
            if (result == "4103" || result == "4301")
            {
                LoginUIManager.Instance.CreateAccountResponse(false, "Username is in already taken.");
            }
            else if (result == "4104")
            {
                LoginUIManager.Instance.CreateAccountResponse(false, "Username is invalid.");
            }
            else if (result == "4201")
            {
                LoginUIManager.Instance.CreateAccountResponse(false, "Session has expired.");
            }
            else if (result == "4107")
            {
                LoginUIManager.Instance.CreateAccountResponse(false, "Invalid email address.");
            }

            else
            {
                LoginUIManager.Instance.CreateAccountResponse(false, "Something went wrong. Error code : " + result);
            }
        }
    }

    public static void CreateCharacter(string charSelect)
    {
        LoginUIManager.Instance.EnableCanvasGroup(false);
        //  Debug.Log("Creating Character");
        var data = new PlayerCharacterCreateAPI();
        data.displayName = LoginUIManager.charUserName;
        data.latitude = MapsAPI.Instance.physicalPosition.y;
        data.longitude = MapsAPI.Instance.physicalPosition.x;
        data.male = (charSelect.Contains("female") ? false : true);
        data.characterSelection = charSelect;
        username = LoginUIManager.charUserName;
        APIManager.Instance.Put("create-character", JsonConvert.SerializeObject(data), (s, i) => CreateCharacterCallback(charSelect, s, i), true, false);
    }

    public static int tryCount = 0;
    static void CreateCharacterCallback(string name, string result, int status)
    {
        if (status == 200)
        {
            Debug.Log("Creating Character Success");
            AppsFlyerAPI.CreatedAvatar();
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            GetCharacter(OnGetCharcterResponse);
        }
        else
        {
            if (tryCount >= 4)
                AutoLogin();

            if (result == "4103") //Character was already created
            {
                // Debug.LogError("CREATE CHARACTER ERROR 4103");
                AutoLogin();
            }
            else
            {
                //Debug.LogError("CreateCharacter error: " + status + ".\t" + result);

                //try again
                tryCount++;
                CreateCharacter(name);
            }
        }
    }

#region Password Reset

    public static void ResetPasswordRequest(string Username)
    {
        var data = new PlayerResetAPI();
        data.username = Username;
        username = Username;
        Action<string, int> callback;
        callback = ResetPasswordRequestCallback;
        APIManager.Instance.Post("request-reset", JsonConvert.SerializeObject(data), callback, false, false);
    }

    static void ResetPasswordRequestCallback(string result, int status)
    {
        //  Debug.Log(result);

        if (status == 200)
        {
            string email = JsonConvert.DeserializeObject<PlayerResetCallback>(result).email;
            LoginUIManager.Instance.EnterResetCode(email);
        }
        else
        {
            if (result == "4102")
            {
                LoginUIManager.Instance.EmailNull();
            }
            else if (result == "4101")
            {
                LoginUIManager.Instance.resetUserNull();
            }
            else if (result == "4103")
            {
                LoginUIManager.Instance.resetUserNull();
            }
            else if (result == "4104")
            {
                LoginUIManager.Instance.resetUserNull();
            }
        }
    }

    public static void SendResetCode(string code)
    {

        var data = new PlayerResetAPI();
        data.code = code;
        data.username = username;

        Action<string, int> callback;
        callback = SendResetCodeCallback;
        APIManager.Instance.Post("request-reset", JsonConvert.SerializeObject(data), callback, false, false);
    }

    static void SendResetCodeCallback(string result, int status)
    {
        // Debug.Log(result);

        if (status == 200)
        {

            token = JsonConvert.DeserializeObject<PlayerPasswordCallback>(result).token;
            //       Debug.Log(token);
            LoginUIManager.Instance.FinishPasswordReset();
        }
        else
        {
            LoginUIManager.Instance.ResetCodeWrong();
        }
    }

    public static void SendNewPassword(string password)
    {
        //    Debug.Log("Sending New Password");
        var data = new PlayerResetAPI();
        data.password = password;
        data.token = token;
        data.username = username;

        Action<string, int> callback;
        callback = SendNewPasswordCallback;
        APIManager.Instance.Post("reset-password", JsonConvert.SerializeObject(data), callback, false, false);
    }

    static void SendNewPasswordCallback(string result, int status)
    {
        //  Debug.Log(result);
        if (status == 200)
        {
            LoginUIManager.Instance.PostPasswordReset(username, password);
        }
        else
        {
            if (result == "4200")
            {
                LoginUIManager.Instance.PasswordTokenError("Session has expired");
            }
        }
    }

#endregion

    private static void OnLoginSuccess()
    {
    }
}