
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Token
{
    //portal, spirit, duke, location, witch, summoningEvent, gem, herb, tool, silver, lore, energy
    private static readonly Dictionary<string, MarkerSpawner.MarkerType> m_TypeMap = new Dictionary<string, MarkerSpawner.MarkerType>
    {
        { "",               MarkerSpawner.MarkerType.NONE },
        { "portal",         MarkerSpawner.MarkerType.PORTAL },
        { "spirit",         MarkerSpawner.MarkerType.SPIRIT },
        { "duke",           MarkerSpawner.MarkerType.DUKE },
        { "location",       MarkerSpawner.MarkerType.PLACE_OF_POWER },
        { "character",      MarkerSpawner.MarkerType.WITCH },
        { "summoningEvent", MarkerSpawner.MarkerType.SUMMONING_EVENT },
        { "gem",            MarkerSpawner.MarkerType.GEM },
        { "herb",           MarkerSpawner.MarkerType.HERB },
        { "tool",           MarkerSpawner.MarkerType.TOOL },
        { "silver",         MarkerSpawner.MarkerType.SILVER },
        { "lore",           MarkerSpawner.MarkerType.LORE },
        { "energy",         MarkerSpawner.MarkerType.ENERGY }
    };
    public static MarkerSpawner.MarkerType TypeFromString(string type)
    {
        if (m_TypeMap.ContainsKey(type))
            return m_TypeMap[type];
        else
            return MarkerManager.MarkerType.NONE;
    }

    public string type;
    [JsonProperty("_id")]
    public string instance;
    public float longitude;
    public float latitude;

    [NonSerialized, JsonIgnore]
    public double lastEnergyUpdate;

    [JsonIgnore]
    public MarkerSpawner.MarkerType Type { get { return (type == null ? MarkerSpawner.MarkerType.NONE : m_TypeMap[type]); } }


    ////{ get; set; }
    //// { get; set; }
    //// { get; set; }
    //// { get; set; }
    ////{ get; set; }
    ////{ get; set; }
    //// { get; set; }
    ////{ get; set; }
    ////public string race { get; set; }
    ////public bool male { get { return race != null && race.StartsWith("m_"); } }
    ////public bool bot { get; set; }
    ////public int degree { get; set; }
    ////public float latitude { get; set; }
    ////public float longitude { get; set; }
    ////public bool physical { get; set; }
    ////public int position { get; set; }
    ////public HashSet<string> immunityList { get; set; }
    ////public int tier { get; set; }
    ////public int energy { get; set; }
    ////public int baseEnergy { get; set; }
    ////public int amount { get; set; }
    ////public int level { get; set; }

    [JsonIgnore]
    public string Id => instance;
    [JsonIgnore]
    public int position => 0;
}

public class CollectableToken : Token
{
    public string collectible;
    public int amount;
}

public class CharacterToken : Token
{
    public int baseEnergy;
    public int energy;
    public string state;
    public int level;
    //public string[] immunities;
    public int degree;
    public string coven;
}

public class SpiritToken : CharacterToken
{
    [JsonProperty("spirit")]
    public string spiritId;
    public string owner;

}

public class WitchToken : CharacterToken
{
    [JsonProperty("name")]
    public string displayName;
    public List<EquippedApparel> equipped;
    public int bodyType;

    [JsonIgnore]
    public bool male => (bodyType >= 3);
}

public class PopToken : Token
{
    public string owner;
}
