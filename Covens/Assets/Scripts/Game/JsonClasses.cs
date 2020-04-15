using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;

public enum IngredientType
{
    gem, tool, herb, none
}

public class LocationBuff
{
    public string id { get; set; }
    public string type { get; set; }
    public string buff { get; set; }
    public string spiritId { get; set; }
    public string spellId { get; set; }
}


public class KnownSpirits
{
    public string spirit { get; set; }
    public double banishedOn { get; set; }
    public string dominion { get; set; }
}

public struct Sun
{
    public double sunRise { get; set; }
    public double sunSet { get; set; }
}

public class AnalyticsSession
{
    public string SessionId { get; set; }
}

public struct DailyRewards
{
    public int silver;
    public int energy;
    public int gold;
    public StatusEffect effect;
}

#region Login

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
    
    public void AddCosmetic(string id)
    {
        var data = DownloadedAssets.GetCosmetic(id);

        if (data == null)
            return;

        bool owned = false;
        foreach (var cosmetic in PlayerDataManager.playerData.inventory.cosmetics)
        {
            if (cosmetic.id == id)
            {
                owned = true;
                break;
            }
        }

        if (!owned)
            cosmetics.Add(data);
    }
}

public class Item
{
    public int count { get; set; }
    public string id { get; set; }
}

public struct CollectableItem
{
    [JsonProperty("collectible")]
    public string id;
    public int count;

    public CollectableItem(string id, int count)
    {
        this.id = id;
        this.count = count;
    }
}

public struct CollectibleData
{
    public int amount;
    [JsonProperty("collectible")]
    public string id;
    public string type;
}

public struct ConsumableItem
{
    [JsonProperty("consumable")]
    public string id;
    public int amount;

    public ConsumableItem(string id, int count)
    {
        this.id = id;
        this.amount = count;
    }
}

public class KytelerItem
{
    public string id;
    public double discoveredOn;
    public string location;
    public string ownerName;
}

public struct GardenData
{
    public float latitude;
    public float longitude;
    public int distance;
}

public struct MoonData
{
    public double phase { get; set; }
    public double luminosity { get; set; }
    public double zenith { get; set; }
    public double moonRise { get; set; }
    public double moonSet { get; set; }
    public bool alwaysUp { get; set; }
    public bool alwaysDown { get; set; }
}

#endregion

public class SpellData
{
    public enum Target
    {
        SELF = 0,
        OTHER = 1,
        ANY = 2,
    }

    public enum TargetType
    {
        ANY = 0,
        WITCH = 1,
        SPIRIT = 2,
    }

    public const string SILENCED_STATUS = "silenced";
    public const string BOUND_STATUS = "bound";
    public const string CHANNELING_STATUS = "channeling";
    public const string CHANNELED_STATUS = "channeled";

    [DefaultValue("")]
    public string id;
    public int glyph;
    public int school;
    [DefaultValue("")]
    public string baseSpell;
    public bool common;
    public int cost;
    //public int xp;
    public int level;
    public int cooldownBattle;
    public int maxCooldownBattle;

    public List<string> ingredients;
    public Target target;
    public int align;
    public bool pop;
    public bool beneficial;
    public bool hidden;
    public float cooldown;

    public List<string> states;
    public List<string> targetStatus;
    public TargetType targetType;
    public List<int> targetSchool;
    public List<int> casterSchool;

    [JsonIgnore]
    public string Name => LocalizeLookUp.GetSpellName(id);
    [JsonIgnore]
    public string Description => LocalizeLookUp.GetSpellSpiritDescription(id)
                                    .Replace("{{minDamage}}", "")
                                    .Replace("{{maxDamage}}", "")
                                    .Replace("{{duration}}", "")
                                    .Replace("{{maxStacks}}", "")
                                    .Replace("{{levelRequired}}", level.ToString());
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

public class spellIngredientsData
{
    public spellIngredientsData()
    {

    }

    public spellIngredientsData(string id, int amount)
    {
        this.collectible = id;
        this.count = amount;
    }

    public string collectible { get; set; }
    public int count { get; set; }
}


public struct SpiritInstance
{    
    [JsonProperty("spirit")]
    public string id;                       //spirit_barghest
    [JsonProperty("_id")]
    public string instance;                 //database id
    public double summonOn;                 //summon timestamp
    public double banishedOn;               //discovered timestamp
    public double expiresOn;                //expire timestamp
    public double lat;                      //latitude
    public double lng;                      //longitude
    public string location;                 //discovered dominion
    public int xpGained;
    public int degree;
    public int energy;
    public int attacked;                    //number of witches the spirit attacked
    public int gathered;                    //number of collectables the spirit picked up

    //public double createdOn { get; set; }
    //public string state { get; set; }
    //public LastAttackedBy lastAttackedBy { get; set; }
    //public List<Gathered> ingredients { get; set; }

    [JsonIgnore]
    public string spirit => id;
    [JsonIgnore]
    public SpiritDeckUIManager.type deckCardType;
    [JsonIgnore]
    public int zone;                        // ?
}

#region coven
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
    public string name { get; set; }
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

    public List<CovenMember> members { get; set; }
    public List<CovenOverview> allies { get; set; }
    public List<CovenOverview> alliedCovens { get; set; }
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
    public List<string> matches { get; set; }
}

public class MemberInvite
{
    public List<MemberOverview> requests;
    public List<MemberOverview> invites;
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

#region shop

public struct ItemData
{
    public string id;
    public int count;
}

#endregion