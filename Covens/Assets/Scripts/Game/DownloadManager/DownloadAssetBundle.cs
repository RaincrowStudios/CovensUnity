
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
using Raincrow.Store;

public static class DownloadAssetBundle
{
    public static string baseURL = "https://storage.googleapis.com/raincrow-covens/";
    public static bool isDictLoaded = false;
}

#region json classes
public class AssetCacheJson
{
    public List<string> bundles { get; set; }
}

public struct ConditionData
{
    [JsonProperty("spellId")]
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
    public int[] zones;

    public string Name { get => LocalizeLookUp.GetSpiritName(id); }
    public string Location { get => string.IsNullOrEmpty(legend) ? LocalizeLookUp.GetText("chat_world") : LocalizeLookUp.GetText(legend); }
    public string Type { get => LocalizeLookUp.GetText(type); }
    public string Behavior { get => LocalizeLookUp.GetSpiritBehavior(id); }
    public string Description { get => LocalizeLookUp.GetSpiritDesc(id); }
}

public struct PlaceOfPowerSettings
{
    public struct EntryCost
    {
        public int gold;
        public int silver;
    }

    public int[] openTimeWindow;
    public int[] cooldownWindow;
    public List<EntryCost> entryCosts;
}

public class GameSettingsData
{
    public int[] summoningCosts;
    public float idleTimeLimit;
    //public float interactionRadius;
    [JsonProperty("witchVideos")]
    public string[] witchSchool;
    public long[] alignment;
    [JsonProperty("xp")]
    public ulong[] exp;
    public int[] baseEnergy;
    public float[] lunarEfficiency;

    [JsonProperty("placeOfPower")]
    public PlaceOfPowerSettings PlaceOfPower;

    [JsonProperty("spells")]
    public Dictionary<string, SpellData> Spells;

    [JsonProperty("spirits")]
    public Dictionary<string, SpiritData> Spirits;
    
    [JsonProperty("collectibles")]
    public Dictionary<string, IngredientData> Collectibles;

    [JsonProperty("conditions")]
    public Dictionary<string, ConditionData> Conditions;

    [JsonProperty("cosmetics")]
    public Dictionary<string, CosmeticData> Cosmetics;

    [JsonProperty("bundles")]
    public Dictionary<string, List<ItemData>> Bundles;

    [JsonProperty("consumables")]
    public Dictionary<string, ConsumableData> Consumables;

    [JsonProperty("silver")]
    public Dictionary<string, CurrencyBundleData> Silver;
}

public struct IngredientData
{
    private static readonly Dictionary<string, IngredientType> m_CollectableTypeMap = new Dictionary<string, IngredientType>
    {
        { "",   IngredientType.none },
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
#endregion