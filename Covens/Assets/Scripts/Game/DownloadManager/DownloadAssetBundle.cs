
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

public class GameDictionary
{
    public Dictionary<string, SpiritData> Spirits;
}

public class GameSettingsData
{
    public int[] summoningCosts;
    public float idleTimeLimit;
    public float interactionRadius;
    [JsonProperty("witchVideos")]
    public string[] witchSchool;
    public long[] alignment;
    public ulong[] exp;
    public int[] baseEnergy;

    public Dictionary<string, SpellData> Spells;
    public Dictionary<string, SpiritData> Spirits;
    public Dictionary<string, GardenData> Gardens;
    public Dictionary<string, IngredientData> Collectibles;
    public Dictionary<string, ConditionData> Conditions;
    public Dictionary<string, CosmeticData> Cosmetics;
}

public struct IngredientData
{
    private static readonly Dictionary<string, IngredientType> m_CollectableTypeMap = new Dictionary<string, IngredientType>
    {
        { null, IngredientType.none  },
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