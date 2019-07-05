using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
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
        Debug.Log("SYSTEM LANGUAGE");
        var t = Application.systemLanguage.ToString();
        Debug.Log(t);

        for (int i = 0; i < DictionaryManager.Languages.Length; i++)
        {
            if (DictionaryManager.Languages[i] == t)
            {
                DictionaryManager.language = i;
                return;
            }
        }
        DictionaryManager.language = 0;
    }
    private void Start()
    {
        //Setting up AppsFlyerStuff
        AppsFlyer.setAppsFlyerKey("Wdx4jw7TTNEEJYUh5UnaDB");
#if UNITY_IOS
        AppsFlyer.setAppID("com.raincrow.covens");
        //above is same as the android one
        AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
        AppsFlyer.setAppID("com.raincrow.covens");
        AppsFlyer.init("Wdx4jw7TTNEEJYUh5UnaDB", "AppsFlyerTrackerCallbacks");
#endif

        //show the splash screens and hints
        StartUpManager.Instance.Init();

        //wait for the gps/network
        GetGPS.OnInitialized += OnGPSReady;
    }

    private void OnGPSReady()
    {
        DownloadManager.DownloadAssets();
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
        StartUpManager.Instance.OutDatedBuild();
#if UNITY_IPHONE
        DownloadAssetBundle.Instance.appleIcon.SetActive(true);
#elif UNITY_ANDROID
        DownloadAssetBundle.Instance.playstoreIcon.SetActive(true);
#endif
    }


    private void OnDictionaryStart()
    {
        StartCoroutine(DownloadAssetBundle.AnimateDownloadingText());
    }

    private void OnDictionaryReady()
    {
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



    private int m_CurrentFileIndex;
    private int m_FilesAmount;

    private void OnAssetDownloadStart(string name, int index, int total, float fileSize)
    {
        m_CurrentFileIndex = index;
        m_FilesAmount = total;
        DownloadAssetBundle.Instance.downloadingInfo.text = "Assets " + (index).ToString() + " out of " + total.ToString() + " (" + fileSize.ToString("F2") + "MB)";
    }

    private void OnAssetDownloadProgress(string name, float progress, float fileSize)
    {
        DownloadAssetBundle.Instance.downloadingInfo.text =
            "Downloading: " +
            (m_CurrentFileIndex).ToString() +
            " out of " +
            m_FilesAmount.ToString() +
            " (" + (progress * fileSize).ToString("F2") + "/" + fileSize.ToString("F2") + "MB)";

        DownloadAssetBundle.Instance.slider.value = progress;
    }

    private void OnAssetDownloadFinish(string name)
    {

    }

    private void OnAssetDownloadError(string name, string error)
    {
        HandleServerDown.Instance.AssetDownloadError(name);
    }

    private void OnAllDownloadsCompleted()
    {
        DownloadAssetBundle.Instance.DownloadUI.SetActive(false);
        LoginAPIManager.AutoLogin();
    }
}
