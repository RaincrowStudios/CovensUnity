using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class JsonClasses : MonoBehaviour
{

}


public class Result
{
    public int total { get; set; }
    public int xpGain { get; set; }
    public bool critical { get; set; }
    public bool reflected { get; set; }
    public string effect { get; set; }
}

public class Token
{
    public string instance { get; set; }
    public string owner { get; set; }
    public string displayName { get; set; }
    public string coven { get; set; }
    public string state { get; set; }
    public string type { get; set; }
    public string spiritType { get; set; }
    public string spiritId { get; set; }
    public string race { get; set; }
    public bool male { get; set; }
    public Dictionary<string, EquippedApparel> equipped { get; set; }
    public bool bot { get; set; }
    public int degree { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    public bool physical { get; set; }
    public int position { get; set; }
    public HashSet<string> immunityList { get; set; }
    public int tier { get; set; }
    public int energy { get; set; }
    public int amount { get; set; }
    public int level { get; set; }
    [NonSerialized]
    public GameObject Object;
    [NonSerialized]
    public float scale;
    [NonSerialized]
    public MarkerSpawner.MarkerType Type;
}

// public class Signature
// {
//     public string id { get; set; }
//     public string baseSpell { get; set; }
//     public int cost { get; set; }
//     public List<string> types { get; set; }
//     public List<string> states { get; set; }
//     public List<Gathered> ingredients { get; set; }
// }

public class LastAttackDetail
{
    string instance;
    string type;
}

public class MarkerDataDetail
{
    public float latitude { get; set; }
    public float longitude { get; set; }
    public int spiritCount { get; set; }
    public string controlledBy { get; set; }
    public double rewardOn { get; set; }
    public bool physicalOnly { get; set; }
    public bool isCoven { get; set; }
    public bool dailyBlessing { get; set; }
    public bool full { get; set; }
    public string displayName { get; set; }
    public string id { get; set; }
    public string instance { get; set; }
    public string worldRank { get; set; }
    public string dominionRank { get; set; }
    public string state { get; set; }
    public string covenStatus { get; set; }
    public string type { get; set; }
    public bool male { get; set; }
    public string favoriteSpell { get; set; }
    public List<object> achievements { get; set; }
    public int energy { get; set; }
    public int baseEnergy { get; set; }
    public int xpToLevelUp { get; set; }
    public string dominion { get; set; }
    public string race { get; set; }
    public string coven { get; set; }
    public string covenName { get; set; }
    public string covenTitle { get; set; }
    public int degree { get; set; }
    public int level { get; set; }
    public int xp { get; set; }
    public int xpGain { get; set; }
    public int silver { get; set; }
    public int gold { get; set; }
    public string description { get; set; }
    public string nemesis { get; set; }
    public string benefactor { get; set; }
    public double summonOn { get; set; }
    public double createdOn { get; set; }
    public double expireOn { get; set; }
    public string owner { get; set; }
    public LastAttackDetail lastAttackedBy { get; set; }
    public string lastHealedBy { get; set; }
    public string ownerCoven { get; set; }
    public int count { get; set; }
    public List<Conditions> conditions { get; set; }
    public Dictionary<string, CoolDown> cooldownDict = new Dictionary<string, CoolDown>();
    public List<string> weaknesses { get; set; }
    public Ingredients ingredients { get; set; }
    public Inventory inventory { get; set; }
    public List<SpellData> spells { get; set; }
    public Dictionary<string, SpellData> spellsDict = new Dictionary<string, SpellData>();
    public List<string> validSpells { get; set; }
    public List<EquippedApparel> equipped { get; set; }
    public List<CoolDown> cooldownList { get; set; }
    public Dailies dailies { get; set; }
    public Blessing blessing;
    [NonSerialized]
    public HashSet<string> KnownSpiritsList = new HashSet<string>();
    public List<KnownSpirits> knownSpirits { get; set; }
    public Firsts firsts { get; set; }
    public Dictionary<string, KnownSpirits> knownSpiritsDict = new Dictionary<string, KnownSpirits>();
}

public class HeatMapPoints
{
    public int count { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public class Firsts
{
    public bool locationReward { get; set; }
    public bool portal { get; set; }
    public bool flight { get; set; }
    public bool collect { get; set; }
    public bool cast { get; set; }
    public bool purchaseGold { get; set; }
    public bool locationSummon { get; set; }
    public bool purchaseSilver { get; set; }
    public bool purchaseMoney { get; set; }
    public bool kill { get; set; }
    public bool portalSummon { get; set; }
    public bool banish { get; set; }
    public bool kyteler { get; set; }
}


public class KnownSpirits
{
    public string id { get; set; }
    public List<string> tags { get; set; }
    public double banishedOn { get; set; }
    public string location { get; set; }
}

public class Blessing
{
    public int daily { get; set; }
    public int lunar { get; set; }
    public int locations { get; set; }
    public int moonPhase { get; set; }
}

public class Sun
{
    public double sunRise { get; set; }
    public double sunSet { get; set; }
}

public class Spellcraft
{
    public string id { get; set; }
    public string ingredient { get; set; }
    public string relation { get; set; }
    public string type { get; set; }
    public string location { get; set; }
    public int amount { get; set; }
    public int count { get; set; }
    public bool complete { get; set; }
}

public class Gather
{
    public string id { get; set; }
    public string type { get; set; }
    public int amount { get; set; }
    public string location { get; set; }
    public int count { get; set; }
    public bool complete { get; set; }
}

public class Explore
{
    public string id { get; set; }
    public int count { get; set; }
    public bool complete { get; set; }
}

public class Dailies
{
    public bool collected { get; set; }
    public long expiresOn { get; set; }
    public Spellcraft spellcraft { get; set; }
    public Gather gather { get; set; }
    public Explore explore { get; set; }
}

public class AnalyticsSession
{
    public string SessionId { get; set; }
}

public class Rewards
{
    public int silver { get; set; }
    public int energy { get; set; }
    public int gold { get; set; }

}



public class Conditions
{
    public string bearer { get; set; }
    public string instance { get; set; }
    public string id { get; set; }
    public double expiresOn { get; set; }
    public string baseSpell { get; set; }
    public string status { get; set; }
}

public class CoolDown
{
    public string instance { get; set; }
    public string spell { get; set; }
    public double expiresOn { get; set; }
}

public class InventoryData
{
    public Dictionary<string, int> herbs { get; set; }
    public Dictionary<string, int> tool { get; set; }
    public Dictionary<string, int> gems { get; set; }
}

[SerializeField]
public class MapAPI
{
    public string characterName { get; set; }
    public string target { get; set; }
    public string type { get; set; }
    public bool physical { get; set; }
    public float longitude { get; set; }
    public float latitude { get; set; }
}

#region Login

public class PlayerCharacterCreateAPI
{
    public string displayName { get; set; }
    public bool male { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    public string characterSelection { get; set; }
}

[Serializable]
public class PlayerLoginAPI
{
    public string username { get; set; }
    public string language { get; set; }
    public string password { get; set; }
    public string game { get; set; }
    public string email { get; set; }
    public double longitude { get; set; }
    public double latitude { get; set; }
    public string UID { get; set; }
}

[Serializable]
public class PlayerLoginCallback
{
    public string token { get; set; }
    public string wsToken { get; set; }
    public Account account { get; set; }
    public MarkerDataDetail character { get; set; }
    public Config config { get; set; }
}

public class Ingredients
{
    public List<InventoryItems> gems { get; set; }
    public List<InventoryItems> tools { get; set; }
    public List<InventoryItems> items { get; set; }
    public List<InventoryItems> herbs { get; set; }
    public Dictionary<string, InventoryItems> herbsDict = new Dictionary<string, InventoryItems>();
    public Dictionary<string, InventoryItems> toolsDict = new Dictionary<string, InventoryItems>();
    public Dictionary<string, InventoryItems> gemsDict = new Dictionary<string, InventoryItems>();

    public int Amount(string ingredientId)
    {
        if (herbsDict.ContainsKey(ingredientId))
            return herbsDict[ingredientId].count;

        if (toolsDict.ContainsKey(ingredientId))
            return toolsDict[ingredientId].count;

        if (gemsDict.ContainsKey(ingredientId))
            return gemsDict[ingredientId].count;

        return 0;
    }

    public void Add(string id, int amount)
    {
        if (herbsDict.ContainsKey(id))
            herbsDict[id].count += amount;

        if (toolsDict.ContainsKey(id))
            toolsDict[id].count += amount;

        if (gemsDict.ContainsKey(id))
            gemsDict[id].count += amount;
    }

    public void GetIngredient(string id, out InventoryItems item, out IngredientType type)
    {
        if (herbsDict.ContainsKey(id))
        {
            item = herbsDict[id];
            type = IngredientType.herb;
            return;
        }

        if (toolsDict.ContainsKey(id))
        {
            item = toolsDict[id];
            type = IngredientType.tool;
            return;
        }

        if (gemsDict.ContainsKey(id))
        {
            item = gemsDict[id];
            type = IngredientType.gem;
            return;
        }

        item = null;
        type = IngredientType.none;
    }
}
public class Inventory
{
    public List<ApparelData> cosmetics { get; set; }
    public List<ConsumableItem> consumables { get; set; }
}
public class ConsumableItem
{
    public int count { get; set; }
    public string id { get; set; }
}

public class InventoryItems
{
    public string displayName { get; set; }
    public int count { get; set; }
    public int rarity { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    //	public EnumWardrobeCategory Type { get; set;}
}

public class KytelerItem
{
    public string id;
    public double discoveredOn;
    public string location;
    public string ownerName;
}

[Serializable]
public class Account
{
    public string username { get; set; }
    public bool ftf { get; set; }
    public string email { get; set; }
    public bool character { get; set; }
}

public class Config
{
    //	public float dictionary { get; set;}
    public float interactionRadius { get; set; }
    public int idleTimeLimit { get; set; }
    public float displayRadius { get; set; }
    public List<SummoningMatrix> summoningMatrix { get; set; }
    public MoonData moon { get; set; }
    public int tribunal { get; set; }
    public int daysRemaining { get; set; }
    public string dominion { get; set; }
    public string strongestWitch { get; set; }
    public string strongestCoven { get; set; }
    public List<int> summoningCosts { get; set; }
    public List<HeatMapPoints> heatmap { get; set; }
    public List<GardenData> gardens { get; set; }
    public Sun sun { get; set; }
    public ExploreLore explore { get; set; }
}

public class ExploreLore
{
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public class GardenData
{
    public string id { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
    public int distance { get; set; }
}

public class MoonData
{
    public double phase { get; set; }
    public double moonRise { get; set; }
    public double moonSet { get; set; }
    public bool alwaysUp { get; set; }
    public bool alwaysDown { get; set; }
    public double luminosity { get; set; }

}

public class SummoningMatrix
{
    public string spirit { get; set; }
    public List<int> zone { get; set; }
    public string tool { get; set; }
    public string herb { get; set; }
    public string gem { get; set; }
}

[Serializable]
public class PlayerResetCallback
{
    public string email { get; set; }
}

[SerializeField]
public class PlayerPasswordCallback
{
    public string token { get; set; }
}

[SerializeField]
public class PlayerResetAPI
{
    public string username { get; set; }
    public string code { get; set; }
    public string token { get; set; }
    public string password { get; set; }
}

#endregion

public class MarkerAPI
{
    public markerLocation location { get; set; }
    public List<Token> tokens { get; set; }
}

public class markerLocation
{
    public int music { get; set; }
    public string dominion { get; set; }
    public string garden { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public int zone { get; set; }
}

public class SpellData
{
    public string id { get; set; }
    public string baseSpell { get; set; }
    public string displayName { get; set; }
    public int school { get; set; }
    public int level { get; set; }
    public bool unlocked { get; set; }
    public int cost { get; set; }
    public string range { get; set; }
    public string casting { get; set; }
    public string description { get; set; }
    public string lore { get; set; }
    public List<string> states { get; set; }
    public string[] ingredients { get; set; }
    public string gem = "";
    public string herb = "";
    public string tool = "";
}


public class Equipped
{
    public string hat { get; set; }
    public string hair { get; set; }
    public string neck { get; set; }
    public string dress { get; set; }
    public string wristRight { get; set; }
    public string wristLeft { get; set; }
    public string handRight { get; set; }
    public string handLeft { get; set; }
    public string fingerRight { get; set; }
    public string fingerLeft { get; set; }
    public string waist { get; set; }
    public string legs { get; set; }
    public string feet { get; set; }
    public string carryOns { get; set; }
    public string skinFace { get; set; }
    public string skinShoulder { get; set; }
    public string skinChes { get; set; }
}

public class SpellTargetData
{
    public string spell { get; set; }
    public string target { get; set; }
    public List<spellIngredientsData> ingredients { get; set; }
}

public class spellIngredientsData
{
    public string id { get; set; }
    public int count { get; set; }
}


public class AttackData
{
    public bool success { get; set; }
    public bool crit { get; set; }
    public bool resist { get; set; }
    public int damage { get; set; }
    public int currentEnergy { get; set; }
    public int amountResisted { get; set; }
    public int xp { get; set; }
    public InteractionType type;
}

public enum InteractionType
{
    AttackCrit, AttackNormal, AttackResist, AttackFail, TargetEscape, TargetDied, Hit, Resist, Death, TargetImmune, TargetSilenced, TargetProtected
}

public enum CurrentView
{
    MapView, IsoView, TransitionView
}



public class SpiritData
{
    public string id { get; set; }
    public string instance { get; set; }
    public double summonOn { get; set; }
    public double createdOn { get; set; }
    public double banishedOn { get; set; }
    public double expiresOn { get; set; }
    public double lat { get; set; }
    public double lng { get; set; }
    public string location { get; set; }
    public string state { get; set; }
    public LastAttackedBy lastAttackedBy { get; set; }
    public string spirit { get; set; }
    public int xpGained { get; set; }
    public int degree { get; set; }
    public int energy { get; set; }
    public int attacked { get; set; }
    public int zone { get; set; }
    public int gathered { get; set; }
    public List<Gathered> ingredients { get; set; }
    public SpiritDeckUIManager.type deckCardType { get; set; }
}

public class LastAttackedBy
{
    public string instance { get; set; }
    public string type { get; set; }
}

public class Gathered
{
    public string id { get; set; }
    public string type { get; set; }
    public int count { get; set; }

}

#region coven
// requests
public class CovenRequest_Ally
{
    public string covenName { get; set; }
    public string coven { get; set; }
}
public class CovenRequest_DisplayByName
{
    public string covenName { get; set; }
}
public class CovenRequest_DisplayById
{
    public string covenInstance { get; set; }
}
public class CovenRequest_Invite
{
    public string invited { get; set; }
    public string invitedName { get; set; }
}
public class CovenRequest_Join
{
    public string inviteToken { get; set; }
}
public class CovenRequest_Kick
{
    public string kickedName { get; set; }
    public string kicked { get; set; }
}
public class CovenRequest_Promote
{
    public string promotedName { get; set; }
    public string promoted { get; set; }
    public int promotion { get; set; }
}
public class CovenRequest_Requests
{
    public string coven { get; set; }
    public string covenName { get; set; }
}
public class CovenRequest_Title
{
    public string titled { get; set; }
    public string titledName { get; set; }
    public string title { get; set; }
}
public class CovenRequest_Unally
{
    public string coven { get; set; }
    public string covenName { get; set; }
    public string title { get; set; }
}
public class CovenRequest_ByName
{
    public string covenName { get; set; }
}
public class CovenRequest_ByInstance
{
    public string coven { get; set; }
}

public class PlayerRequestData
{
    public string playerName { get; set; }
}
public class CovenPromoteRequestData
{
    public string covenName { get; set; }
    //public int role { get; set; }
    public string playerName { get; set; }
}
public class CovenPlayerRequestData
{
    //public string covenName { get; set; }
    public string request { get; set; }
}
public class CovenChangeTitleRequestData
{
    public string covenName { get; set; }
    public string playerName { get; set; }
    public string title { get; set; }
}
public class CovenData
{
    public string coven { get; set; }
    public string covenName { get; set; }
    public int? score { get; set; }
    public int rank { get; set; }
    public string createdBy { get; set; }
    public long createdOn { get; set; }
    public long disbandedOn { get; set; }
    public string dominion { get; set; }
    public int? dominionRank { get; set; }

    public CovenMember[] members { get; set; }
    public CovenOverview[] allies { get; set; }
    public CovenOverview[] alliedCovens { get; set; }
}


public class CovenMember
{
    public string player { get; set; }
    public string character { get; set; }
    public long joinedOn { get; set; }
    public long lastActiveOn { get; set; }
    public long leftOn { get; set; }
    public int role { get; set; }
    public string displayName { get; set; }
    public string title { get; set; }
    public string state { get; set; }
    public int level { get; set; }
    public string degree { get; set; }
}


public class CovenOverview
{
    public string covenName { get; set; }
    public string inviteToken { get; set; }
    public string coven { get; set; }
    public int members { get; set; }
    public int rank { get; set; }
    public long wasAlliedOn { get; set; }
    public long alliedOn { get; set; }
    public long invitedOn { get; set; }
    public long date { get { return alliedOn > 0 ? alliedOn : invitedOn; } }
}
public class CovenInvite
{
    public CovenOverview[] invites;
}

public class FindUserRequest
{
    public string playerName { get; set; }
    public bool hasCoven { get; set; }
}
public class FindRequest
{
    public string query { get; set; }
}
public class FindResponse
{
    public string[] matches { get; set; }
}

public class MemberInvite
{
    public MemberOverview[] requests;
    public MemberOverview[] invites;
}
public class MemberOverview
{
    public string character { get; set; }
    public string displayName { get; set; }
    public int level { get; set; }

    // invites
    public long invitedOn;
    public string inviteToken;
}


#endregion


#region Inventory

public class Inventory_Consume
{
    public string consumable { get; set; }
}
public class Inventory_Display
{
    public string consumable { get; set; }
}
public class Inventory_Equip
{
    public Equipped equipped { get; set; }
}

#endregion

public class TargetMarkerDetailData
{
    public string target { get; set; }
}

#region shop

public enum EnumCurrency
{
    None,
    Gold,
    Silver,
    IAP,
}

public class Shop_Purchase
{
    public string purchaseItem { get; set; }
    public int amount { get; set; }
    public string currency { get; set; }
}
public class Shop_PurchaseSilver
{
    public string id { get; set; }
}




public partial class Shop_DisplayResponse
{
    public ShopBundle[] items { get; set; }
}
public partial class ShopBundle
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string Type { get; set; }
    public Content[] Contents { get; set; }
    public long SilverCost { get; set; }
    public long GoldCost { get; set; }
}
public partial class Content
{
    public string Id { get; set; }
    public long Count { get; set; }
}


#endregion