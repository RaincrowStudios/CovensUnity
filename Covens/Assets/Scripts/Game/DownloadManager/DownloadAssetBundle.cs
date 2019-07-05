
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

public class ConditionDict
{
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

    public string spellDescriptionPhysical { get; set; }

    public int spellSchool { get; set; }
}

public class SpiritData
{
    public string type;
    public int tier;
    public int reward;
    public string tool;
    public string herb;
    public string gem;

    [JsonProperty("legend")]
    public string lore;
    public int[] zones;
}

public class GameDictionary
{
    public Dictionary<string, SpiritData> Spirits;
}

public class DictMatrixData
{
    public Dictionary<string, SpellDict> Spells { get; set; }

    public Dictionary<string, SpellFeedbackData> SpellFeedback { get; set; }

    public Dictionary<string, SpiritData> Spirits { get; set; }

    public Dictionary<string, ConditionDict> Conditions { get; set; }

    public Dictionary<string, IngredientDict> Collectibles { get; set; }

    public Dictionary<string, StoreDictData> Store { get; set; }

    public Dictionary<string, LocalizeData> Quest { get; set; }

    public Dictionary<string, LocalizeData> CountryCodes { get; set; }

    public List<LocalizeData> LoadingTips { get; set; }

    public Dictionary<string, LocalizeData> SpiritTypes { get; set; }

    public Dictionary<string, LocalizeData> WitchSchool { get; set; }

    public Dictionary<string, LocalizeData> Gardens { get; set; }

    public Dictionary<string, LocalizeData> Other { get; set; }
    public Dictionary<string, LocalizeData> Zone { get; set; }

    public List<LocalizeData> FTFDialogues { get; set; }
}

public class IngredientDict
{
    public string description { get; set; }

    public string hint { get; set; }

    public int rarity { get; set; }

    public string name { get; set; }

    public string type { get; set; }

    public string spirit { get; set; }

    public bool forbidden = false;
}

public class StoreDictData
{
    public string title { get; set; }

    public string subtitle { get; set; }

    public string onBuyTitle { get; set; }

    public string onBuyDescription { get; set; }

    public string onConsumeDescription { get; set; }
}

public class LocalizeData
{
    public string value { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}

public class SpellFeedbackData
{
    public string asCaster { get; set; }
    public string asTarget { get; set; }
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