
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerRank
{
    public int global;
    public int dominion;
}

public struct StatusEffect
{
    public struct Modifier
    {
        public string status;
        public int resilience;
        public int power;
        public int aptitude;
        public int wisdom;
        public int beCrit;
    }
    
    public string spell;
    public float duration;
    public bool buff;
    public Modifier modifiers;
    public int stack;
    public int stackable;
    public double expiresOn;
}

public abstract class MarkerData
{
    public abstract MarkerSpawner.MarkerType Type { get; }
}

public class LocationMarkerData : MarkerData
{
    public override MarkerSpawner.MarkerType Type => MarkerSpawner.MarkerType.PLACE_OF_POWER;

    public int level;
    public string displayName;
    public string locationType;
    public bool physicalOnly;
    public bool full;
    public bool isCoven;
    public string controlledBy;
    public double rewardOn;
    public string herb;
    public string gem;
    public string tool;
    public Buff buff;
    public string spiritId;
    public double takenOn;
    public bool limitReached;

    public class Buff
    {
        public string id;
        public string type;
        public string spellId;
        public string spiritId;
        public int buff;
    }
}

public abstract class CharacterMarkerData : MarkerData
{
    public virtual string state { get; set; }
    public virtual int energy { get; set; }
    public virtual int degree { get; set; }
    public virtual int level { get; set; }
    public virtual int power { get; set; }
    public virtual int resilience { get; set; }

    public List<StatusEffect> effects;
    
    [JsonIgnore]
    public virtual string covenId { get; }

    [JsonIgnore]
    public int baseEnergy
    {
        get
        {
            if (level < PlayerDataManager.baseEnergyPerLevel.Length)
                return PlayerDataManager.baseEnergyPerLevel[level];
            return energy;
        }
    }
}

public class WitchMarkerData : CharacterMarkerData
{
    public override MarkerSpawner.MarkerType Type => MarkerSpawner.MarkerType.WITCH;

    public virtual string dominion { get; set; }
    public virtual string name { get; set; }
    public virtual int bodyType { get; set; }
    public virtual float latitude { get; set; }
    public virtual float longitude { get; set; }
    public virtual int worldRank { get; set; }
    public virtual int dominionRank { get; set; }

    public virtual List<EquippedApparel> equipped { get; set; }
    
    [JsonIgnore]
    public virtual bool male { get => bodyType >= 3; }
}

public class SpiritMarkerData : CharacterMarkerData
{
    public override MarkerSpawner.MarkerType Type => MarkerSpawner.MarkerType.SPIRIT;

    public virtual string id { get; set; }
    public virtual string owner { get; set; }
    public virtual double createdOn { get; set; }
    public virtual double expiresOn { get; set; }
    public virtual int bounty { get; set; }
}

public struct CovenInfo
{
    public string coven;
    public string name;
    public int role;
    public long joinedOn;
    public string title;
}

public struct QuestStatus
{
    public struct QuestProgress
    {
        public int count;
        public bool completed;
    }

    public QuestsController.CovenDaily daily;
    public bool completed;
    public QuestProgress spell;
    public QuestProgress gather;
    public QuestProgress explore;
}

public class CovenRequest
{
    public double date;
    public string coven;
    public string name;
    public int worldRank;
}

public class CovenInvite
{
    public double date;
    public string coven;
    public string name;
    public int worldRank;
}

public class PlayerData : WitchMarkerData
{
    [JsonProperty("_id")] public string instance;
    public string account;
    public string physicalDominion;
    public ulong xp;
    public int alignment;
    public bool whiteMastery;
    public bool shadowMastery;
    public bool greyMastery;
        
    public int silver;
    public int gold;
    public int foxus;
    public int ward;
    public int favor;
    public int aptitude;
    public int wisdom;
    public bool tutorial;

    public string favoriteSpell;
    public string race;
    public bool dailyBlessing;
    public string benefactor;
    public string nemesis;

    public CovenInfo covenInfo;
    public QuestStatus quest;

    [JsonProperty("tools")] private List<CollectableItem> m_Tools;
    [JsonProperty("herbs")] private List<CollectableItem> m_Herbs;
    [JsonProperty("gems")] private List<CollectableItem> m_Gems;

