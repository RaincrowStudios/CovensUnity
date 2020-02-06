namespace Raincrow.BattleArena.Model
{
    public interface ITurnModel
    {
        ICharacterModel[] Character { get; }
        int ActionsPerTurnCount { get; }
        float TurnLimit { get; }
    }
}
