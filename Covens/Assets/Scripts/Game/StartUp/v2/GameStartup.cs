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
        if (m_LoginReady == false)
            return;
        if (m_DownloadsReady == false)
            return;
        if (m_LogosReady == false)
            return;
        
        if (LoginAPIManager.characterLoggedIn) 
        {
            //the character is ready, go to game
            StartGame();
        }
        else if (LoginAPIManager.accountLoggedIn)
        {
            //the player is logged in, but dont have a char
            //go to char creation
            LoginUIManager.Open(LoginUIManager.Screen.CHOOSE_CHARACTER);
            LoginAPIManager.OnCharacterReady += StartGame;
        }
        else
        {
            //no account, no char
            LoginUIManager.Open(LoginUIManager.Screen.WELCOME);
            LoginAPIManager.OnCharacterReady += StartGame;
        }        
    }

    private void StartGame()
    {
        SocketClient.Instance.InitiateSocketConnection();
        LoginAPIManager.OnCharacterReady -= StartGame;

        //show tribunal screen

        //show dominion

        //show hints
               
        SplashManager.Instance.ShowHints(() =>
        {
            Debug.LogError("TODO: CHECK TUTORIAL");

            //show tutorial or go to game
            Debug.Log("Initializing the map at lat" + PlayerDataManager.playerData.latitude + " lon" + PlayerDataManager.playerData.longitude);
            MapsAPI.Instance.InitMap(PlayerDataManager.playerData.longitude, PlayerDataManager.playerData.latitude, 1, null, false);

            Debug.Log("Loading the game scene");
            SceneManager.LoadSceneAsync(
               SceneManager.Scene.GAME,
               UnityEngine.SceneManagement.LoadSceneMode.Single,
               (progress) => SplashManager.Instance.ShowLoading(progress),
               () => { });
        });
    }
}
