
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class JsonClasses : MonoBehaviour
{

}

public class WebSocketResponse
{
	public string command{ get; set; }
	public string instance{ get; set; }
	public string spell{ get; set; }
	public int energy { get; set; }
	public double expiresOn { get; set; }
	public string state { get; set; }
	public string type { get; set; }
	public string caster { get; set; }
	public string action { get; set; }
	public string target { get; set; }
	public string spirit { get; set; }
	public int xp { get; set; }
	public int degree { get; set; }
	public Result result { get; set; }
	public int targetEnergy { get; set; }
	public string targetStatus { get; set; }
	public InteractionType iType;
	public Token token { get; set;}

    public string member { get; set; }
    public string covenName { get; set; }
    public string coven { get; set; }
    public string newTitle { get; set; }
    public int newRole { get; set; }
    public string displayName { get; set; }
    public int level { get; set; }
    public string inviteToken { get; set; }
}

public class Result
{
	public int total { get; set; }
	public int xpGain { get; set; }
	public bool critical { get; set; }
	public bool reflected { get; set; }
	public string effect{ get; set;}
}

public class Token
{
	public string instance { get; set; }
	public string displayName { get; set; }
	public string coven { get; set; }
	public string state { get; set; }
	public string type { get; set; }
	public bool male { get; set; }
	public int degree { get; set; }
	public float latitude { get; set; }
	public float longitude { get; set; }
	public bool physical { get; set; }
	public HashSet<string> immunityList { get; set;}

	[NonSerialized] 
	public GameObject Object;
	[NonSerialized] 
	public float scale;
	[NonSerialized] 		
	public MarkerSpawner.MarkerType Type;
}

public class Signature
{
	public string id { get; set; }
	public string baseSpell { get; set; }
	public int cost { get; set; }
	public List<string> types { get; set; }
	public List<string> states { get; set; }
	public List<Gathered> ingredients { get; set; }
}

public class MarkerDataDetail
{
	public string displayName{ get; set; }
	public string id{ get; set; }
	public string instance{ get; set; }
	public string worldRank{ get; set; }
	public string state{ get; set; }
	public string covenStatus{ get; set; }
	public string type{ get; set; }
	public bool male { get; set; }
	public string favoriteSpell{ get; set; }
	public List<object> achievements { get; set; }
	public int energy{ get; set; }
	public int baseEnergy{ get; set; }
	public string dominion{ get; set; }
	public bool gender{ get; set; }
	public string coven{ get; set; }
	public int degree{ get; set; }
	public int level{ get; set; }
	public int xp{ get; set; }
	public int xpGain{ get; set; }
	public int silver{ get; set; }
	public int gold { get; set; }
	public string description{ get; set; }
	public double summonOn{ get; set; }
	public double createdOn{ get; set; }
	public double expireOn{ get; set; }
	public string owner{ get; set; }
	public string lastAttackedBy{ get; set; }
	public string lastHealedBy{ get; set; }
	public string ownerCoven{ get; set; }
	public int count{ get; set; }
	public List<Conditions> conditions { get; set; }
	[NonSerialized]
	public Dictionary<string,Conditions> conditionsDict = new Dictionary<string, Conditions>();
	[NonSerialized]
	public Dictionary<string,CoolDown> cooldownDict = new Dictionary<string, CoolDown>();
	public List<string> weaknesses { get; set; }
	public Ingredients ingredients { get; set;}
	public Inventory inventory { get; set; }
	public List<SpellData> spells { get; set;}
	[NonSerialized]
	public Dictionary<string,SpellData> spellsDict = new Dictionary<string, SpellData>();
	public List<string> validSpells { get; set;}
	public Equipped equipped {get;set;}
	public List<Signature> signatures { get; set;}
	public List<CoolDown> cooldownList{get; set;}
}

public class Conditions
{
	public string bearerInstance { get; set;}
	public string description { get; set; }
	public string conditionInstance { get; set; }
	public string condition { get; set; }
	public string spellID { get; set; }
	public string status { get; set; }
	public double expiresOn { get; set; }
}

public class CoolDown
{
	public string instance { get; set;}
	public string spell { get; set;}
	public double expiresOn { get; set;}
}

public class InventoryData
{
	public Dictionary<string,int> herbs{ get; set; }
	public Dictionary<string,int> tool { get; set; }
	public Dictionary<string,int> gems { get; set; }
}

[SerializeField]
public class MapAPI
{
	public string characterName{ get; set; }
	public string target{ get; set; }
	public string type { get; set; }
	public bool physical{ get; set; }
	public float longitude{ get; set; }
	public float latitude{ get; set; }
}

#region Login

