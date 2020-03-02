using Newtonsoft.Json;

namespace Raincrow.BattleArena.Model
{
    public class ActionRequestType
    {
        public static readonly string Move = "move";
        public static readonly string Cast = "cast";
        public static readonly string Summon = "summon";
        public static readonly string Flee = "flee";
    }        

    public class FleeActionRequestModel : IActionRequestModel
    {
        public string Type => ActionRequestType.Flee;
    }

    public class MoveActionRequestModel : IActionRequestModel
    {
        public string Type => ActionRequestType.Move;
        public BattleSlot Position { get; set; }
    }

    public class CastSpellActionRequestModel : IActionRequestModel
    {
        public string Type => ActionRequestType.Cast;
        public string SpellId { get; set; }
        public string TargetId { get; set; }
        public InventoryItemModel[] Ingredients { get; set; }
    }

    public class SummonActionRequestModel : IActionRequestModel
    {
        public string Type => ActionRequestType.Summon;
        public string SpiritId { get; set; }
        public BattleSlot Position { get; set; }
    }

    public interface IActionRequestModel
    {
        string Type { get; }
    }

    public class ActionResultType
    {
        public static readonly string BanishSpirit = "battle.kill.spirit";
        public static readonly string BanishWitch = "battle.kill.character";
        public static readonly string CastSpell = "battle.cast";
        public static readonly string Summon = "battle.summon";
        public static readonly string Move = "battle.move";
        public static readonly string Flee = "battle.flee";
    }

    public class MoveActionResultModel : IActionResultModel
    {
        public BattleSlot Position { get; set; }
        public string Event =>  ActionResultType.Move;
    }

    public class CastSpellActionResultModel : IActionResultModel
    {
        public string Event => ActionResultType.CastSpell;
        public string SpellId { get; set; }
        public string TargetId { get; set; }
    }

    public class SummonResultActionModel : IActionResultModel
    {
        public string Event => ActionResultType.Summon;
        public string SpiritId { get; set; }
        public BattleSlot Position { get; set; }
    }

    public class FleeActionResultModel : IActionResultModel
    {
        public string Event => ActionResultType.Flee;
    }

    public interface IActionResultModel
    {
        string Event { get; }
    }
}