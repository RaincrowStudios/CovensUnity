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
	public string status { get; set; }
	public int xp { get; set; }
	public Result result { get; set; }
	public int targetEnergy { get; set; }
	public string targetStatus { get; set; }
	public InteractionType iType;
	public Token token { get; set;}
}

public class Result
{
	public int total { get; set; }
	public bool critical { get; set; }
	public bool resist { get; set; }
	public string conditions { get; set; }
}

public class Token
{
	public string displayName{ get; set; }
	public string summoner{ get; set; }
	public string creator{ get; set; }
	public string instance{ get; set; }
	public bool male{ get; set; }
	public string type { get; set; }
	public string subtype { get; set; }
	public string command { get; set; }
	public float latitude{ get; set; }
	public float longitude{ get; set; }
	public int degree{ get; set; }
	public string target { get; set; }
	public bool dead { get; set; }
	public int distance { get; set; }
	[NonSerialized] 
 	public GameObject Object;
	[NonSerialized] 
	public MarkerSpawner.MarkerType Type;
}
	
public class MarkerDataDetail
{
	public string displayName{ get; set; }
	public string instance{ get; set; }
	public string worldRank{ get; set; }
	public string covenStatus{ get; set; }
	public string type{ get; set; }
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
	public string description{ get; set; }
	public double summonOn{ get; set; }
	public double createdOn{ get; set; }
	public double expireOn{ get; set; }
	public string owner{ get; set; }
	public string ownerCoven{ get; set; }
	public int count{ get; set; }
	public List<Conditions> conditions { get; set; }
	public List<string> weaknesses { get; set; }
	public bool immune { get; set; }
	public Inventory ingredients { get; set;}
	public List<SpellData> spellBook { get; set;}
	public List<string> validSpells { get; set;}
	public Equipped equipped {get;set;}
}

public class Conditions
{
	public string instance { get; set; }
	public string id { get; set; }
	public string displayName { get; set; }
	public string caster { get; set; }
	public long expiresOn { get; set; }
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

[Serializable]
public class PlayerLoginAPI
{
	public string username{ get; set; }
	public string password{ get; set; }
	public string game{ get; set; }
	public double lng{ get; set; }
	public double lat{ get; set; }
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

public class Inventory
{
	public List<InventoryItems> gems { get; set; }
	public List<InventoryItems> tools { get; set; }
	public List<InventoryItems> items { get; set; }
	public List<InventoryItems> herbs { get; set; }
	public Dictionary<string,InventoryItems> herbsDict = new Dictionary<string, InventoryItems>();
	public Dictionary<string,InventoryItems> toolsDict = new Dictionary<string, InventoryItems>();
	public Dictionary<string,InventoryItems> gemsDict = new Dictionary<string, InventoryItems>();
}

public class InventoryItems
{
	public string displayName{ get; set;}
	public int count { get; set;}
	public string id { get; set;}
	public string family { get; set;}
	public string description { get; set;}
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
	public object cost { get; set; }
	public string range { get; set; }
	public string description { get; set; }

}

public class Skin
{
	public string face { get; set; }
	public string shoulder { get; set; }
	public string chest { get; set; }
	public string arm { get; set; }
}

public class Equipped
{
	public string outfit { get; set; }
	public string head { get; set; }
	public string hair { get; set; }
	public string neck { get; set; }
	public string chest { get; set; }
	public string wristLt { get; set; }
	public string wristRt { get; set; }
	public string hands { get; set; }
	public string fingerLt { get; set; }
	public string fingerRt { get; set; }
	public string waist { get; set; }
	public string legs { get; set; }
	public string feet { get; set; }
	public string carryOnLt { get; set; }
	public string carryOnRt { get; set; }
	public Skin skin { get; set; }
}

public class SpellTargetData
{
	public string spell { get; set; }
	public string target { get; set; }
	public List<string> ingredients{ get; set;}
}

public class SpellTargetChannelData
{
	public string spell { get; set; }
	public int energy { get; set; }
	public string target { get; set; }
	public List<string> ingredients{ get; set;}
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

public enum Spells
{
	spell_hex,
	spell_sunEater,
	spell_bind,
	spell_sealShadow,
	spell_leech,
	spell_resurrection,
	spell_greaterHex,
	spell_shadowTablet,
	spell_bless,
	spell_whiteFlame,
	spell_silence,
	spell_sealLight,
	spell_lightJudgement,
	spell_grace,
	spell_slowBurn,
	spell_blindingAura,
	spell_radiance,
	spell_dispel,
	spell_invisibility,
	spell_aradiaFavor,
	spell_abremelinOil,
	spell_abremelinBalm,
	spell_foolBargain,
	spell_mortalCoil,
	spell_deeSeal,
	spell_trueSight,
	spell_mirrors
}

#region coven

public class CovenDataAPI
{
	public string instanceID { get; set;}
    public string covenName { get; set; }
}

public class CovenData
{
	public List<CovenItem> players { get; set;}
}

public class CovenItem
{
	public string playerName{ get; set;}
	public string playerLevel{ get; set;}
	public int degree{ get; set;}
	public string rank{ get; set;}
	public bool isCreator{ get; set;}
}


#endregion