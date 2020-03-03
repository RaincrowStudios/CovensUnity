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
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    public class CastActionRequestModel : ActionRequestModel
    {
        [JsonProperty("spellId")]
        public string SpellId { get; set; }
        [JsonProperty("targetId")]
        public string TargetId { get; set; }
        [JsonProperty("ingredients")]
        public InventoryItemModel[] Ingredients { get; set; }
    }

    public class SummonActionRequestModel : ActionRequestModel
    {
        public string SpiritId { get; set; }
        public BattleSlot Position { get; set; }
    }

    [JsonConverter(typeof(ActionRequestModelConverter))]
    public class ActionRequestModel : IActionRequestModel
    {
        [JsonProperty("type")]
        public virtual string Type { get; set; }
    }

    public interface IActionRequestModel
    {
        string Type { get; }
    }

    public class ActionResultType
    {
        public const string Banish = "battle.kill";
        public const string Cast = "battle.cast";
        public const string Summon = "battle.summon";
        public const string Move = "battle.move";
        public const string Flee = "battle.flee";
    }

    public class BanishActionResultModel : ActionResultModel
    {
        public string TargetId { get; set; }
    }

    public class MoveActionResultModel : ActionResultModel
    {
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    public class CastActionResultModel : ActionResultModel
    {
        [JsonProperty("target")]
        public GenericCharacterObjectServer Target { get; set; }
        [JsonProperty("caster")]
        public GenericCharacterObjectServer Caster { get; set; }
    }

    public class SummonActionResultModel : ActionResultModel
    {
        public string SpiritId { get; set; }
        [JsonProperty("position")]
        public BattleSlot Position { get; set; }
    }

    public class FleeActionResultModel : ActionResultModel { }

    [JsonConverter(typeof(ActionResultModelConverter))]
    public class ActionResultModel : IActionResultModel
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
    }

    public interface IActionResultModel
    {        
        bool IsSuccess { get; set; }
    }

    #region Server Classes

    public class BattleActor
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("actionResolution")]
        public List<BattleAction> Actions { get; set; }
    }

    public class BattleAction
    {
        [JsonProperty("action")]
        public ActionRequestModel Request { get; set; }
        [JsonProperty("result")]
        public List<ActionResultModel> Results { get; set; }
    }

    #endregion    
}