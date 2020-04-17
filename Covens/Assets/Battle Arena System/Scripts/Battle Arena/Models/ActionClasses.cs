using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raincrow.BattleArena.Converters;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Model
{
    public class ActionRequestType
    {
        public const string Move = "move";
        public const string Cast = "cast";
        public const string Summon = "summon";
        public const string Flee = "flee";
    }

    public class FleeActionRequestModel : ActionRequestModel
    {
        [JsonProperty("type")]
        public override string Type => ActionRequestType.Flee;
    }

    public class MoveActionRequestModel : ActionRequestModel
    {
        [JsonProperty("type")]
        public override string Type => ActionRequestType.Move;
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    public class CastActionRequestModel : ActionRequestModel
    {
        [JsonProperty("type")]
        public override string Type => ActionRequestType.Cast;
        [JsonProperty("spellId")]
        public string SpellId { get; set; }
        [JsonProperty("targetId")]
        public string TargetId { get; set; }
        [JsonProperty("ingredients")]
        public InventoryItemModel[] Ingredients { get; set; }
    }

    public class SummonActionRequestModel : ActionRequestModel
    {
        [JsonProperty("type")]
        public override string Type => ActionRequestType.Summon;
        [JsonProperty("spiritId")]
        public string SpiritId { get; set; }
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    [JsonConverter(typeof(ActionRequestModelConverter))]
    public abstract class ActionRequestModel : IActionRequestModel
    {
        public abstract string Type { get; }
    }

    public interface IActionRequestModel
    {
        string Type { get; }
    }

    public class ActionResponseType
    {
        public const string Banish = "battle.kill";
        public const string Cast = "battle.cast";
        public const string Summon = "battle.summon";
        public const string Move = "battle.move";
        public const string Flee = "battle.flee";
        //public const string Join = "battle.join";
    }

    public class BanishActionResponseModel : ActionResponseModel
    {
        public override string Type => ActionResponseType.Banish;
        [JsonProperty("target")]
        public GenericCharacterObjectServer Target { get; set; }
        [JsonProperty("killedBy")]
        public GenericCharacterObjectServer KilledBy { get; set; }

    }

    //public class JoinActionResponseModel : ActionResponseModel
    //{
    //    public override string Type => ActionResponseType.Join;
    //    [JsonProperty("object")]
    //    public GenericCharacterObjectServer Object { get; set; }
    //    [JsonProperty("position")]
    //    public BattleSlot Position { get; set; }
    //}

    public class MoveActionResponseModel : ActionResponseModel
    {
        public override string Type => ActionResponseType.Move;
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    public class CastActionResponseModel : ActionResponseModel
    {
        public override string Type => ActionResponseType.Cast;
        [JsonProperty("target")]
        public GenericCharacterObjectServer Target { get; set; }
        [JsonProperty("caster")]
        public GenericCharacterObjectServer Caster { get; set; }
        [JsonProperty("spell")]
        public string Spell { get; set; }
        [JsonProperty("school")]
        public int School { get; set; }
        [JsonProperty("spellCost")]
        public int SpellCost { get; set; }
        [JsonProperty("cooldown")]
        public int Cooldown { get; set; }
        [JsonProperty("result")]
        public CastActionResultModel Result;
    }

    public class CastActionResultModel : ICastActionResultModel
    {
        [JsonProperty("damage")]
        public int EnergyChange { get; set; }
        [JsonProperty("isCritical")]
        public bool IsCritical { get; set; }
        [JsonProperty("xp")]
        public int XP { get; set; }
        [JsonProperty("alignment")]
        public int Alignment { get; set; }
    }

    public interface ICastActionResultModel
    {
        int EnergyChange { get; set; }
        bool IsCritical { get; set; }
        int XP { get; set; }
        int Alignment { get; set; }
    }

    public class SummonActionResponseModel : ActionResponseModel
    {
        public override string Type => ActionResponseType.Summon;
        [JsonProperty("spirit")]
        public GenericCharacterObjectServer Spirit { get; set; }
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
        [JsonProperty("summoningEnergy")]
        public int SummoningEnergy { get; set; }
    }

    public class FleeActionResponseModel : ActionResponseModel
    {
        public override string Type => ActionResponseType.Flee;
    }

    [JsonConverter(typeof(ActionResponseModelConverter))]
    public abstract class ActionResponseModel : IActionResponseModel
    {
        public abstract string Type { get; }
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
    }

    public interface IActionResponseModel
    {
        bool IsSuccess { get; set; }
        string Type { get; }
    }

    #region Server Classes

    public class BattleActor
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("actionResolution")]
        public List<BattleAction> Actions { get; set; }

        public BattleActor()
        {
            Actions = new List<BattleAction>();
        }
    }

    public class BattleAction
    {
        [JsonProperty("action")]
        public ActionRequestModel Request { get; set; }
        [JsonProperty("result")]
        public List<ActionResponseModel> Results { get; set; }

        public BattleAction()
        {
            Results = new List<ActionResponseModel>();
        }
    }

    #endregion    
}