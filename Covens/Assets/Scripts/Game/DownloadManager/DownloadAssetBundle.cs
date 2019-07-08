
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.ComponentModel;

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

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance = this;
    }
    
    public static IEnumerator AnimateDownloadingText()
    {
        string downloadText = LocalizeLookUp.GetText("download");
        float delay = .5f;
        Instance.downloadingTitle.text = downloadText;
        yield return new WaitForSeconds(delay);
        Instance.downloadingTitle.text = downloadText + " .";
        yield return new WaitForSeconds(delay);
        Instance.downloadingTitle.text = downloadText + " . .";
        yield return new WaitForSeconds(delay);
        Instance.downloadingTitle.text = downloadText + " . . .";
        Instance.StartCoroutine(AnimateDownloadingText());
    }

    public static void SetMessage(string message, string submessage)
    {
        Instance.downloadingTitle.overflowMode = TextOverflowModes.Overflow;
        Instance.downloadingTitle.text = message;
        Instance.downloadingInfo.text = submessage;
    }
}

#region json classes
public class AssetCacheJson
{
    public List<string> bundles { get; set; }
}

public struct ConditionData
{
    public string spellID;
}

public struct SpiritData
{
    public string id;
    [DefaultValue("")]
    public string type;
    public int tier;
    public int reward;
    public string tool;
    [DefaultValue("")]
    public string herb;
    [DefaultValue("")]
    public string gem;
    [DefaultValue("")]
    public string legend;
    [DefaultValue(new int[0])]
    public int[] zones;

    public string Name { get => LocalizeLookUp.GetSpiritName(id); }
    public string Location { get => LocalizeLookUp.GetText(legend); }
    public string Type { get => LocalizeLookUp.GetText(type); }
    public string Behavior { get => LocalizeLookUp.GetSpiritBehavior(id); }
    public string Description { get => LocalizeLookUp.GetSpiritDesc(id); }
}

public class GameDictionary
{
    public Dictionary<string, SpiritData> Spirits;
}

public class DictMatrixData
{
    public Dictionary<string, SpellData> Spells { get; set; }
    public Dictionary<string, SpiritData> Spirits;
    public Dictionary<string, GardenData> Gardens;
    public Dictionary<string, IngredientData> Collectibles;
    public Dictionary<string, ConditionData> Conditions;
    public string[] witchVideos;
}

public struct IngredientData
{
    private static readonly Dictionary<string, IngredientType> m_CollectableTypeMap = new Dictionary<string, IngredientType>
    {
        { "", IngredientType.none },
        { "herb", IngredientType.herb },
        { "tool", IngredientType.tool },
        { "gem", IngredientType.gem}
    };

    public int rarity;
    public string type;
    public bool forbidden;

    [JsonIgnore]
    public IngredientType Type => m_CollectableTypeMap[type];
}

public class LocalizeData
{
    public string value { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}

public class AssetResponse
{
    public string dictionary { get; set; }
    public List<string> assets { get; set; }
    public string version { get; set; }
    public int android { get; set; }
    public int apple { get; set; }
    public bool maintenance { get; set; }
}
#endregion