public class PlayerCharacterCreateAPI
{
	public string displayName{ get; set; }
	public bool male{ get; set; }
	public double longitude{ get; set; }
	public double latitude{ get; set; }
}

[Serializable]
public class PlayerLoginAPI
{
	public string username{ get; set; }
	public string password{ get; set; }
	public string game{ get; set; }
	public string email{ get; set; }
	public double lng{ get; set; }
	public double lat{ get; set; }
	public string UID{ get; set; }
}

[Serializable]
public class PlayerLoginCallback
{
	public string token{ get; set; }
	public string wsToken{ get; set; }
	public Account account{ get; set; }
	public MarkerDataDetail character{ get; set; }
	public Config config { get; set;}
}

public class Ingredients
{
	public List<InventoryItems> gems { get; set; }
	public List<InventoryItems> tools { get; set; }
	public List<InventoryItems> items { get; set; }
	public List<InventoryItems> herbs { get; set; }
	public Dictionary<string,InventoryItems> herbsDict = new Dictionary<string, InventoryItems>();
	public Dictionary<string,InventoryItems> toolsDict = new Dictionary<string, InventoryItems>();
	public Dictionary<string,InventoryItems> gemsDict = new Dictionary<string, InventoryItems>();
}
public class Inventory
{
	public string[] cosmetics { get; set; }
	public ConsumableItem[] consumables { get; set; }
}
public class ConsumableItem
{
	public int count { get; set; }
	public string id { get; set; }
}

public class InventoryItems
{
	public string displayName{ get; set;}
	public int count { get; set;}
	public int rarity { get; set;}
	public string id { get; set;}
	public string name{ get; set;}
	//	public EnumWardrobeCategory Type { get; set;}
}

[Serializable]
public class Account
{
	public string username{ get; set; }
	public string email{ get; set; }
}

public class Config
{
	public float interactionRadius { get; set; }
	public float displayRadius { get; set; }
	public List<SummoningMatrix> summonginMatrix{ get; set;}
}

public class SummoningMatrix{
	public string spirit{ get; set;}
	public string tool{get;set;}
}

[Serializable]
public class PlayerResetCallback
{
	public string email{ get; set; }
}

[SerializeField]
public class PlayerPasswordCallback
{
	public string token{ get; set; }
}

[SerializeField]
public class PlayerResetAPI
{
	public string username{ get; set; }
	public string code{ get; set; }
	public string token{ get; set; }
	public string password{ get; set; }
}

#endregion

public class MarkerAPI
{
	public List<Token> tokens{ get; set; }
}

public class SpellData
{
	public string id { get; set; }
	public string displayName { get; set; }
	public int school { get; set; }
	public int level { get; set; }
	public int cost { get; set; }
	public string range { get; set; }
	public string casting { get; set; }
	public List<string> types { get; set; }
	public string description { get; set; }
	public List<string> states { get; set; }
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
	public List<spellIngredientsData> ingredients{ get; set;}
}

public class spellIngredientsData
{
	public string id { get; set;}
	public int count { get; set;}
}


public class AttackData
{
	public bool success { get; set; }
	public bool crit { get; set; }
	public bool resist { get; set; }
	public int damage { get; set; }
	public int currentEnergy{ get; set;}
	public int amountResisted { get; set; }
	public int xp { get; set;}
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



public class SpiritData{
	public string id{ get; set; }
	public string instance{ get; set; }
	public double summonOn{ get; set; }
	public double createdOn{ get; set; }
	public double banishedOn{ get; set; }
	public double expireOn{ get; set; }
	public double lat{ get; set; }
	public double lng{ get; set; }
	public string location{ get; set; }
	public string state{ get; set; }
	public string lastAttackedBy{ get; set; }
	public string spirit{ get; set; }
	public int xpGained{ get; set;}
	public int degree{ get; set;}
	public int energy{ get; set;}
	public List<Gathered> gathered { get; set;}
	public List<Gathered> ingredients { get; set;}
}

public class Gathered{
	public string id{ get; set;}
	public string type{ get; set;}
	public int count{ get; set;}
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
//



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

	public CovenMember[] members { get; set;}
	public CovenOverview[] allies { get; set; }
	public CovenOverview[] alliedCovens { get; set; }
}


public class CovenMember
{
    public string player { get; set;}
    public string character{ get; set;}
    public long joinedOn{ get; set;}
    public long lastActiveOn { get; set;}
    public long leftOn { get; set;}
    public int role{ get; set;}
    public string displayName{ get; set;}
    public string title{ get; set;}
    public string state{ get; set;}
    public int level{ get; set;}
    public string degree{ get; set;}
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
	public string target { get; set;}
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
