using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using MEC;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class DownloadedAssets : MonoBehaviour
{
    private static DownloadedAssets m_Instance;
    public static DownloadedAssets Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new GameObject("DownloadedAssets").AddComponent<DownloadedAssets>();
            return m_Instance;
        }
    }

    private static Dictionary<string, List<AssetBundle>> loadedBundles = new Dictionary<string, List<AssetBundle>>();
    public static Dictionary<string, List<string>> assetBundleDirectory = new Dictionary<string, List<string>>();

    public static string AppVersion { get; set; }

    public static Dictionary<string, Sprite> AllSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> IconSprites = new Dictionary<string, Sprite>();

    private static Dictionary<string, string> m_LocalizationDict = null;
    public static Dictionary<string, string> LocalizationDictionary
    {
        get
        {
            if (m_LocalizationDict == null)
            {
                string path = "Localization/" + DictionaryManager.Languages[DictionaryManager.languageIndex];
                TextAsset asset = Resources.Load<TextAsset>(path);
                string json = asset ? asset.text : Resources.Load<TextAsset>("Localization/English").text;
                Debug.Log("Initializing default localization \"" + path + ".json\"");
                m_LocalizationDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            return m_LocalizationDict;
        }
        set
        {
            m_LocalizationDict = value;
        }
    }


    public static Dictionary<string, SpellData> spellDictData = new Dictionary<string, SpellData>();
    public static Dictionary<string, SpiritData> spiritDict = new Dictionary<string, SpiritData>();
    public static Dictionary<string, ConditionData> conditionsDict = new Dictionary<string, ConditionData>();
    public static Dictionary<string, IngredientData> ingredientDict = new Dictionary<string, IngredientData>();
    public static Dictionary<string, CosmeticData> cosmeticDict = new Dictionary<string, CosmeticData>();

    public static PlaceOfPowerSettings PlaceOfPowerSettings { get; set; }

    public static bool LoadingAsset { get; private set; }
    public static bool UnloadingMemory { get; private set; }
    public static event System.Action OnWillUnloadAssets;

    void Awake()
    {        
        Application.lowMemory += OnApplicationLowMemory;
    }

    private void OnDestroy()
    {
        Application.lowMemory -= OnApplicationLowMemory;
    }

    public void OnApplicationLowMemory()
    {
        if (!UnloadingMemory)
        {
            StartCoroutine(UnloadMemory());
        }
    }

    private IEnumerator UnloadMemory()
    {
        string debug = "Memory is low. Unloading assets";
        debug += "\n- Total system memory: " + SystemInfo.systemMemorySize;
        debug += "\n- Total video memory: " + SystemInfo.graphicsMemorySize;
        debug += "\n- Total Reserved memory by Unity: " + (Profiler.GetTotalReservedMemoryLong() / 1000000) + "MB";
        debug += "\n- Allocated memory by Unity: " + (Profiler.GetTotalAllocatedMemoryLong() / 1000000) + "MB";
        debug += "\n- Reserved but not allocated: " + (Profiler.GetTotalUnusedReservedMemoryLong() / 1000000) + "MB";
        debug += "\n- Allocated memory for graphics driver: " + (Profiler.GetAllocatedMemoryForGraphicsDriver() / 1000000) + "MB";
        debug += "\n- Reserved space for managed-memory: " + (Profiler.GetMonoHeapSizeLong() / 1000000) + "MB";
        debug += "\n- Allocated managed-memory: " + (Profiler.GetMonoHeapSizeLong() / 1000000) + "MB";
        Debug.LogError(debug);

        UnloadingMemory = true;
        OnWillUnloadAssets?.Invoke();
        
        //unload assetbundles
        foreach (var bundleList in loadedBundles.Values)
        {
            foreach (var bundle in bundleList)
            {
                bundle.Unload(false);
            }
        }
        loadedBundles.Clear();
        AllSprites.Clear();
        IconSprites.Clear();

        //unload unused
        AsyncOperation unloadAssets = Resources.UnloadUnusedAssets();
        yield return unloadAssets;

        yield return 0;
        yield return new WaitForEndOfFrame();

        //hide the UI
        UnloadingMemory = false;

        debug = "Asset unloading complete";
        debug += "\n- Total system memory: " + SystemInfo.systemMemorySize;
        debug += "\n- Total video memory: " + SystemInfo.graphicsMemorySize;
        debug += "\n- Total Reserved memory by Unity: " + (Profiler.GetTotalReservedMemoryLong() / 1000000) + "MB";
        debug += "\n- Allocated memory by Unity: " + (Profiler.GetTotalAllocatedMemoryLong() / 1000000) + "MB";
        debug += "\n- Reserved but not allocated: " + (Profiler.GetTotalUnusedReservedMemoryLong() / 1000000) + "MB";
        debug += "\n- Allocated memory for graphics driver: " + (Profiler.GetAllocatedMemoryForGraphicsDriver() / 1000000) + "MB";
        debug += "\n- Reserved space for managed-memory: " + (Profiler.GetMonoHeapSizeLong() / 1000000) + "MB";
        debug += "\n- Allocated managed-memory: " + (Profiler.GetMonoHeapSizeLong() / 1000000) + "MB";
        Debug.Log(debug);
    }

    #region SpriteGetters
    public static Coroutine GetSprite(string id, System.Action<Sprite> callback, bool isIcon = false)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogException(new System.Exception("nullorempty id"));
            callback?.Invoke(null);
            return null;
        }

        if (!isIcon && AllSprites.ContainsKey(id))
        {
            callback?.Invoke(AllSprites[id]);
            return null;
        }
        else if (isIcon && IconSprites.ContainsKey(id))
        {
            callback?.Invoke(IconSprites[id]);
            return null;
        }
        else
        {
            return Instance.StartCoroutine(getSpritetHelper(id, callback, isIcon));
            //Timing.RunCoroutine(getSpritetHelper(id, callback, isIcon));
        }
    }

    public static Coroutine GetSprite(string id, Image image, bool isIcon = false)
    {
        return GetSprite(id, spr => image.overrideSprite = spr, isIcon);
    }

    #endregion
    
    static IEnumerator getSpritetHelper(string id, System.Action<Sprite> callback, bool isIcon)
    {
        //Log("get sprite " + id);
        while (LoadingAsset)
            yield return null;

        LoadingAsset = true;

        string type = "";
        if (id.Contains("spirit"))
            type = "spirit";
        else if (id == "attack" || id.Contains("spell") || id == "elixir_xp" || id == "elixir_degree")
            type = "spell";
        else if (!isIcon)
            type = "apparel";
        else if (isIcon)
            type = "icon";
        
        if (!loadedBundles.ContainsKey(type))
        {
            if (assetBundleDirectory.ContainsKey(type) == false)
            {
                Debug.LogError($"Asset bundle \"{type}\" not found");
                LoadingAsset = false;
                callback?.Invoke(null);
                yield break;
            }

            loadedBundles[type] = new List<AssetBundle>();
            foreach (var item in assetBundleDirectory[type])
            {
                //float time = Time.unscaledTime;
                //Log("loading " + item);
                var request = AssetBundle.LoadFromFileAsync(item);
                yield return request;
                //Log("loaded " + item + " in " + (Time.unscaledTime - time));
                loadedBundles[type].Add(request.assetBundle);
            }
        }

        if (type == "spell")
        {
            SpellData spell = GetSpell(id);
            if (spell == null)
            {
                LoadingAsset = false;
                callback?.Invoke(null);
                yield break;
            }
            id = spell.glyph.ToString();
        }

        foreach (var item in loadedBundles[type])
        {
            if (item.Contains(id + ".png"))
            {
                //float time = Time.unscaledTime;
                //Log("loading " + id + ".png");
                AssetBundleRequest request = item.LoadAssetAsync(id + ".png", typeof(Sprite));
                yield return request;
                //Log("loaded " + id + ".png in " + (Time.unscaledTime - time));
                var tempSp = request.asset as Sprite;
                callback?.Invoke(tempSp);

                if (isIcon)
                    IconSprites[tempSp.texture.name] = tempSp;
                else
                    AllSprites[tempSp.texture.name] = tempSp;

                //yield return Timing.WaitForOneFrame;
                LoadingAsset = false;
                yield break;
            }
        }

        //yield return Timing.WaitForOneFrame;
        Debug.LogException(new System.Exception("sprite not found for " + id + " in bundle " + type));
        LoadingAsset = false;
        callback?.Invoke(null);
    }

    //public static void LoadStyleApparel(string id, System.Action<Sprite> callback)
    //{
    //    string path = System.IO.Path.Combine(Application.persistentDataPath, assetKey + ".unity3d");
    //    string currentKey = "apparel";

    //}

    public static void LoadAsset(string assetKey)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, assetKey + ".unity3d");
        string currentKey = "";

        if (assetKey.Contains("spirit"))
            currentKey = "spirit";
        else if (assetKey.Contains("spell"))
            currentKey = "spell";
        else if (assetKey.Contains("apparel"))
            currentKey = "apparel";
        else if (assetKey.Contains("icon"))
            currentKey = "icon";

        if (assetBundleDirectory.ContainsKey(currentKey))
        {
            if (!assetBundleDirectory[currentKey].Contains(path))
                assetBundleDirectory[currentKey].Add(path);
        }
        else
        {
            assetBundleDirectory[currentKey] = new List<string>() { path };
        }
    }

    public static SpellData GetSpell(string id)
    {
        if (id == null)
            id = "attack";

        if (spellDictData.ContainsKey(id))
        {
            SpellData res = spellDictData[id];
            if (res.states == null)
                res.states = new List<string>();
            if (res.ingredients == null)
                res.ingredients = new List<string>();
            return res;
        }
        else
        {
            Debug.LogError($"Spell \"{id}\" not found.");
            return null;
        }
    }

    public static ConditionData GetCondition(string id)
    {
        if (conditionsDict.ContainsKey(id))
        {
            return conditionsDict[id];
        }
        else
        {
            Debug.LogError($"Condition \"{id}\" not found.");
            return new ConditionData
            {
                spellID = id
            };
        }
    }

    public static SpiritData GetSpirit(string id)
    {
        if (string.IsNullOrEmpty(id) == false && spiritDict.ContainsKey(id))
        {
            return spiritDict[id];
        }
        else
        {
            Debug.LogError($"Spirit \"{id}\" not found.");
            return new SpiritData
            {
                id = id,
                legend = "?",
                type = "?"
            };
        }
    }

    public static IngredientData GetCollectable(string id)
    {
        if (ingredientDict.ContainsKey(id))
        {
            return ingredientDict[id];
        }
        else
        {
            Debug.LogError($"Collectable \"{id}\" not found.");
            return new IngredientData
            {
                type = "",
                forbidden = false,
                rarity = 0,
            };
        }
    }

    public static CosmeticData GetCosmetic(string id)
    {
        if (cosmeticDict.ContainsKey(id))
        {
            return cosmeticDict[id];
        }
        else
        {
            Debug.LogError($"Cosmetic \"{id}\" not found.");
            return null;
        }
    }

    private static void Log(string msg)
    {
        Debug.Log("<color=magenta> " + msg + " </color>");
    }
}





