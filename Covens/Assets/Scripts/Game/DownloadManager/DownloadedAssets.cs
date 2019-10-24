using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine.UI;

public class DownloadedAssets : MonoBehaviour
{
    public static DownloadedAssets Instance { get; set; }

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
                string path = "Localization/" + "Portuguese";// DictionaryManager.Languages[DictionaryManager.languageIndex];
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

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        Application.lowMemory += OnApplicationLowMemory;
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
        Debug.LogError("Memory is low. Unloading assets.");
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

        //hide the UI
        UnloadingMemory = false;
    }

    #region SpriteGetters
    public static void GetSprite(string id, System.Action<Sprite> callback, bool isIcon = false)
    {
        if (!isIcon && AllSprites.ContainsKey(id))
        {
            callback?.Invoke(AllSprites[id]);
        }
        else if (isIcon && IconSprites.ContainsKey(id))
        {
            callback?.Invoke(IconSprites[id]);
        }
        else
        {
            Timing.RunCoroutine(getSpiritHelper(id, callback, isIcon));
        }
    }

    public static void GetSprite(string id, Image spr, bool isIcon = false)
    {
        if (!isIcon && AllSprites.ContainsKey(id))
        {
            spr.overrideSprite = AllSprites[id];

        }
        else if (isIcon && IconSprites.ContainsKey(id))
        {
            spr.overrideSprite = IconSprites[id];

        }
        else
        {
            Timing.RunCoroutine(getSpiritHelper(id, spr, isIcon));
        }
    }
    #endregion
    
    static IEnumerator<float> getSpiritHelper(string id, System.Action<Sprite> callback, bool isIcon)
    {

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
                callback?.Invoke(null);
                yield break;
            }

            loadedBundles[type] = new List<AssetBundle>();
            foreach (var item in assetBundleDirectory[type])
            {
                var bundleRequest = AssetBundle.LoadFromFile(item);
                loadedBundles[type].Add(bundleRequest);
            }
        }

        if (type == "spell")
        {
            SpellData spell = GetSpell(id);
            if (spell == null)
            {
                callback?.Invoke(null);
                yield break;
            }
            id = spell.glyph.ToString();
        }

        foreach (var item in loadedBundles[type])
        {
            if (item.Contains(id + ".png"))
            {
                var request = item.LoadAssetAsync(id + ".png", typeof(Sprite));
                Timing.WaitUntilDone(request);
                var tempSp = request.asset as Sprite;
                callback?.Invoke(tempSp);

                if (isIcon)
                    IconSprites[tempSp.texture.name] = tempSp;
                else
                    AllSprites[tempSp.texture.name] = tempSp;

                yield return Timing.WaitForOneFrame;
                yield break;
            }
        }

        yield return Timing.WaitForOneFrame;
        Debug.LogException(new System.Exception("sprite not found for " + id + " in bundle " + type));
        callback?.Invoke(null);
    }

    static IEnumerator<float> getSpiritHelper(string id, Image spr, bool isIcon)
    {

        string type = "";
        if (id.Contains("spirit"))
            type = "spirit";
        else if (id.Contains("spell") || id == "elixir_xp" || id == "elixir_degree")
            type = "spell";
        else if (!isIcon)
            type = "apparel";
        else if (isIcon)
            type = "icon";

        if (!loadedBundles.ContainsKey(type))
        {
            loadedBundles[type] = new List<AssetBundle>();
            foreach (var item in assetBundleDirectory[type])
            {
                var bundleRequest = AssetBundle.LoadFromFile(item);
                loadedBundles[type].Add(bundleRequest);

            }
        }

        if (type == "spell")
            id = spellDictData[id].glyph.ToString();

        spr.overrideSprite = null;
        foreach (var item in loadedBundles[type])
        {
            if (item.Contains(id + ".png"))
            {
                var request = item.LoadAssetAsync(id + ".png", typeof(Sprite));
                Timing.WaitUntilDone(request);
                var tempSp = request.asset as Sprite;
                spr.overrideSprite = tempSp;
                if (isIcon)
                    IconSprites[tempSp.texture.name] = tempSp;
                else
                    AllSprites[tempSp.texture.name] = tempSp;

            }
        }

        yield return Timing.WaitForOneFrame;

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
        if (spiritDict.ContainsKey(id))
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
}





