using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine.UI;

public class DownloadedAssets : MonoBehaviour
{
    public static DownloadedAssets Instance { get; set; }
    public static Dictionary<string, SpiritDict> spiritDictData = new Dictionary<string, SpiritDict>();
    public static Dictionary<string, SpellDict> spellDictData = new Dictionary<string, SpellDict>();
    public static Dictionary<string, SpellFeedbackData> spellFeedbackDictData = new Dictionary<string, SpellFeedbackData>();
    public static Dictionary<string, ConditionDict> conditionsDictData = new Dictionary<string, ConditionDict>();
    public static Dictionary<string, IngredientDict> ingredientDictData = new Dictionary<string, IngredientDict>();
    public static Dictionary<string, StoreDictData> storeDict = new Dictionary<string, StoreDictData>();
    public static Dictionary<string, LocalizeData> questsDict = new Dictionary<string, LocalizeData>();
    public static Dictionary<string, LocalizeData> countryCodesDict = new Dictionary<string, LocalizeData>();
    public static List<LocalizeData> tips = new List<LocalizeData>();
    public static Dictionary<string, LocalizeData> spiritTypeDict = new Dictionary<string, LocalizeData>();
    public static Dictionary<string, LocalizeData> gardenDict = new Dictionary<string, LocalizeData>();
    public static Dictionary<string, Sprite> AllSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, Sprite> IconSprites = new Dictionary<string, Sprite>();
    public static Dictionary<string, List<string>> assetBundleDirectory = new Dictionary<string, List<string>>();
    static Dictionary<string, List<AssetBundle>> loadedBundles = new Dictionary<string, List<AssetBundle>>();
    public static Dictionary<string, LocalizeData> localizedText = new Dictionary<string, LocalizeData>();
    public static Dictionary<int, string> zonesIDS = new Dictionary<int, string>();
    public static List<string> ftfDialogues = new List<string>();
    public static string AppVersion { get; set; }

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    #region SpriteGetters
    public static void GetSprite(string id, System.Action<Sprite> callback, bool isIcon = false)
    {
        if (!isIcon && AllSprites.ContainsKey(id))
        {
            //spr.sprite = AllSprites[id];
            callback?.Invoke(AllSprites[id]);
        }
        else if (isIcon && IconSprites.ContainsKey(id))
        {
            //spr.sprite = IconSprites[id];
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
            spr.sprite = AllSprites[id];

        }
        else if (isIcon && IconSprites.ContainsKey(id))
        {
            spr.sprite = IconSprites[id];

        }
        else
        {
            Timing.RunCoroutine(getSpiritHelper(id, spr, isIcon));
        }
    }
    #endregion

    private static AssetBundle m_MapBundle;
    private static List<AssetBundleRequest> m_MapRequests = new List<AssetBundleRequest>();

    public static void InstantiateStateMap(System.Action<GameObject> callback)
    {
        InstantiateMap("mapState.prefab", callback);
    }

    public static void InstantiateCountryMap(System.Action<GameObject> callback)
    {
        InstantiateMap("mapCountry.prefab", callback);
    }

    private static void InstantiateMap(string name, System.Action<GameObject> callback)
    {
        if (m_MapBundle == null)
        {
            m_MapBundle = AssetBundle.LoadFromFile(assetBundleDirectory["map"][0]); ;
        }

        var request = m_MapBundle.LoadAssetAsync<GameObject>(name);
        m_MapRequests.Add(request);

        request.completed += op =>
        {
            callback?.Invoke(Instantiate(request.asset) as GameObject);
            m_MapRequests.Remove(request);

            if (m_MapRequests.Count == 0)
            {
                m_MapBundle.Unload(false);
            }
        };
    }



    //public void CreateMap()
    //{
    //    StartCoroutine(GetMapPrefab());
    //}

    //IEnumerator GetMapPrefab()
    //{
    //    var mapBundle = AssetBundle.LoadFromFile(assetBundleDirectory["map"][0]);
    //    var prefab = mapBundle.LoadAssetAsync<GameObject>("mapState.prefab");
    //    // var prefab = mapBundle.LoadAssetAsync<GameObject>("mapCountry.prefab");
    //    yield return prefab;
    //    Instantiate(prefab.asset);
    //}

    static IEnumerator<float> getSpiritHelper(string id, System.Action<Sprite> callback, bool isIcon)
    {

        string type = "";
        if (id.Contains("spirit"))
            type = "spirit";
        else if (id == "attack" || id.Contains("spell"))
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
            SpellDict spell = GetSpell(id);
            if (spell != null)
                id = spell.spellGlyph.ToString();
        }

        foreach (var item in loadedBundles[type])
        {
            if (item.Contains(id + ".png"))
            {
                var request = item.LoadAssetAsync(id + ".png", typeof(Sprite));
                Timing.WaitUntilDone(request);
                var tempSp = request.asset as Sprite;
                //spr.sprite = tempSp;
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
        else if (id.Contains("spell"))
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
            id = spellDictData[id].spellGlyph.ToString();

        foreach (var item in loadedBundles[type])
        {
            if (item.Contains(id + ".png"))
            {
                var request = item.LoadAssetAsync(id + ".png", typeof(Sprite));
                Timing.WaitUntilDone(request);
                var tempSp = request.asset as Sprite;
                spr.sprite = tempSp;
                if (isIcon)
                    IconSprites[tempSp.texture.name] = tempSp;
                else
                    AllSprites[tempSp.texture.name] = tempSp;

            }
        }

        yield return Timing.WaitForOneFrame;

    }

    public static IngredientDict GetIngredient(string id)
    {
        if (ingredientDictData.ContainsKey(id))
            return ingredientDictData[id];
        else
        {
            Debug.LogError($"Ingredient \"{id}\" not found.");
            return null;
        }
    }

    public static StoreDictData GetStoreItem(string id)
    {
        if (storeDict.ContainsKey(id))
            return storeDict[id];
        else
        {
            Debug.LogError($"StoreItem \"{id}\" not found.");
            return null;
        }
    }

    public static SpellDict GetSpell(string id)
    {
        if (id == null)
            id = "attack";
        if (spellDictData.ContainsKey(id))
            return spellDictData[id];
        else
        {
            Debug.LogError($"Spell \"{id}\" not found.");
            return null;
        }
    }

    public static ConditionDict GetCondition(string id)
    {
        if (conditionsDictData.ContainsKey(id))
        {
            return conditionsDictData[id];
        }
        else
        {
            Debug.LogError($"Condition \"{id}\" not found.");
            return null;
        }
    }

    public static SpiritDict GetSpirit(string id)
    {
        if (spiritDictData.ContainsKey(id))
        {
            return spiritDictData[id];
        }
        else
        {
            Debug.LogError($"Spirit \"{id}\" not found.");
            return null;
        }
    }

    public static IngredientDict GetCollectable(string id)
    {
        if (ingredientDictData.ContainsKey(id))
        {
            return ingredientDictData[id];
        }
        else
        {
            Debug.LogError($"Collectable \"{id}\" not found.");
            return null;
        }
    }
}