    [JsonIgnore] private Dictionary<string, int> m_HerbsDict = null;
    [JsonIgnore] private Dictionary<string, int> m_ToolsDict = null;
    [JsonIgnore] private Dictionary<string, int> m_GemsDict = null;

    [JsonProperty("cosmetics")] private List<string> m_Cosmetics;
    public List<string> spirits;
    public List<KnownSpirits> knownSpirits;
    public List<CovenInvite> covenInvites;
    public List<CovenRequest> covenRequests;
    public HashSet<string> immunities;

    [JsonIgnore]
    public ulong xpToLevelUp
    {
        get
        {
            if (level < PlayerDataManager.xpToLevelup.Length)
                return PlayerDataManager.xpToLevelup[level];// - xp;

            return 0;
        }
    }
    
    public void Setup()
    {
        m_HerbsDict = new Dictionary<string, int>();
        m_ToolsDict = new Dictionary<string, int>();
        m_GemsDict = new Dictionary<string, int>();

        foreach (var item in m_Herbs)
            m_HerbsDict[item.collectible] = item.count;
        foreach (var item in m_Tools)
            m_ToolsDict[item.collectible] = item.count;
        foreach (var item in m_Gems)
            m_GemsDict[item.collectible] = item.count;
        
        Debug.LogError("TODO: GET BLESSINGS");
        blessing = new Blessing { };

        Debug.LogError("TODO: WATCHED VIDEOS");
        firsts = new Firsts { };
    }

    public int GetIngredient(string id)
    {
        if (string.IsNullOrEmpty(id))
            return 0;

        IngredientData data = DownloadedAssets.GetCollectable(id);

        if (data.Type == IngredientType.gem && m_GemsDict.ContainsKey(id))
            return m_GemsDict[id];

        if (data.Type == IngredientType.herb && m_HerbsDict.ContainsKey(id))
            return m_HerbsDict[id];

        if (data.Type == IngredientType.tool && m_ToolsDict.ContainsKey(id))
            return m_ToolsDict[id];

        return 0;
    }

    public void SetIngredient(string id, int amount)
    {
        if (string.IsNullOrEmpty(id))
            return;

        IngredientData data = DownloadedAssets.GetCollectable(id);

        if (data.Type == IngredientType.gem)
            m_GemsDict[id] = Mathf.Max(0, amount);

        else if (data.Type == IngredientType.herb)
            m_HerbsDict[id] = Mathf.Max(0, amount);

        else if (data.Type == IngredientType.tool)
            m_ToolsDict[id] = Mathf.Max(0, amount);
    }

    public void AddIngredient(string id, int amount)
    {
        if (string.IsNullOrEmpty(id))
            return;

        IngredientData data = DownloadedAssets.GetCollectable(id);

        if (data.Type == IngredientType.gem)
        {
            int current = m_GemsDict.ContainsKey(id) ? m_GemsDict[id] : 0;
            m_GemsDict[id] = Mathf.Max(0, current + amount);
        }

        else if (data.Type == IngredientType.herb)
        {
            int current = m_HerbsDict.ContainsKey(id) ? m_HerbsDict[id] : 0;
            m_HerbsDict[id] = Mathf.Max(0, current + amount);
        }

        else if (data.Type == IngredientType.tool)
        {
            int current = m_ToolsDict.ContainsKey(id) ? m_ToolsDict[id] : 0;
            m_ToolsDict[id] = Mathf.Max(0, current + amount);
        }
    }

    public List<CollectableItem> GetAllIngredients(IngredientType type)
    {
        Dictionary<string, int> dict;
        if (type == IngredientType.gem)
            dict = m_GemsDict;
        else if (type == IngredientType.herb)
            dict = m_HerbsDict;
        else if (type == IngredientType.tool)
            dict = m_ToolsDict;
        else
            dict = new Dictionary<string, int>();

        List<CollectableItem> result = new List<CollectableItem>();
        foreach(var pair in dict)
        {
            if (pair.Value <= 0)
                continue;

            result.Add(new CollectableItem
            {
                collectible = pair.Key,
                count = pair.Value
            });
        }

        return result;
    }

