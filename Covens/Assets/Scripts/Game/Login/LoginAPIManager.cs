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
    static string token;
    public static string username;
    static string password;
    public static string loginToken;
    public static string wssToken;
    public static bool isNewAccount = true;
    static MarkerDataDetail rawData;
    public static bool sceneLoaded = false;
    public static bool hasCharacter = false;
    public static bool FTFComplete = false;
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
    static HashSet<string> knownSP;

    void Awake()
    {
        //		PlayerPrefs.DeleteKey ("Username");
        //		PlayerPrefs.DeleteKey ("Password");
        DontDestroyOnLoad(this.gameObject);
    }

    public static void AutoLogin()
    {
        if (StoredUserName != "")
        {
            ALogin(StoredUserName, StoredUserPassword);
        }
        else
        {
            StartUpManager.Instance.DoSceneLoading();
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
    }

    static void ALoginCallback(string result, int status)
    {
        //		Debug.Log ("LoginCallBack:" + status + "  " + result);
        if (status == 200)
        {

            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;


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
        }
        else if (status == 0)
        {
            UIGlobalErrorPopup.ShowError(
                cancelAction: () => ALogin(StoredUserName, StoredUserPassword),
                txt: "Login error",
                cancelTxt: "Retry"
            );
        }
        else
        {
            StartUpManager.Instance.DoSceneLoading();
        }
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
    }

    static void LoginCallback(string result, int status)
    {
        Debug.Log("LoginCallBack:" + status + "  " + result);
        if (status == 200)
        {
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;
            SetupConfig(data.config);
            FTFComplete = data.account.ftf;
            //			loggedIn = true;
        }
        else
        {
            DownloadAssetBundle.Instance.gameObject.SetActive(false);
            LoginUIManager.Instance.WrongPassword();
            print(status + "," + result);
        }
    }

    public static void WebSocketConnected()
    {
        print("WebSocketConnected");
        if (isNewAccount)
        {
            LoginUIManager.Instance.CreateAccountResponse(true, "");
        }
        else
        {
            GetCharacter();
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
            //			PlayerDataManager.SpiritToolsDict[ item.spirit] = item.tool;
            //			PlayerDataManager.ToolsSpiritDict [item.tool] = item.spirit;
            PlayerDataManager.summonMatrixDict[item.spirit] = item;
        }
        print("Init WSS");
        WebSocketClient.Instance.InitiateWSSCOnnection();
    }

    public static void GetCharacterReInit()
    {
        APIManager.Instance.GetData("character/get", OnGetCharcterInitResponse);
    }

    public static void OnGetCharcterInitResponse(string result, int response)
    {
        if (response == 200)
        {
            if (Application.isEditor)
            {
                TextEditor te = new TextEditor();
                te.content = new GUIContent(result);
                te.SelectAll();
                te.Copy();
            }
            rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(result);
            PlayerDataManager.playerData = DictifyData(rawData);
            PlayerDataManager.currentDominion = PlayerDataManager.playerData.dominion;
            ChatConnectionManager.Instance.InitChat();
            APIManager.Instance.GetData("/location/leave", (string s, int r) =>
            {
            });
            GetQuests();
            PlayerManager.Instance.InitFinished();
            GetNewTokens();
            MarkerManagerAPI.GetMarkers(false);
            PlayerDataManager.playerData.KnownSpiritsList = knownSP;
        }
    }

    static void GetNewTokens()
    {
        APIManager.Instance.GetDataRC("refresh-tokens", (string s, int r) =>
        {
            //			print(s);
            if (r == 200)
            {
                var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(s);
                wssToken = data.wsToken;
                loginToken = data.token;
                //				print("Reseting WSS");
                WebSocketClient.Instance.InitiateWSSCOnnection(true);
            }
        });
    }

    static void GetCharacter()
    {
        print("get Character ");
        APIManager.Instance.GetData("character/get", OnGetCharcterResponse);
    }

    static void OnGetCharcterResponse(string result, int response)
    {
        if (response == 200)
        {
            //			var data = JObject.Parse(result);
            //			print ("get Character response");
            // TextEditor te = new TextEditor();
            // te.content = new GUIContent(result);
            // te.SelectAll();
            // te.Copy();
            rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(result);
            knownSP = rawData.KnownSpiritsList;
            if (!sceneLoaded)
                StartUpManager.Instance.ShowTribunalTimer();
            else
            {
                if (isNewAccount || !hasCharacter)
                {
                    WebSocketClient.websocketReady = true;
                }
                InitiliazingPostLogin();
            }
            //			DownloadAssetBundle.Instance.gameObject.SetActive (false);
            loggedIn = true;
        }
        else
        {
            //	LoginUIManager.Instance.initiateLogin ();

            if (!sceneLoaded)
            {
                loggedIn = false;
                StartUpManager.Instance.DoSceneLoading();
            }
            else
            {
                LoginUIManager.Instance.initiateLogin();
            }
            Debug.LogError(result);
        }

        if (LoginUIManager.Instance)
            LoginUIManager.Instance.EnableCanvasGroup(true);
    }

    public static void InitiliazingPostLogin()
    {
        PlayerDataManager.playerData = DictifyData(rawData);
        PlayerDataManager.currentDominion = PlayerDataManager.playerData.dominion;
        LoginUIManager.Instance.CorrectPassword();
        ChatConnectionManager.Instance.InitChat();
        ApparelManager.instance.SetupApparel();
        PushManager.InitPush();
        //		SettingsManager.Instance.FbLoginSetup ();
        CovenController.Load();
        WebSocketClient.Instance.MM = MovementManager.Instance;
        GetQuests();
        APIManager.Instance.GetData("/location/leave", (string s, int r) =>
        {
        });
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

    }



    static void GetQuests()
    {
        APIManager.Instance.GetData("daily/get",
            (string result, int response) =>
            {
                print(response);
                if (response == 200)
                {
                    QuestLogUI.currentQuests = JsonConvert.DeserializeObject<Dailies>(result);
                }
                else
                    print(result + response);
            });
    }


    public static MarkerDataDetail DictifyData(MarkerDataDetail data)
    {
        if (data.coven == "")
            data.covenName = "";
        if (data.ingredients.gems != null)
        {
            foreach (var item in data.ingredients.gems)
            {
                if (!DownloadedAssets.ingredientDictData.ContainsKey(item.id))
                {
                    print(item.id);
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
                    //				print (item.id);
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
                    print(item.id);
                    continue;
                }

                item.name = DownloadedAssets.ingredientDictData[item.id].name;
                item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
                data.ingredients.herbsDict[item.id] = item;
            }
        }
        foreach (var item in data.spells)
        {
            item.school = DownloadedAssets.spellDictData[item.id].spellSchool;
            item.displayName = DownloadedAssets.spellDictData[item.id].spellName;
            item.description = DownloadedAssets.spellDictData[item.id].spellDescription;
            item.lore = DownloadedAssets.spellDictData[item.id].spellLore;
            data.spellsDict.Add(item.id, item);
            if (item.baseSpell != item.id)
            {
                foreach (var ing in item.ingredients)
                {
                    if (DownloadedAssets.ingredientDictData[ing.id].type == "herb")
                    {
                        item.herb = ing.id;
                    }
                    else if (DownloadedAssets.ingredientDictData[ing.id].type == "gem")
                    {
                        item.gem = ing.id;
                    }
                    else
                    {
                        item.tool = ing.id;
                    }
                }
            }
        }
        try
        {
            foreach (var item in data.cooldownList)
            {
                data.cooldownDict[item.instance] = item;
            }
        }
        catch
        {
            // nothing to cooldown
        }

        foreach (var item in data.inventory.cosmetics)
        {
            Utilities.SetCatagoryApparel(item);
        }

        foreach (var item in data.conditions)
        {
            data.conditionsDict.Add(item.instance, item);
            if (item.status == "silenced")
            {
                BanishManager.isSilenced = true;
                BanishManager.silenceTimeStamp = item.expiresOn;
            }
            if (item.status == "bound")
            {
                BanishManager.isBind = true;
                BanishManager.bindTimeStamp = item.expiresOn;
                BanishManager.Instance.BindLogin();
            }
        }
        if (data.conditionsDict.Count == 0)
        {
            ConditionsManager.Instance.SetupButton(false);
        }
        else
        {
            ConditionsManager.Instance.SetupButton(true);
        }

        foreach (var item in data.knownSpirits)
        {
            data.knownSpiritsDict.Add(item.id, item);
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
            //			print ("Account Created");
            //			print (result);
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            wssToken = data.wsToken;
            SetupConfig(data.config);
        }
        else
        {
            Debug.LogError(result);
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
        print("Creating Character");
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
    static void CreateCharacterCallback(string name,string result, int status)
    {
        if (status == 200)
        {
            print("Creating Character Success");
            var data = JsonConvert.DeserializeObject<PlayerLoginCallback>(result);
            loginToken = data.token;
            GetCharacter();
        }
        else
        {
            if (tryCount >= 4)
                AutoLogin();

            if (result == "4103")
            {
                //				LoginUIManager.Instance.CreateCharacterError ();
                Debug.LogError("CREATE CHARACTER ERROR 4103");
                AutoLogin();
            }
            else
            {
                Debug.LogError("CreateCharacter error: " + status + ".\t" + result);

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
        print(result);

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
        print(result);

        if (status == 200)
        {

            token = JsonConvert.DeserializeObject<PlayerPasswordCallback>(result).token;
            print(token);
            LoginUIManager.Instance.FinishPasswordReset();
        }
        else
        {
            LoginUIManager.Instance.ResetCodeWrong();
        }
    }

    public static void SendNewPassword(string password)
    {
        print("Sending New Password");
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
        print(result);
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
}