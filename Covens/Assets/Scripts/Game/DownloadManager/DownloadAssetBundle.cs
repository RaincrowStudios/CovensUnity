
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class DownloadAssetBundle : MonoBehaviour
{

    public static DownloadAssetBundle Instance { get; set; }
    public TextMeshProUGUI downloadingTitle;
    public TextMeshProUGUI downloadingInfo;
    public Slider slider;
    public GameObject DownloadUI;
    public static string baseURL = "https://storage.googleapis.com/raincrow-covens/";
    bool isDownload = false;

    List<string> existingBundles = new List<string>();
    List<string> downloadableAssets = new List<string>();
    int TotalAssets = 0;
    public static bool isDictLoaded = false;
    public static bool isAssetBundleLoaded = false;
    AssetResponse AS;

    public GameObject playstoreIcon;
    public GameObject appleIcon;


    enum AssetType
    {
        spirit
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }

    void Start()
    {
        var data = new { game = "covens" };
        APIManager.Instance.Post("assets", JsonConvert.SerializeObject(data), (string s, int r) =>
        {
            if (r == 200)
            {
                print(s);
                var d = JsonConvert.DeserializeObject<AssetResponse>(s);
                isDictLoaded = false;
                isAssetBundleLoaded = false;
                if (d.maintenance)
                {
                    StartUpManager.Instance.ServerDown.SetActive(true);
                    return;
                }
                AS = d;

#if UNITY_IPHONE

                if (d.apple > int.Parse(Application.version))
                {
                    StartUpManager.Instance.OutDatedBuild();
                    StartUpManager.Instance.enabled = false;
                    GetGPS.instance.enabled = false;
                    appleIcon.SetActive(true);
                    return;
                }

#endif

#if UNITY_ANDROID
                {
                    if (d.android > int.Parse(Application.version))
                    {
                        StartUpManager.Instance.OutDatedBuild();
                        StartUpManager.Instance.enabled = false;
                        GetGPS.instance.enabled = false;
                        playstoreIcon.SetActive(true);
                        return;
                    }
                }
#endif


                StartCoroutine(InitiateLogin());
                if (PlayerPrefs.GetString("AssetCacheJson") != "")
                {
                    var cache = JsonConvert.DeserializeObject<AssetCacheJson>(PlayerPrefs.GetString("AssetCacheJson"));
                    existingBundles = cache.bundles;
                }
                d.assets.Add("map");
                DownloadAsset(d.assets);

                StartCoroutine(AnimateDownloadingText());
                StartCoroutine(GetDictionaryMatrix());

            }
            else
            {
                print(s);
                StartUpManager.Instance.ServerDown.SetActive(true);
            }
        }, false, false);

    }


    IEnumerator GetDictionaryMatrix(int version = 0)
    {
        //Debug.LogError("DOWNLOADING NEW DICTIONARY FORMAT - REMEMBER TO REMOVE THIS AFTER UPLOADING NEW DICTIONARY TO SERVER");
#if !PRODUCTION
        AS.dictionary = "Dictionary60.json";
#endif

        string filename = "dict.text";
        string localDictionaryPath = Path.Combine(Application.persistentDataPath, filename);

        if (PlayerPrefs.HasKey("DataDict"))
        {
            string currentDictionary = PlayerPrefs.GetString("DataDict");
            if (currentDictionary == AS.dictionary)
            {
                if (System.IO.File.Exists(localDictionaryPath))
                {
                    Debug.Log($"\"{AS.dictionary}\" already downloaded.");
                    string json = System.IO.File.ReadAllText(localDictionaryPath);
                    SaveDict(JsonConvert.DeserializeObject<DictMatrixData>(json));
                    yield break;
                }
                else
                {
                    Debug.Log($"Dictionary \"{AS.dictionary}\" is marked as download but not found.");
                }
            }
            else
            {
                Debug.Log($"Dictionary \"{currentDictionary}\" outdated.");
            }
        }
        else
        {
            Debug.Log("No dictionary found");
        }

        Debug.Log($"Downloading \"{AS.dictionary}\"");
        using (UnityWebRequest www = UnityWebRequest.Get(baseURL + AS.dictionary))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Couldnt load the dictionary:\n" + www.error);
                //#if UNITY_EDITOR
                //                Debug.Log("loading local dictionary");
                //                TextAsset textAsset = UnityEditor.EditorGUIUtility.Load("dictionary.json") as TextAsset;
                //                if (textAsset != null)
                //                {
                //                    var data = JsonConvert.DeserializeObject<DictMatrixData>(textAsset.text);
                //                    SaveDict(data);
                //                }
                //                else
                //                {
                //                    Debug.LogError("no local dictionary available");
                //                }
                //#endif
            }
            else
            {
                File.WriteAllText(localDictionaryPath, www.downloadHandler.text);
                PlayerPrefs.SetString("DataDict", AS.dictionary);
                Debug.Log($"Downloaded new dictionary \"{AS.dictionary}\"");
                try
                {
                    string text = System.IO.File.ReadAllText(localDictionaryPath);
                    var data = JsonConvert.DeserializeObject<DictMatrixData>(text);
                    SaveDict(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }

    public void SaveDict(DictMatrixData data)
    {
        try
        {
            foreach (var item in data.Spells)
            {
                DownloadedAssets.spellDictData.Add(item.spellID, item);
            }
            foreach (var item in data.Spirits)
            {
                DownloadedAssets.spiritDictData.Add(item.spiritID, item);
            }
            foreach (var item in data.Conditions)
            {
                DownloadedAssets.conditionsDictData[item.conditionID] = item;
            }
            foreach (var item in data.Collectibles)
            {
                DownloadedAssets.ingredientDictData.Add(item.id, item);
            }
            foreach (var item in data.Store)
            {
                DownloadedAssets.storeDict[item.id] = item;
            }
            foreach (var item in data.Quest)
            {
                DownloadedAssets.questsDict.Add(item.id, item);
            }
            foreach (var item in data.CountryCodes)
            {
                DownloadedAssets.countryCodesDict.Add(item.id, item);
            }
            foreach (var item in data.SpiritTypes)
            {
                DownloadedAssets.spiritTypeDict.Add(item.id, item);
            }
            foreach (var item in data.Gardens)
            {
                DownloadedAssets.gardenDict.Add(item.id, item);
            }
            foreach (var item in data.Other)
            {
                DownloadedAssets.localizedText[item.id] = item.value;
            }
            foreach (var item in data.FTFDialogues)
            {
                DownloadedAssets.ftfDialogues.Add(item.value);
            }
            foreach (var item in data.Zone)
            {
                DownloadedAssets.zonesIDS[int.Parse(item.id)] = item.value;
            }
            DownloadedAssets.ftfDialogues.Add("");     // its need one empty string at the end of array
            DownloadedAssets.tips = data.LoadingTips;
            WitchSchoolManager.witchVideos = data.WitchSchool;
            isDictLoaded = true;
            LocalizationManager.CallChangeLanguage();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            StartUpManager.Instance.ServerDown.SetActive(true);
        }

    }

    IEnumerator AnimateDownloadingText()
    {
        string downloadText = LocalizeLookUp.GetText("download");
        float delay = .5f;
        downloadingTitle.text = downloadText;
        yield return new WaitForSeconds(delay);
        downloadingTitle.text = downloadText + " .";
        yield return new WaitForSeconds(delay);
        downloadingTitle.text = downloadText + " . .";
        yield return new WaitForSeconds(delay);
        downloadingTitle.text = downloadText + " . . .";
        StartCoroutine(AnimateDownloadingText());
    }

    public void DownloadAsset(List<string> assetKeys)
    {
        foreach (var item in assetKeys)
        {
            if (!existingBundles.Contains(item))
            {
                TotalAssets++;
                downloadableAssets.Add(item);
            }
            else
            {
                if (item.Contains("spirit"))
                {
                    LoadAsset(item);
                }
                else if (item.Contains("spell"))
                {
                    LoadAsset(item);
                }
                else if (item.Contains("apparel"))
                {
                    LoadAsset(item);
                }
                else if (item.Contains("icon"))
                {
                    LoadAsset(item);
                }
                else if (item.Contains("map"))
                {
                    LoadAsset(item);
                }

            }
        }

        if (downloadableAssets.Count > 0)
        {
            DownloadAssetHelper(0);
        }
        else
        {
            isAssetBundleLoaded = true;
        }
    }

    //	public void InitiateLoginHelper()
    //	{
    //		StartCoroutine (InitiateLogin ());
    //	}
    //
    IEnumerator InitiateLogin()
    {
        yield return new WaitUntil(() => isAssetBundleLoaded == true);
        yield return new WaitUntil(() => isDictLoaded == true);
        DownloadUI.SetActive(false);
        this.StopAllCoroutines();
    }

    void DownloadAssetHelper(int i)
    {
        StartCoroutine(StartDownload(AssetType.spirit, downloadableAssets[i], i));
    }

    IEnumerator StartDownload(AssetType asset, string assetKey, int i)
    {
        string url = baseURL + assetKey;

#if UNITY_IPHONE
        url = baseURL + "appleassets/" + assetKey;
#endif

        UnityWebRequest webRequest = UnityWebRequest.Head(url);
        webRequest.Send();
        while (!webRequest.isDone)
        {
            yield return null;
        }

        float size = float.Parse(webRequest.GetResponseHeader("Content-Length")) * 0.000001f;
        downloadingInfo.text = "Assets " + (i + 1).ToString() + " out of " + TotalAssets.ToString() + " (" + size.ToString("F2") + "MB)";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            //			print ("Pulling assets from : " + url);
            isDownload = true;
            StartCoroutine(Progress(request));
            yield return request.SendWebRequest();
            isDownload = false;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Couldn't reach the servers!");
            }
            else
            {
                //				print ("Bundle Downloaded");
                i++;
                string tempPath = Path.Combine(Application.persistentDataPath, assetKey + ".unity3d");
                File.WriteAllBytes(tempPath, request.downloadHandler.data);
                //				print ("Asset Stored : " + tempPath);
                existingBundles.Add(assetKey);
                AssetCacheJson CacheJson = new AssetCacheJson { bundles = existingBundles };
                PlayerPrefs.SetString("AssetCacheJson", JsonConvert.SerializeObject(CacheJson));
                LoadAsset(assetKey);
            }
        }
        if (downloadableAssets.Count > i)
        {
            DownloadAssetHelper(i);
        }
        else
        {

            isAssetBundleLoaded = true;

        }
    }

    void LoadAsset(string assetKey)
    {

        string path = Path.Combine(Application.persistentDataPath, assetKey + ".unity3d");
        string currentKey = "";

        if (assetKey.Contains("spirit"))
        {
            currentKey = "spirit";


        }
        else if (assetKey.Contains("spell"))
        {
            currentKey = "spell";


        }
        else if (assetKey.Contains("apparel"))
        {
            currentKey = "apparel";

        }
        else if (assetKey.Contains("icon"))
        {
            currentKey = "icon";

        }
        else if (assetKey.Contains("map"))
        {
            currentKey = "map";
            print("map");
        }

        if (DownloadedAssets.assetBundleDirectory.ContainsKey(currentKey))
        {
            DownloadedAssets.assetBundleDirectory[currentKey].Add(path);
        }
        else
        {
            DownloadedAssets.assetBundleDirectory[currentKey] = new List<string>() { path };
            //			print (path);
        }

    }



    IEnumerator delayUnload(AssetBundle bundle)
    {
        yield return new WaitForSeconds(.15f);
        bundle.Unload(false);
    }

    IEnumerator Progress(UnityWebRequest req)
    {
        while (isDownload)
        {
            slider.value = req.downloadProgress;
            yield return null;
        }
    }
}

