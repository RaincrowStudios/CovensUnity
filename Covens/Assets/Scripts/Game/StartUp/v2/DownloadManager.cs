using BestHTTP;
using Newtonsoft.Json;
using Raincrow.Store;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadManager : MonoBehaviour
{
    public struct AssetResponse
    {
        public string game;// => "136";
        public string store;// => "125";
        public string localization;// => "130";

        public List<string> assets;// => new List<string>() { "spirits-4", "spells-4", "apparel-8", "apparel-9", "icon-13", "icon-14" };
        public string version;
        public int android;
        public int apple;
        public bool maintenance;
    }

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

    public static bool DictionaryReady { get; set; }

    public static AssetResponse AssetVersion { get; private set; }

    public static void DownloadAssets(System.Action downloadComplete)
    {
        Debug.Log("Requesting asset list from server");
        
        //APIManagerServer.EnableAutoRetry = false;

        //int retryCount = 0;
        System.Action getAssets = () => { };

        getAssets = () =>
        {
            APIManager.Instance.GetRaincrow("assets", "" , (string s, int responseCode) =>
            {
                if (responseCode == 200 && !string.IsNullOrEmpty(s))
                {
                    //APIManagerServer.EnableAutoRetry = true;
                    Debug.Log("Assets to download:\n" + s);
                    var d = JsonConvert.DeserializeObject<AssetResponse>(s);
                    Instance.StartCoroutine(StartDownloads(d, downloadComplete, null));
                }
                else
                {
                    Debug.LogError("Failed to request asset list from server.\n[" + responseCode.ToString() + "] " + s);
                    OnServerError?.Invoke(responseCode, s);
                }
            });
        };

        getAssets.Invoke();
    }

    private static IEnumerator StartDownloads(AssetResponse assets, System.Action dictionariesDownloaded, System.Action bundlesDownloaded)
    {
        //check if server is under maintenance
        if (assets.maintenance)
        {
            OnMaintenance?.Invoke();
            yield break;
        }

        AssetVersion = assets;

        //setup the AppVersion
        int appVersion = 0;
#if UNITY_ANDROID
        appVersion = assets.android;
#elif UNITY_IOS
        appVersion = assets.apple;
#endif

        DownloadedAssets.AppVersion = $"{Application.version}.{appVersion}";
        if (Debug.isDebugBuild)
            DownloadedAssets.AppVersion += ".DEV";

#if !PRODUCTION
        DownloadedAssets.AppVersion += ".STAGING";
#endif
        DownloadedAssets.AppVersion += $" ({Application.buildGUID})";

        if (appVersion > int.Parse(Application.version))
        {
            OnVersionOutdated?.Invoke();
            yield break;
        }
        
        //download game the dictionary
        OnDictionaryDownloadStart?.Invoke();

        {
            bool isDictionaryComplete = false;
            bool isDictionaryParseError = false;
            string dictionaryDownloadError = null;

            DictionaryManager.GetGameDictionary(assets.game,
                onDicionaryReady: () =>
                {
                    isDictionaryComplete = true;
                },
                onDownloadError: (code, response) =>
                {
                    isDictionaryComplete = true;
                    dictionaryDownloadError = $"Error downloading game settings. [{code}] {response}";
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
                OnDictionaryParserError?.Invoke("","");
                yield break;
            }
        }

        //download the store dictionary
        {
            bool isDictionaryComplete = false;
            bool isDictionaryParseError = false;
            string dictionaryDownloadError = null;

            DictionaryManager.GetStoreDictionary(assets.store,
                onDicionaryReady: () =>
                {
                    isDictionaryComplete = true;
                },
                onDownloadError: (code, response) =>
                {
                    isDictionaryComplete = true;
                    dictionaryDownloadError = $"Error downloading store settings. [{code}] {response}";
                },
                onParseError: () =>
                {
                    isDictionaryComplete = true;
                    isDictionaryParseError = true;
                });

            //wait for download to finish
            while (isDictionaryComplete == false)
                yield return 0;

            //check if there was any error
            if (string.IsNullOrEmpty(dictionaryDownloadError) == false)
            {
                OnDictionaryError?.Invoke(dictionaryDownloadError);
                yield break;
            }

            if (isDictionaryParseError)
            {
                OnDictionaryParserError?.Invoke("","");
                yield break;
            }
        }

        //download the localisation dictionary
        {
            bool isDictionaryComplete = false;
            bool isDictionaryParseError = false;
            string dictionaryDownloadError = null;

            DictionaryManager.GetLocalisationDictionary(assets.localization,
                onDicionaryReady: () =>
                {
                    isDictionaryComplete = true;
                },
                onDownloadError: (code, response) =>
                {
                    isDictionaryComplete = true;
                    dictionaryDownloadError = $"Error downloading localization settings. [{code}] {response}";
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
                OnDictionaryParserError?.Invoke("","");
                yield break;
            }
        }


        OnDownloadedDictionary?.Invoke();
        DictionaryReady = true;
        dictionariesDownloaded?.Invoke();


        //figure out which bundles must be downloaded
        List<string> bundlesToDownload = new List<string>();
        AssetCacheJson cachedAssetKeys = new AssetCacheJson();

        //if (PlayerPrefs.HasKey("AssetCacheJson"))
        //{
            //cachedAssetKeys = JsonConvert.DeserializeObject<AssetCacheJson>(PlayerPrefs.GetString("AssetCacheJson"));
            foreach (string _key in assets.assets)
            {
                ////check if its not in the assetcache
                //if (cachedAssetKeys.bundles.Contains(_key) == false)
                //{
                //    bundlesToDownload.Add(_key);
                //}
                ////check if the file is in the persistendatapath
                //else
                //{
                    string assetPath = Path.Combine(Application.persistentDataPath, _key + ".unity3d");
                    if (System.IO.File.Exists(assetPath) == false)
                    {
                        bundlesToDownload.Add(_key);
                    }
                //}
            }
        //}
        //else
        //{
        //    cachedAssetKeys = new AssetCacheJson
        //    {
        //        bundles = new List<string>()
        //    };
        //    bundlesToDownload = assets.assets;
        //}
        
        //download the bundles
        string assetBaseUrl = DownloadManager.downloadUrl + "assetbundles/285/";
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            assetBaseUrl += "ios/";
        else
            assetBaseUrl += "android/";

        for (int i = 0; i < bundlesToDownload.Count; i++)
        {
            string assetName = bundlesToDownload[i];
            float size = 0;

            bool fail = true;
            int retryCount = 0;

            //get the actual file
            fail = true;
            retryCount = 0;
            UnityWebRequest www = UnityWebRequest.Get(assetBaseUrl + assetName);
            while (fail && retryCount < APIManagerServer.MaxRetries)
            {
                www.SendWebRequest();
                OnDownloadStart?.Invoke(assetName, i + 1, bundlesToDownload.Count, size);

                //Debug.Log(assetName + ": " + size.ToString("F2") + "MB");

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
                    yield return new WaitForSeconds(APIManagerServer.RetryCooldown);
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
            DownloadedAssets.LoadAssetPath(key);
        }

        OnDownloadsComplete?.Invoke();
        bundlesDownloaded?.Invoke();
    }

    public static bool DeserializeLocalisationDictionary(string json, System.Action onError)
    {
        try
        {
            DownloadedAssets.LocalizationDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse localization dictionary");
            if (Application.isEditor)
                Debug.Log(json);
            Debug.LogException(e);
            //OnDictionaryParserError?.Invoke(e.Message, e.StackTrace);
            onError?.Invoke();
            return false;
        }
    }

    public static bool DeserializeGameDictionary(string json, System.Action onError)
    {
        try
        {
            GameSettingsData data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameSettingsData>(json, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Populate
            });

            data.Spirits.Remove("spirit_greyShade");

            PlayerDataManager.summoningCosts = data.summoningCosts;
            PlayerDataManager.alignmentPerDegree = data.alignment;
            PlayerDataManager.xpToLevelup = data.experience;
            PlayerDataManager.baseEnergyPerLevel = data.baseEnergy;
            PlayerDataManager.forbiddenValue = data.forbiddenValue;
            MoonManager.LunarEffeciency = data.lunarEfficiency;

            DownloadedAssets.spellDictData = data.Spells;
            DownloadedAssets.spellDictData["attack"] = new SpellData
            {
                id = "attack",
                baseSpell = "",
                glyph = 9,
                school = -1,
                target = SpellData.Target.OTHER,
                states = new List<string> { "", "vulnerable" },
                hidden = true
            };

            DownloadedAssets.spiritDict = data.Spirits;
            //DownloadedAssets.gardenDict = data.Gardens;
            DownloadedAssets.conditionsDict = data.Conditions;
            DownloadedAssets.ingredientDict = data.Collectibles;
            WitchSchoolManager.witchVideos = data.witchSchool;

            DownloadedAssets.cosmeticDict = data.Cosmetics;
            StoreManagerAPI.IngredientBundleDict = data.Bundles;
            StoreManagerAPI.ConsumableDict = data.Consumables;
            StoreManagerAPI.CurrencyBundleDict = data.Currencies;

            DownloadedAssets.PlaceOfPowerSettings = data.PlaceOfPower;

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse game dictionary");
            Debug.LogException(e);
            //OnDictionaryParserError?.Invoke(e.Message, e.StackTrace);
            onError?.Invoke();
            return false;
        }
    }

    public static bool DeserializeStoreDictionary(string json, System.Action onError)
    {
        try
        {
            StoreManagerAPI.StoreData = JsonConvert.DeserializeObject<StoreData>(json, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Populate
            });

            foreach (var item in DownloadedAssets.cosmeticDict.Values)
            {
                Utilities.SetCatagoryApparel(item);
            }

            StoreManagerAPI.StoreData.Cosmetics.Sort(new System.Comparison<StoreItem>((a, b) => LocalizeLookUp.GetStoreTitle(a.id).CompareTo(LocalizeLookUp.GetStoreTitle(b.id))));

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse store dictionary");
            Debug.LogException(e);
            //OnDictionaryParserError?.Invoke(e.Message, e.StackTrace);

            onError?.Invoke();
            return false;
        }
    }

    public static void DownloadFile(string url, System.Action<long,long> onProgress, System.Action<int, string> onComplete)
    {
        var uri = new System.Uri(url);
        var bestHTTPRequest = new HTTPRequest(uri, (request, response) =>
        {
            Debug.Log("[download complete] " + url);
            onComplete?.Invoke(response.StatusCode, response.DataAsText);
        });

        Debug.Log("[download start] " + url);

        bestHTTPRequest.OnProgress += (request, downloaded, length) =>
        {
            //Debug.Log("[download progress] " + url + "\n" + downloaded + "/" + length);
            onProgress?.Invoke(downloaded, length);
        };

        //bestHTTPRequest.DisableRetry

        bestHTTPRequest.Send();
    }
}
