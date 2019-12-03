using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Token
{
    private static readonly Dictionary<string, MarkerSpawner.MarkerType> m_TypeMap = new Dictionary<string, MarkerSpawner.MarkerType>
    {
        { "",               MarkerSpawner.MarkerType.NONE },
        { "portal",         MarkerSpawner.MarkerType.PORTAL },
        { "spirit",         MarkerSpawner.MarkerType.SPIRIT },
        { "duke",           MarkerSpawner.MarkerType.DUKE },
        { "placeOfPower",   MarkerSpawner.MarkerType.PLACE_OF_POWER },
        { "character",      MarkerSpawner.MarkerType.WITCH },
        { "summoningEvent", MarkerSpawner.MarkerType.SUMMONING_EVENT },
        { "gem",            MarkerSpawner.MarkerType.GEM },
        { "herb",           MarkerSpawner.MarkerType.HERB },
        { "tool",           MarkerSpawner.MarkerType.TOOL },
        { "silver",         MarkerSpawner.MarkerType.SILVER },
        { "lore",           MarkerSpawner.MarkerType.LORE },
        { "energy",         MarkerSpawner.MarkerType.ENERGY },
        { "boss",           MarkerSpawner.MarkerType.BOSS },
        { "loot",           MarkerSpawner.MarkerType.LOOT },
    };

    public static MarkerSpawner.MarkerType TypeFromString(string type)
    {
        if (type == null)
            return MarkerManager.MarkerType.NONE;

        if (m_TypeMap.ContainsKey(type))
            return m_TypeMap[type];

        return MarkerManager.MarkerType.NONE;
    }

    public virtual string type { get; set; }
    [JsonProperty("_id")]
    public virtual string instance { get; set; }
    public virtual float longitude { get; set; }
    public virtual float latitude { get; set; }

    [NonSerialized, JsonIgnore]
    public double lastEnergyUpdate;

    [JsonIgnore]
    public MarkerSpawner.MarkerType Type { get { return (type == null ? MarkerSpawner.MarkerType.NONE : m_TypeMap[type]); } }

    [JsonIgnore]
    public virtual string Id => instance;

    public int position;
    public int island;
    public List<int> islands;   // For the Guardian Spirit in pops, I get an array of islands it can attack.
    [JsonIgnore]
    public int popIndex
    {
        get
        {
            if (island >= 0) return island * 3 + position;
            else
                return -1;
        }
    }

}

public class CollectableToken : Token
{
    public string collectible;
    public int amount;
}

public class CharacterToken : Token
{
    public virtual int energy { get; set; }
    public virtual string state { get; set; }
    public virtual int level { get; set; }
    public virtual HashSet<string> immunities { get; set; }
    public virtual int degree { get; set; }
    public virtual string coven { get; set; }

    public virtual int baseEnergy { get; set; }

    public virtual List<StatusEffect> effects { get; set; }

    [JsonIgnore]
    public virtual int maxEnergy => (2 * baseEnergy);


    public bool HasStatus(string status)
    {
        if (effects == null)
            return false;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].modifiers.status == null)
                continue;

            for (int j = 0; j < effects[i].modifiers.status.Count; j++)
            {
                if (effects[i].modifiers.status[j] == status)
                    return true;
            }
        }

        return false;
    }

    public bool HasEffect(string spell)
    {
        if (effects == null)
            return false;

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].spell == spell)
                return true;
        }

        return false;
    }
}

public class SpiritToken : CharacterToken
{
    [JsonProperty("spirit")]
    public string spiritId;
    public string owner;
    [JsonProperty("worldBossFight")]
    public bool IsBossSummon;
}

public class WitchToken : CharacterToken
{
    [JsonProperty("name")]
    public virtual string displayName { get; set; }
    public virtual List<EquippedApparel> equipped { get; set; }
    public virtual int bodyType { get; set; }

    [JsonIgnore]
    public bool male => (bodyType >= 3);

    public override int baseEnergy
    {
        get
        {
            if (level - 1 < PlayerDataManager.baseEnergyPerLevel.Length)
                return PlayerDataManager.baseEnergyPerLevel[level - 1];
            return PlayerDataManager.baseEnergyPerLevel[PlayerDataManager.baseEnergyPerLevel.Length - 1];
        }
    }
}

public class BossToken : CharacterToken
{
    [JsonProperty("spirit")]
    public string spiritId;
}

public class LootToken : Token
{

}



public class PopToken : Token
{
    public override string type => "placeOfPower";
    public int tier;
    public PopLastOwnedBy lastOwnedBy;
}

public class PopLastOwnedBy
{
    [JsonProperty("_id")]
    public string instance;
    public int degree;
    [JsonProperty("name")]
    public string displayName;
    public List<EquippedApparel> equipped;
}

public class EnergyToken : Token
{
    public override string type => "energy";
    public int amount;
}

public class PlayerToken : WitchToken
{
    //witch token
    public override string displayName => PlayerDataManager.playerData.name;
    public override int bodyType => PlayerDataManager.playerData.bodyType;

    public override List<EquippedApparel> equipped
    {
        get => PlayerDataManager.playerData.equipped;
        set => PlayerDataManager.playerData.equipped = value;
    }

    //character token
    public override int energy
    {
        get => PlayerDataManager.playerData.energy;
        set => PlayerDataManager.playerData.energy = value;
    }

    public override int baseEnergy
    {
        get => PlayerDataManager.playerData.baseEnergy;
        set => PlayerDataManager.playerData.baseEnergy = value;
    }

    public override string state
    {
        get => PlayerDataManager.playerData.state;
        set => PlayerDataManager.playerData.state = value;
    }

    public override int level
    {
        get => PlayerDataManager.playerData.level;
        set => PlayerDataManager.playerData.level = value;
    }

    public override HashSet<string> immunities
    {
        get => PlayerDataManager.playerData.immunities;
        set => PlayerDataManager.playerData.immunities = value;
    }

    public override int degree
    {
        get => PlayerDataManager.playerData.degree;
        set => PlayerDataManager.playerData.degree = value;
    }

    //base tokem
    public override string type
    {
        get => "character";
    }

    public override string instance => PlayerDataManager.playerData.instance;

    public override float longitude
    {
        get => PlayerDataManager.playerData.longitude;
        set => PlayerDataManager.playerData.longitude = value;
    }

    public override float latitude
    {
        get => PlayerDataManager.playerData.latitude;
        set => PlayerDataManager.playerData.latitude = value;
    }

    public override List<StatusEffect> effects
    {
        get => PlayerDataManager.playerData.effects;
        set => PlayerDataManager.playerData.effects = value;
    }
}