using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;

public class JsonClasses : MonoBehaviour
{

}

//public class Result
//{
//    public int total { get; set; }
//    public int xpGain { get; set; }
//    public bool critical { get; set; }
//    public bool reflected { get; set; }
//    public string effect { get; set; }
//    public int resilienceChanged { get; set; }
//    public int newResilience { get; set; }
//    public int newPower { get; set; }
//    public int powerChanged { get; set; }
//    public int successChance { get; set; }
//    public int selfEnergy { get; set; }
//}

public class LocationBuff
{
    public string id { get; set; }
    public string type { get; set; }
    public string buff { get; set; }
    public string spiritId { get; set; }
    public string spellId { get; set; }
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
    public string spirit { get; set; }
    public double banishedOn { get; set; }
    public string dominion { get; set; }
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



public class Condition
{
    public string bearer { get; set; }
    public string instance { get; set; }
    public double expiresOn { get; set; }
    public string baseSpell { get; set; }
    public string status { get; set; }
    public int stacked { get; set; }
    public bool constant { get; set; }
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
}

public class Inventory
{
    public List<CosmeticData> cosmetics { get; set; }
    public List<Item> consumables { get; set; }
}

public class Item
{
    public int count { get; set; }
    public string id { get; set; }
}

public struct CollectableItem
{
    public string collectible;
    public int count;
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

public class ExploreLore
{
    public string id { get; set; }
    public float latitude { get; set; }
    public float longitude { get; set; }
}

public struct GardenData
{
    public string id;
    public float latitude;
    public float longitude;
    public int distance;
}

public class MoonData
{
    public double phase { get; set; }
    public double luminosity { get; set; }
    public double zenith { get; set; }
    public double moonRise { get; set; }
    public double moonSet { get; set; }
    public bool alwaysUp { get; set; }
    public bool alwaysDown { get; set; }
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
    public enum Target
    {
        SELF = 0,
        OTHER = 1,
        ANY = 2,
    }

    [DefaultValue("")]
    public string id;
    public int glyph;
    public int school;
    [DefaultValue("")]
    public string baseSpell;
    public bool common;
    public int cost;

    [DefaultValue(new string[0])]
    public string[] ingredients;
    [DefaultValue(new string[0])]
    public string[] states;
    public Target target;
    public int align;
    public bool pop;
    public bool hidden;
    
    [JsonIgnore]
    public string Name => LocalizeLookUp.GetSpellName(id);
    [JsonIgnore]
    public string SpiritDescription => LocalizeLookUp.GetSpellSpiritDescription(id);
    [JsonIgnore]
    public string PhysicalDescription => LocalizeLookUp.GetSpellPhyisicalDescription(id);
    [JsonIgnore]
    public string Lore => LocalizeLookUp.GetSpellLore(id);
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
    public spellIngredientsData()
    {

    }

    public spellIngredientsData(string id, int amount)
    {
        this.id = id;
        this.count = amount;
    }

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



public class SpiritInstance
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