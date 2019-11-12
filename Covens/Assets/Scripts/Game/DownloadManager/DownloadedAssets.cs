using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using MEC;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class DownloadedAssets : MonoBehaviour
{
    private struct CovensAssetRequest
    {
        public string id;
        public System.Action<Sprite> callback;
        public bool isIcon;
    }
    private static DownloadedAssets m_Instance;
    public static DownloadedAssets Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("DownloadedAssets").AddComponent<DownloadedAssets>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    private static Dictionary<string, List<AssetBundle>> loadedBundles = new Dictionary<string, List<AssetBundle>>();
    public static Dictionary<string, List<string>> assetBundleDirectory = new Dictionary<string, List<string>>();

    public static string AppVersion { get; set; }

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

    public static bool UnloadingMemory { get; private set; }
    public static event System.Action OnWillUnloadAssets;

    private static List<CovensAssetRequest> m_RequestQueue = new List<CovensAssetRequest>();
    private static Coroutine m_LoadAssetCoroutine = null;

    void Awake()
    {        
        Application.lowMemory += OnApplicationLowMemory;

        //make sure no assetbundle is loaded
        var loadedBundles = AssetBundle.GetAllLoadedAssetBundles();
        foreach(var bundle in loadedBundles)
        {
            bundle.Unload(false);
        }
    }

    private void OnDestroy()
    {
        Application.lowMemory -= OnApplicationLowMemory;

        foreach (var bundleList in loadedBundles.Values)
            foreach (var bundle in bundleList)
                bundle.Unload(false);
    }

    public void OnApplicationLowMemory()
    {
        if (!UnloadingMemory)
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

            //unload unused
            AsyncOperation unloadAssets = Resources.UnloadUnusedAssets();
            unloadAssets.allowSceneActivation = true;
            unloadAssets.completed += (op) =>
            {
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
            };
        }
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

        m_RequestQueue.Add(new CovensAssetRequest
        {
            id = id,
            callback = callback,
            isIcon = isIcon
        });

        Log("load asset count " + m_RequestQueue.Count);

        if (m_LoadAssetCoroutine == null)
            m_LoadAssetCoroutine = Instance.StartCoroutine(GetSpriteCoroutine());

        return m_LoadAssetCoroutine;
    }

    public static Coroutine GetSprite(string id, Image image, bool isIcon = false)
    {
        return GetSprite(id, spr =>
        {
            if (image)
                image.overrideSprite = spr;
        }, isIcon);
    }

    #endregion
    
    static IEnumerator GetSpriteCoroutine()
    {
        while (m_RequestQueue.Count > 0)
        {
            //for (int i = 0; i < 10; i++)
            //    yield return null;

            bool failed = true;
            string id = m_RequestQueue[0].id;
            System.Action<Sprite> callback = m_RequestQueue[0].callback;
            bool isIcon = m_RequestQueue[0].isIcon;

            string type = "";
            if (id.Contains("spirit"))
                type = "spirit";
            else if (id == "attack" || id.Contains("spell") || id == "elixir_xp" || id == "elixir_degree")
                type = "spell";
            else if (!isIcon)
                type = "apparel";
            else if (isIcon)
                type = "icon";
            
            if (type == "spell")
            {
                SpellData spell = GetSpell(id);
                if (spell == null)
                    id = "0";
                else
                    id = spell.glyph.ToString();
            }

            if (!loadedBundles.ContainsKey(type) && assetBundleDirectory.ContainsKey(type))
            {
                loadedBundles[type] = new List<AssetBundle>();
                foreach (var item in assetBundleDirectory[type])
                {
                    float time = Time.unscaledTime;
                    Log("loading " + item);
                    var request = AssetBundle.LoadFromFileAsync(item);
                    request.completed += op => { Log(item + "\n" + op.isDone + " : " + op.progress); };
                    yield return request;
                    Log("loaded " + item + " in " + (Time.unscaledTime - time));
                    loadedBundles[type].Add(request.assetBundle);
                }
            }

            if (loadedBundles.ContainsKey(type))
            {
                foreach (var bundle in loadedBundles[type])
                {
                    if (bundle.Contains(id + ".png"))
                    {
                        float time = Time.unscaledTime;
                        Log("loading " + id + ".png");
                        AssetBundleRequest request = bundle.LoadAssetAsync(id + ".png", typeof(Sprite));
                        request.completed += op => { Log(id + ".png\n" + op.isDone + " : " + op.progress); };
                        yield return request;
                        Log("loaded " + id + ".png in " + (Time.unscaledTime - time));
                        var sprite = request.asset as Sprite;
                        callback?.Invoke(sprite);

                        failed = false;
                        break;
                    }
                }
            }

            m_RequestQueue.RemoveAt(0);

            if (failed)
            {
                Debug.LogException(new System.Exception("sprite not found for " + id + " in bundle " + type));
                callback?.Invoke(null);
            }
        }
        m_LoadAssetCoroutine = null;
    }

    public static bool IsBundleDownloaded(string id)
    {
        foreach (string key in assetBundleDirectory.Keys)
        {
            if (key.Contains(id))
                return true;
        }

        return false;
    }

    public static void LoadAssetPath(string assetKey)
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
//#if UNITY_EDITOR
        //Debug.Log("<color=magenta> " + msg + " </color>");
//#endif
    }
}





