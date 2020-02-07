namespace Raincrow.BattleArena.Model
{
    public interface IGridModel
    {
        int MaxCellsPerLine { get; }
        int MaxCellsPerColumn { get; }
        ICellModel[,] Cells { get; }
    }
}