#region json classes
public class AssetCacheJson
{
    public List<string> bundles { get; set; }
}

public class ConditionDict
{
    public string conditionID { get; set; }

    public string spellID { get; set; }

    public string conditionDescription { get; set; }
}

public class SpellDict
{
    public string spellID { get; set; }

    public string spellName { get; set; }

    public string spellLore { get; set; }

    public int spellGlyph { get; set; }

    public string spellDescription { get; set; }

    public int spellSchool { get; set; }
}

public class SpiritDict
{
    public string spiritID { get; set; }

    public string spiritName { get; set; }

    public string spiritDescription { get; set; }

    public string spriitBehavior { get; set; }

    public int spiritTier { get; set; }

    public string spiritLegend { get; set; }

    public string spiritTool { get; set; }
}

public class DictMatrixData
{
    public List<SpellDict> Spells { get; set; }

    public List<SpiritDict> Spirits { get; set; }

    public List<ConditionDict> Conditions { get; set; }

    public List<IngredientDict> Collectibles { get; set; }

    public List<StoreDictData> Store { get; set; }

    public List<LocalizeData> Quest { get; set; }

    public List<LocalizeData> CountryCodes { get; set; }

    public List<LocalizeData> LoadingTips { get; set; }

    public List<LocalizeData> SpiritTypes { get; set; }

    public List<LocalizeData> WitchSchool { get; set; }

    public List<LocalizeData> Gardens { get; set; }

    public List<LocalizeData> Other { get; set; }
    public List<LocalizeData> Zone { get; set; }

    public List<LocalizeData> FTFDialogues { get; set; }
}

public class IngredientDict
{
    public string id { get; set; }

    public string description { get; set; }

    public string hint { get; set; }

    public int rarity { get; set; }

    public string name { get; set; }

    public string type { get; set; }

    public string spirit { get; set; }
}

public class StoreDictData
{
    public string id { get; set; }

    public string title { get; set; }

    public string subtitle { get; set; }

    public string onBuyTitle { get; set; }

    public string onBuyDescription { get; set; }

    public string onConsumeDescription { get; set; }
}

public class LocalizeData
{
    public string id { get; set; }
    public string value { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}


public class AssetResponse
{
    public string dictionary { get; set; }
    public List<string> assets { get; set; }
    public int android { get; set; }
    public int apple { get; set; }
    public bool maintenance { get; set; }
}
#endregion