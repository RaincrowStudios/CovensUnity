namespace Raincrow.BattleArena.Model
{
    public interface IBattleArenaGridModel
    {
        int MaxCellsPerLine { get; }
        int MaxCellsPerColumn { get; }
        IBattleArenaCellModel[,] Cells { get; }
    }
}