using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    private int m_CurrentFileIndex;
    private float m_CurrentFileSize;
    private int m_FilesAmount;

    private bool m_DictionaryReady;
    private bool m_DownloadsReady;
    private bool m_LogosReady;
    private bool m_LoginReady;
    private bool m_GameConfigReady;

    private void OnEnable()
    {
        DownloadManager.OnServerError += OnServerError;
        DownloadManager.OnMaintenance += OnMaintenance;
        DownloadManager.OnVersionOutdated += OnVersionOutdated;

        DownloadManager.OnDictionaryDownloadStart += OnDictionaryStart;
        DownloadManager.OnDownloadedDictionary += OnDictionaryReady;
        DownloadManager.OnDictionaryError += OnDictionaryError;
        DownloadManager.OnDictionaryParserError += OnDictionaryParseError;

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

        DownloadManager.OnDownloadStart -= OnAssetDownloadStart;
        DownloadManager.OnDownloadProgress -= OnAssetDownloadProgress;
        DownloadManager.OnDownloadError -= OnAssetDownloadError;
        DownloadManager.OnDownloadFinish -= OnAssetDownloadFinish;
        DownloadManager.OnDownloadsComplete -= OnAllDownloadsCompleted;
    }

    void Awake()
    {
        // if (Application.isEditor) return;
        var t = Application.systemLanguage.ToString();

        for (int i = 0; i < DictionaryManager.Languages.Length; i++)
        {
            if (DictionaryManager.Languages[i] == t)
            {
                DictionaryManager.languageIndex = i;
                return;
            }
        }
        DictionaryManager.languageIndex = 0;
    }

    private void Start()
    {
        //Setting up AppsFlyerStuff
        AppsFlyer.setAppsFlyerKey("Wdx4jw7TTNEEJYUh5UnaDB");
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AppsFlyer.setAppID("com.raincrow.covens");
            AppsFlyer.trackAppLaunch();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AppsFlyer.setAppID("com.raincrow.covens");
            AppsFlyer.init("Wdx4jw7TTNEEJYUh5UnaDB", "AppsFlyerTrackerCallbacks");
        }
        
        //wait for the gps/network
        GetGPS.OnInitialized += OnGPSReady;

        Debug.Log("Instantiating the musk map");
        MapsAPI.Instance.InstantiateMap();
    }

    private void OnGPSReady()
    {
        GetGPS.OnInitialized -= OnGPSReady;

        //start downloading the assets
        DownloadManager.DownloadAssets();

        //show the initial logos
        m_LogosReady = false;
        SplashManager.Instance.ShowLogos(OnSplashLogosFinished);

        //try to login
        TryAutoLogin();

        //get tribunal/sun/moon data
        GetGameConfigurations();
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
    }

    private void OnDictionaryParseError(string error, string stackTrace)
    {
        HandleServerDown.Instance.ShowErrorParseDictionary();
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
        SplashManager.Instance.SetDownloadProgress(name, m_CurrentFileIndex, m_FilesAmount, m_CurrentFileSize, 0);
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



    private void GetGameConfigurations()
    {
        m_GameConfigReady = false;

        LoginAPIManager.GetConfigurations(
            GetGPS.longitude,
            GetGPS.latitude,
            (result, response) =>
            {
                m_GameConfigReady = true;
            });
    }

    private void OnSplashLogosFinished()
    {
        m_LogosReady = true;
        CompleteStartup();
    }

    private void TryAutoLogin()
    {
        m_LoginReady = false;

        //try auto login
        LoginAPIManager.Login((loginResult, loginResponse) =>
        {
            if (loginResult == 200)
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
        if (m_LogosReady == false)
        {
            return;
        }
        else
        {
            //show hints
            if (SplashManager.Instance.IsShowingHints == false)
                SplashManager.Instance.ShowHints(null);

            if (m_LoginReady == false)
                return;
            if (m_DownloadsReady == false)
                return;
        }

        if (LoginAPIManager.characterLoggedIn)
        {
            //the character is ready, go to game
            StartGame();
        }
        else
        {
            LoginAPIManager.OnCharacterReceived += StartGame;
            LoginUIManager.Screen startingScreen;

            if (LoginAPIManager.accountLoggedIn)
                startingScreen = LoginUIManager.Screen.CHOOSE_CHARACTER;
            else
                startingScreen = LoginUIManager.Screen.WELCOME;
            
            if (SplashManager.Instance.IsShowingHints)
            {
                SplashManager.Instance.HideHints(() => LoginUIManager.Open(startingScreen));
            }
            else
            {
                LoginUIManager.Open(startingScreen);
            }
        }
    }

    private void StartGame()
    {
        if (m_GameConfigReady == false)
        {
            //show hints, wait one second, try again
            if (SplashManager.Instance.IsShowingHints)
            {
                LeanTween.value(0, 0, 1).setOnComplete(StartGame);
            }
            else
            {
                SplashManager.Instance.ShowLoading(0);
                SplashManager.Instance.ShowHints(() => LeanTween.value(0, 0, 1).setOnComplete(StartGame));
            }

            return;
        }

        SocketClient.Instance.InitiateSocketConnection();
        LoginAPIManager.OnCharacterReceived -= StartGame;

        //show the tribunal screen and load the gamescene
        SplashManager.Instance.ShowTribunal(() =>
        {
            //go to tutorial or go to game
            Debug.LogError("TODO: CHECK TUTORIAL");
            Debug.LogError("TODO: SHOW DOMINION INFO");

            Debug.Log("Initializing the map at lat" + PlayerDataManager.playerData.latitude + " lon" + PlayerDataManager.playerData.longitude);
            MapsAPI.Instance.InitMap(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, 1, null, false);

            Debug.Log("Loading the game scene");
            if (Application.isEditor)
            {
                SceneManager.LoadSceneAsync(
                   (SceneManager.Scene)PlayerPrefs.GetInt("DEBUGSCENE", 2),
                   UnityEngine.SceneManagement.LoadSceneMode.Single,
                   (progress) => SplashManager.Instance.ShowLoading(progress),
                   null);
            }
            else
            {
                SceneManager.LoadSceneAsync(
                   SceneManager.Scene.GAME,
                   UnityEngine.SceneManagement.LoadSceneMode.Single,
                   (progress) => SplashManager.Instance.ShowLoading(progress),
                   null);
            }
        });
    }
}