    public void SubIngredient(string id, int amount)
    {
        AddIngredient(id, -amount);
    }

    [JsonIgnore]
    public Inventory inventory
    {
        get
        {
            Inventory inv = new Inventory()
            {
                consumables = new List<Item>(),
                cosmetics = new List<CosmeticData>()
            };

            CosmeticData cosmeticData;
            foreach (var id in m_Cosmetics)
            {
                cosmeticData = DownloadedAssets.GetCosmetic(id);
                if (cosmeticData != null && cosmeticData.hidden == false)
                    inv.cosmetics.Add(cosmeticData);
            }

            return inv;
        }
    }
    
    [JsonIgnore]
    public Blessing blessing;
    
    [JsonIgnore]
    public Firsts firsts;

    [JsonIgnore]
    public double lastEnergyUpdate;

    [JsonIgnore]
    public int avatar => bodyType;

    [JsonIgnore]
    public List<SpellData> Spells
    {
        get
        {
            List<SpellData> spells = new List<SpellData>();
            var allSpells = DownloadedAssets.spellDictData.Values;
            foreach (var spellData in allSpells)
            {
                if (spellData.hidden)
                    continue;
                spells.Add(spellData);
            }
            return spells;
        }
    }

    [JsonIgnore]
    public long minAlignment
    {
        get
        {
            int absDegree = Mathf.Abs(degree);
            if (degree < 0)
                return PlayerDataManager.alignmentPerDegree[absDegree];
            else
                return PlayerDataManager.alignmentPerDegree[absDegree + 1];
        }
    }

    [JsonIgnore]
    public long maxAlignment
    {
        get
        {
            int absDegree = Mathf.Abs(degree);
            if (degree < 0)
                return PlayerDataManager.alignmentPerDegree[absDegree + 1];
            else
                return PlayerDataManager.alignmentPerDegree[absDegree];
        }
    }

    [JsonIgnore]
    public override string covenId => covenInfo.coven;
}

//map select
public class SelectWitchData_Map : WitchMarkerData
{
    public string coven;
    public new int power;
    public new int resilience;
    public PlayerRank rank;

    [JsonIgnore]
    public WitchToken token;
    
    public override MarkerSpawner.MarkerType Type => MarkerSpawner.MarkerType.WITCH;


    //temp fix to avoid replacing all WitchMarkerData references
    [JsonIgnore]
    public override string state => token.state; 
    [JsonIgnore]
    public override int energy => token.energy;
    //[JsonIgnore]
    //public override int baseEnergy => token.baseEnergy;
    [JsonIgnore]
    public override int degree => token.degree;
    [JsonIgnore]
    public override int level => token.level;
    //[JsonIgnore]
    //public override string covenName => coven;
    [JsonIgnore]
    public override string dominion => "?";
    [JsonIgnore]
    public override string name => token.displayName;
    [JsonIgnore]
    public override int bodyType => token.bodyType;
    [JsonIgnore]
    public override List<EquippedApparel> equipped => token.equipped;
    [JsonIgnore]
    public override float latitude => token.latitude;
    [JsonIgnore]
    public override float longitude => token.longitude;
    [JsonIgnore]
    public override int dominionRank => rank.dominion;
    [JsonIgnore]
    public override int worldRank => rank.global;

    [JsonIgnore]
    public override bool male => bodyType >= 3;

    [JsonIgnore]
    public override string covenId => coven;
}

public class SelectSpiritData_Map : SpiritMarkerData
{
    public override double createdOn { get; set; }
    public override string owner { get; set; }
    public string coven { get; set; }
    public override int power { get; set; }
    public override int resilience { get; set; }
    public override int bounty { get; set; }

    [JsonIgnore]
    public SpiritToken token;

    [JsonIgnore]
    public override string state => token.state;

    [JsonIgnore]
    public override int energy => token.energy;
    //[JsonIgnore]
    //public override int baseEnergy => token.baseEnergy;
    [JsonIgnore]
    public override int degree => token.degree;
    [JsonIgnore]
    public override int level => token.level;
    //[JsonIgnore]
    //public override string covenName => coven;
}