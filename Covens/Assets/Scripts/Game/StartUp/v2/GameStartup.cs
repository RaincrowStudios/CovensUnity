using Raincrow;
using Raincrow.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public class GameStartup : MonoBehaviour
{
    private int m_CurrentFileIndex;
    private float m_CurrentFileSize;
    private int m_FilesAmount;

    private bool m_DictionaryReady;
    private bool m_DownloadsReady;
    private bool m_LogosReady;
    private bool m_LoginReady;
    private bool m_ConfigReady;

    public static string Dominion { get; private set; }
    public static string TopPlayer { get; private set; }
    public static string TopCoven { get; private set; }

    private void OnEnable()
    {
        DownloadManager.OnServerError += OnServerError;
        DownloadManager.OnMaintenance += OnMaintenance;
        DownloadManager.OnVersionOutdated += OnVersionOutdated;

        DownloadManager.OnDictionaryDownloadStart += OnDictionaryStart;
        DownloadManager.OnDownloadedDictionary += OnDictionaryReady;
        DownloadManager.OnDictionaryError += OnDictionaryError;
        DownloadManager.OnDictionaryParserError += OnDictionaryParseError;

        DictionaryManager.OnDownloadProgress += OnDictionaryDownloadProgress;

        DownloadManager.OnDownloadStart += OnAssetDownloadStart;
        DownloadManager.OnDownloadProgress += OnAssetDownloadProgress;
        DownloadManager.OnDownloadError += OnAssetDownloadError;
        DownloadManager.OnDownloadFinish += OnAssetDownloadFinish;
        DownloadManager.OnDownloadsComplete += OnAllDownloadsCompleted;
    }

    private void OnDisable()
    {
        DownloadManager.OnServerError -= OnServerError;
        DownloadManager.OnMaintenance -= OnMaintenance;
        DownloadManager.OnVersionOutdated -= OnVersionOutdated;

        DownloadManager.OnDictionaryDownloadStart -= OnDictionaryStart;
        DownloadManager.OnDownloadedDictionary -= OnDictionaryReady;
        DownloadManager.OnDictionaryError -= OnDictionaryError;
        DownloadManager.OnDictionaryParserError -= OnDictionaryParseError;

        DictionaryManager.OnDownloadProgress += OnDictionaryDownloadProgress;

        DownloadManager.OnDownloadStart -= OnAssetDownloadStart;
        DownloadManager.OnDownloadProgress -= OnAssetDownloadProgress;
        DownloadManager.OnDownloadError -= OnAssetDownloadError;
        DownloadManager.OnDownloadFinish -= OnAssetDownloadFinish;
        DownloadManager.OnDownloadsComplete -= OnAllDownloadsCompleted;
    }

    void Awake()
    {
        var t = PlayerManager.SystemLanguage;

        for (int i = 0; i < DictionaryManager.Languages.Length; i++)
        {
            if (DictionaryManager.Languages[i] == t)
            {
                DictionaryManager.languageIndex = i;
                return;
            }
        }

        LeanTween.init(0);
    }

    private void Start()
    {
        SettingsManager.LoadSettings();

        //Setting up AppsFlyerStuff
        AppsFlyer.setAppsFlyerKey("Wdx4jw7TTNEEJYUh5UnaDB");
#if UNITY_IOS
        {
            AppsFlyer.setAppID("com.raincrow.covens");
            AppsFlyer.trackAppLaunch();
        }
#elif UNITY_ANDROID
        {
            AppsFlyer.setAppID("com.raincrow.covens");
            AppsFlyer.init("Wdx4jw7TTNEEJYUh5UnaDB", "AppsFlyerTrackerCallbacks");
        }
#endif
        //show loading screen
        SplashManager.Instance.ShowLoading(0);

        //wait for the gps/network
        GetGPS.OnInitialized += OnGPSReady;

        MapsAPI.Instance.InstantiateMap();
    }

    private void OnGPSReady()
    {
        SplashManager.Instance.ShowLoading(1);
        GetGPS.OnInitialized -= OnGPSReady;

        //start downloading the assets
        DownloadManager.DownloadAssets(TryAutoLogin);

        //show the initial logos
        m_LogosReady = false;
        SplashManager.Instance.ShowLogos(OnSplashLogosFinished);
    }

    private void OnServerError(int responseCode, string response)
    {
        bool isRelease = false;

#if UNITY_EDITOR
        isRelease = UnityEditor.EditorPrefs.GetString("game") == "Release";
#elif PRODUCTION
        isRelease = true;
#endif
        HandleServerDown.Instance.ShowServerDown(isRelease);
    }

    private void OnMaintenance()
    {
        HandleServerDown.Instance.ShowMaintenance();
    }

    private void OnVersionOutdated()
    {
        SplashManager.Instance.OutDatedBuild();
    }



    private void OnDictionaryStart()
    {
        m_DictionaryReady = false;
    }

    private void OnDictionaryReady()
    {
        m_DictionaryReady = true;
        DownloadAssetBundle.isDictLoaded = true;
    }

    private void OnDictionaryError(string error)
    {
        HandleServerDown.Instance.ShowErrorDictionary();
        throw new System.Exception(error);
    }

    private void OnDictionaryParseError(string error, string stackTrace)
    {
        HandleServerDown.Instance.ShowErrorParseDictionary();
        throw new System.Exception(error + "\n" + stackTrace);
    }

    private void OnDictionaryDownloadProgress(string name, float size, float progress)
    {
        name = (LocalizeLookUp.HasKey("chat_settings") ? LocalizeLookUp.GetText("chat_settings") : "settings") + " (" + name + ")";
        SplashManager.Instance.SetDownloadProgress(name, 0, 0, size, progress);
    }


    private void OnAssetDownloadStart(string name, int index, int total, float fileSize)
    {
        m_DownloadsReady = false;
        m_CurrentFileIndex = index;
        m_CurrentFileSize = fileSize;
        m_FilesAmount = total;

        SplashManager.Instance.SetDownloadProgress(name, index, total, fileSize, 0);
    }

    private void OnAssetDownloadProgress(string name, float progress, float fileSize)
    {
        SplashManager.Instance.SetDownloadProgress(name, m_CurrentFileIndex, m_FilesAmount, m_CurrentFileSize, progress);
    }

    private void OnAssetDownloadFinish(string name)
    {
        SplashManager.Instance.SetDownloadProgress(name, m_CurrentFileIndex, m_FilesAmount, m_CurrentFileSize, 1);
    }

    private void OnAssetDownloadError(string name, string error)
    {
        HandleServerDown.Instance.AssetDownloadError(name);
    }

    private void OnAllDownloadsCompleted()
    {
        SplashManager.Instance.ShowDownloadSlider(false);
        SplashManager.Instance.SetDownloadMessage("", "");

        m_DownloadsReady = true;
        CompleteStartup();
    }

    private void OnSplashLogosFinished()
    {
        m_LogosReady = true;
        CompleteStartup();
    }

    private void TryAutoLogin()
    {
        m_ConfigReady = false;
        m_LoginReady = false;

        //try auto login
        LoginAPIManager.Login((loginResult, loginResponse) =>
        {
            if (loginResult == 200 && (loginResponse.hasCharacter.HasValue == false || loginResponse.hasCharacter.Value == true))
            {
                //the player is logged in, get the character
                LoginAPIManager.GetCharacter((charResult, charResponse) =>
                {
                    m_LoginReady = true;
                    CompleteStartup();
                });
            }
            else
            {
                //the login failed
                m_LoginReady = true;
                CompleteStartup();
            }
        });
    }

    private void CompleteStartup()
    {
        //wait for logos finish 
        if (m_LogosReady == false)
        {
            return;
        }
        else
        {

            if (SplashManager.Instance.IsShowingHints == false)
                SplashManager.Instance.ShowHints(null);

            //wait for downloads to finish
            if (m_DownloadsReady == false)
                return;

            //wait for the auto login result
            if (m_LoginReady == false)
                return;
        }

        if (LoginAPIManager.characterLoggedIn)
        {
            StartGame();
        }
        else
        {
            SplashManager.Instance.HideHints(1, null);
            LoginAPIManager.OnCharacterReceived += StartGame;
            LoginUIManager.Screen startingScreen;

            if (LoginAPIManager.accountLoggedIn)
                startingScreen = LoginUIManager.Screen.CHOOSE_CHARACTER;
            else
                startingScreen = LoginUIManager.Screen.WELCOME;

            if (SplashManager.Instance.IsShowingHints)
            {
                SplashManager.Instance.HideHints(1, () => LoginUIManager.Open(startingScreen));
            }
            else
            {
                LoginUIManager.Open(startingScreen);
            }
        }
    }

    private void StartGame()
    {
        if (m_ConfigReady == false)
        {
            SplashManager.Instance.ShowLoading(0);
            LoginAPIManager.GetConfigurations(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, (config, error) =>
            {
                SplashManager.Instance.ShowLoading(1);

                m_ConfigReady = true;
                if (string.IsNullOrEmpty(error) == false)
                    Debug.LogError("GetConfig failed\n" + error);

                Dominion = string.IsNullOrEmpty(config.dominion) ? "Ronin" : config.dominion;
                TopPlayer = config.dominionRank.topPlayer;
                TopCoven = config.dominionRank.topCoven;

                StartGame();
            });
            return;
        }

        CrashReportHandler.SetUserMetadata("character", PlayerDataManager.playerData.name);
        LoginAPIManager.OnCharacterReceived -= StartGame;

        //show the tribunal screen and load the gamescene
        SplashManager.Instance.ShowTribunal(() =>
        {
            UINearbyLocations.GetLocations(null, false);

            Debug.Log("Initializing the map at lat" + PlayerDataManager.playerData.latitude + " lon" + PlayerDataManager.playerData.longitude);
            MapsAPI.Instance.InitMap(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, 1, null, false);

            Debug.Log("Loading the game scene");
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.GAME,
                UnityEngine.SceneManagement.LoadSceneMode.Single,
                (progress) => SplashManager.Instance.ShowLoading(progress),
                OnGameSceneLoaded);
        });
    }

    private void OnGameSceneLoaded()
    {
        //SplashManager.Instance.HideTribunal(() => SceneManager.UnloadScene(SceneManager.Scene.START, null, null));

        if (PlayerDataManager.IsFTF)
        {
            FTFManager.OnFinishFTF += OnFinishFTF;
            FTFManager.StartFTF();
        }
        else
        {
            SocketClient.Instance.InitiateSocketConnection();
            ChatManager.InitChat();
            
            if (PlayerDataManager.playerData.insidePlaceOfPower && PlayerDataManager.playerData.placeOfPower != "")
            {
                LocationIslandController.ResumeBattle(PlayerDataManager.playerData.placeOfPower);
            }
            else
            {
                UIDominionSplash.Instance.Show(() =>
                {
                    if (Raincrow.FTF.FirstTapManager.IsFirstTime("nextpoplaunch"))
                    {
                        Raincrow.FTF.FirstTapManager.Show("nextpoplaunch", BlessingManager.CheckDailyBlessing);
                    }
                    else
                    {
                        BlessingManager.CheckDailyBlessing();
                    }
                });
            }
        }
    }

    private void OnFinishFTF()
    {
        FTFManager.OnFinishFTF -= OnFinishFTF;

        SocketClient.Instance.InitiateSocketConnection();
        ChatManager.InitChat();
    }
}
