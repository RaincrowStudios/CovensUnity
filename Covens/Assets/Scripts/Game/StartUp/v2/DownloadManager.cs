﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    private const string downloadUrl = "https://storage.googleapis.com/raincrow-covens/";
    /// <summary>
    /// name
    /// index
    /// total
    /// </summary>
    public static event System.Action<string, int, int, float> OnDownloadStart;
    /// <summary>
    /// name, progress
    /// </summary>
    public static event System.Action<string, float, float> OnDownloadProgress;
    /// <summary>
    /// name
    /// </summary>
    public static event System.Action<string> OnDownloadFinish;
    /// <summary>
    /// name, message
    /// </summary>
    public static event System.Action<string, string> OnDownloadError;
    /// <summary>
    /// 
    /// </summary>
    public static event System.Action OnDownloadsComplete;

    /// <summary>
    /// 
    /// </summary>
    public static event System.Action OnDictionaryDownloadStart;
    /// <summary>
    /// 
    /// </summary>
    public static event System.Action OnDownloadedDictionary;
    /// <summary>
    /// error
    /// </summary>
    public static event System.Action<string> OnDictionaryError;
    /// <summary>
    /// error, stacktrace
    /// </summary>
    public static event System.Action<string, string> OnDictionaryParserError;

    /// <summary>
    /// 
    /// </summary>
    public static event System.Action OnVersionOutdated;
    /// <summary>
    /// 
    /// </summary>
    public static event System.Action OnMaintenance;
    /// <summary>
    /// responseCode, response
    /// </summary>
    public static event System.Action<int, string> OnServerError;



    private static DownloadManager m_Instance;
    private static DownloadManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new GameObject("DownloadManager").AddComponent<DownloadManager>();
            return m_Instance;
        }
    }

    private const int MAX_RETRIES = 3;
    private const float RETRY_COOLDOWN = 3f;

    public static void DownloadAssets(bool useBackupServer = false)
    {
        if (useBackupServer)
        {
            Debug.Log("Requesting asset list from backup server");
            DownloadAssetBundle.SetMessage("Getting asset list from backup server", "");
        }
        else
        {
            Debug.Log("Requesting asset list from server");
            DownloadAssetBundle.SetMessage("Getting asset list from server", "");
        }

        APIManagerServer.ENABLE_AUTO_RETRY = false;
        CovenConstants.isBackUpServer = useBackupServer;

        int retryCount = 0;
        System.Action getAssets = () => { };

        getAssets = () =>
        {
            var data = new { game = "covens" };
            APIManager.Instance.Post("assets", JsonConvert.SerializeObject(data), (string s, int r) =>
            {
                if (r == 200 && !string.IsNullOrEmpty(s))
                {
                    APIManagerServer.ENABLE_AUTO_RETRY = true;
                    Debug.Log("Assets to download:\n" + s);
                    var d = JsonConvert.DeserializeObject<AssetResponse>(s);
                    Instance.StartCoroutine(StartDownloads(d));
                }
                else
                {
                    if (retryCount >= MAX_RETRIES)
                    {
                        if (CovenConstants.isBackUpServer)
                        {
                            Debug.LogError("Failed to request assets from backup server.\n[" + r.ToString() + "] " + s);
                            OnServerError?.Invoke(r, s);
                        }
                        else
                        {
                            Debug.LogError("Failed to request assets.\n[" + r.ToString() + "] " + s);
                            DownloadAssets(true);
                        }
                    }
                    else
                    {

                        if (CovenConstants.isBackUpServer)
                            DownloadAssetBundle.SetMessage("Retrying connection to backup servers . . .", $"Attempt {retryCount}/{MAX_RETRIES} ");
                        else
                            DownloadAssetBundle.SetMessage("Retrying connection to servers . . .", $"Attempt {retryCount}/{MAX_RETRIES} ");

                        Debug.Log("Assets request failed. Retrying[" + retryCount + "]");
                        retryCount += 1;
                        LeanTween.value(0, 0, 1f).setOnComplete(getAssets);
                    }
                }
            }, false, false);
        };

        getAssets.Invoke();
    }

    private static IEnumerator StartDownloads(AssetResponse assets)
    {
        //check if server is under maintenance
        if (assets.maintenance)
        {
            OnMaintenance?.Invoke();
            yield break;
        }

        //setup the AppVersion
        int appVersion = 0;
        if (Application.platform == RuntimePlatform.Android)
            appVersion = assets.android;
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            appVersion = assets.apple;

        DownloadedAssets.AppVersion = string.Concat(appVersion, ".", assets.version);

        if (Debug.isDebugBuild)
            DownloadedAssets.AppVersion += ".DEV";

#if !PRODUCTION
        DownloadedAssets.AppVersion += "-STAGING";
#endif

        if (appVersion > int.Parse(Application.version))
        {
            OnVersionOutdated?.Invoke();
            yield break;
        }
                



        //download the dictionary
        OnDictionaryDownloadStart?.Invoke();

        bool isDictionaryComplete = false;
        bool isDictionaryParseError = false;
        string dictionaryDownloadError = null;

        DictionaryManager.GetDictionary(assets.dictionary,
            onDicionaryReady: () => 
            {
                isDictionaryComplete = true;
            },
            onDownloadError: (code, response) =>
            {
                isDictionaryComplete = true;
                dictionaryDownloadError = $"Error downloading dictionary. [{code}] {response}";
            },
            onParseError: () =>
            {
                isDictionaryComplete = true;
                isDictionaryParseError = true;
            });

        while (isDictionaryComplete == false)
            yield return 0;

        if (string.IsNullOrEmpty(dictionaryDownloadError) == false)
        {
            OnDictionaryError?.Invoke(dictionaryDownloadError);
            yield break;
        }

        if (isDictionaryParseError)
        {
            //error delegate was already invoked in SaveDict method
            //OnDictionaryParserError?.Invoke();
            yield break;
        }

        OnDownloadedDictionary?.Invoke();




        //download the asset bundles
        List<string> bundlesToDownload = new List<string>();
        AssetCacheJson cachedAssetKeys;

        if (PlayerPrefs.HasKey("AssetCacheJson"))
        {
            cachedAssetKeys = JsonConvert.DeserializeObject<AssetCacheJson>(PlayerPrefs.GetString("AssetCacheJson"));
            foreach(string _key in assets.assets)
            {
                //check if its not in the assetcache
                if (cachedAssetKeys.bundles.Contains(_key) == false)
                {
                    bundlesToDownload.Add(_key);
                }
                //check if the file is in the persistendatapath
                else
                {
                    string assetPath = Path.Combine(Application.persistentDataPath, _key + ".unity3d");
                    if (System.IO.File.Exists(assetPath) == false)
                    {
                        bundlesToDownload.Add(_key);
                    }
                }
            }
        }
        else
        {
            cachedAssetKeys = new AssetCacheJson
            {
                bundles = new List<string>()
            };
            bundlesToDownload = assets.assets;
        }


        string assetBaseUrl = DownloadManager.downloadUrl;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            assetBaseUrl += "appleassets/";

        for (int i = 0; i < bundlesToDownload.Count; i++)
        {
            string assetName = bundlesToDownload[i];
            float size = 0;

            //get the header
            bool fail = true;
            int retryCount = 0;
            UnityWebRequest head = UnityWebRequest.Head(assetBaseUrl + assetName);
            while (fail && retryCount < MAX_RETRIES)
            {
                head.SendWebRequest();

                while (!head.isDone)
                    yield return 0;

                fail = head.isNetworkError || head.isHttpError;
                retryCount += 1;
                
                if (fail)
                {
                    APIManager.ThrowRetryError(head, head.url, "");
                    yield return new WaitForSeconds(RETRY_COOLDOWN);
                    head = UnityWebRequest.Head(assetBaseUrl + assetName);
                }
            }

            if (fail)
            {
                Debug.LogError("Failed to download header " + assetName + "\n" + head.error);
                OnDownloadError?.Invoke(assetName, head.error);
                yield break;
            }
            else
            {
                size = float.Parse(head.GetResponseHeader("Content-Length")) * 0.000001f;
            }
            
            //get the file
            fail = true;
            retryCount = 0;
            UnityWebRequest www = UnityWebRequest.Get(assetBaseUrl + assetName);
            while (fail && retryCount < MAX_RETRIES)
            {
                www.SendWebRequest();
                OnDownloadStart?.Invoke(assetName, i + 1, bundlesToDownload.Count, size);
                while (!www.isDone)
                {
                    OnDownloadProgress?.Invoke(assetName, www.downloadProgress, size);
                    yield return 0;
                }
                fail = www.isNetworkError || www.isHttpError;
                retryCount += 1;

                if (fail)
                {
                    APIManager.ThrowRetryError(www, www.url, "");
                    yield return new WaitForSeconds(RETRY_COOLDOWN);
                    www = UnityWebRequest.Get(assetBaseUrl + assetName);
                }
            }

            if (fail)
            {
                Debug.LogError("Failed to download " + assetName + "\n" + www.error);
                OnDownloadError?.Invoke(assetName, www.error);
                yield break;
            }
            else
            {
                //save the file to disk
                string filePath = Path.Combine(Application.persistentDataPath, assetName + ".unity3d");
                File.WriteAllBytes(filePath, www.downloadHandler.data);

                //mark it as downloaded in the playerprefs
                if (!cachedAssetKeys.bundles.Contains(assetName))
                    cachedAssetKeys.bundles.Add(assetName);
                PlayerPrefs.SetString("AssetCacheJson", JsonConvert.SerializeObject(cachedAssetKeys));
                
                Debug.Log("Downloaded " + assetName);
                OnDownloadFinish?.Invoke(assetName);
                yield return 0;
            }
        }

        foreach (string key in assets.assets)
        {
            DownloadedAssets.LoadAsset(key);
        }

        OnDownloadsComplete?.Invoke();
    }

    public static bool SaveDict(string version, string json)
    {
        try
        {
            DictMatrixData data = Newtonsoft.Json.JsonConvert.DeserializeObject<DictMatrixData>(json);

            DownloadedAssets.spellDictData = data.Spells;
            DownloadedAssets.spellFeedbackDictData = data.SpellFeedback;
            WitchSchoolManager.witchVideos = data.WitchSchool;

            foreach (var item in data.Zone)
                DownloadedAssets.zonesIDS[int.Parse(item.Key)] = item.Value.value;

            DownloadedAssets.localizedText = data.Other;
            DownloadedAssets.gardenDict = data.Gardens;
            DownloadedAssets.spiritTypeDict = data.SpiritTypes;
            DownloadedAssets.storeDict = data.Store;
            DownloadedAssets.ingredientDictData = data.Collectibles;
            DownloadedAssets.conditionsDictData = data.Conditions;
            DownloadedAssets.spiritDictData = data.Spirits;
            DownloadedAssets.questsDict = data.Quest;
            DownloadedAssets.countryCodesDict = data.CountryCodes;

            foreach (var item in data.FTFDialogues)
                DownloadedAssets.ftfDialogues.Add(item.value);

            DownloadedAssets.ftfDialogues.Add("");     // its need one empty string at the end of array
            DownloadedAssets.tips = data.LoadingTips;
            LocalizationManager.CallChangeLanguage(version, false);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse dictionary: " + e.Message + "\n" + e.StackTrace);
            OnDictionaryParserError?.Invoke(e.Message, e.StackTrace);
            return false;
        }
    }
}
