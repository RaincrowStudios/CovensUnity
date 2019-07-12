using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine.UI;

public class DownloadedAssets : MonoBehaviour
{
    [Header("Low memory UI")]
    [SerializeField] private Canvas m_Canvas;

    public static DownloadedAssets Instance { get; set; }

    private static Dictionary<string, List<AssetBundle>> loadedBundles = new Dictionary<string, List<AssetBundle>>();
    public static Dictionary<string, List<string>> assetBundleDirectory = new Dictionary<string, List<string>>();

    public static string AppVersion { get; set; }

    public static Dictionary<string, Sprite> AllSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> IconSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, string> localizedText = new Dictionary<string, string>();


    public static Dictionary<string, SpellData> spellDictData = new Dictionary<string, SpellData>();
    public static Dictionary<string, SpiritData> spiritDict = new Dictionary<string, SpiritData>();
    public static Dictionary<string, GardenData> gardenDict = new Dictionary<string, GardenData>();
    public static Dictionary<string, ConditionData> conditionsDict = new Dictionary<string, ConditionData>();
    public static Dictionary<string, IngredientData> ingredientDict = new Dictionary<string, IngredientData>();
    public static Dictionary<string, CosmeticData> cosmeticDict = new Dictionary<string, CosmeticData>();
    

    public static bool UnloadingMemory { get; private set; }
    public static event System.Action OnWillUnloadAssets;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        m_Canvas.enabled = false;
        m_Canvas.gameObject.SetActive(false);

        Application.lowMemory += OnApplicationLowMemory;
    }

    private void OnApplicationLowMemory()
    {
        if (!UnloadingMemory)
        {
            StartCoroutine(UnloadMemory());
        }
    }

    private IEnumerator UnloadMemory()
    {
        UnloadingMemory = true;
        OnWillUnloadAssets?.Invoke();

        //show the UI
        m_Canvas.gameObject.SetActive(true);
        m_Canvas.enabled = true;

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
        m_Canvas.enabled = false;
        m_Canvas.gameObject.SetActive(false);
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

            }
        }

        yield return Timing.WaitForOneFrame;
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
            return spellDictData[id];
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
                spellID = "?"
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
                type = "?",
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
            return null;
        }
    }
}





