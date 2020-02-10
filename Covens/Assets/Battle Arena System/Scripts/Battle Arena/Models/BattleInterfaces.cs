namespace Raincrow.BattleArena.Model
{
    public interface IBattleModel
    {
        IGridModel Grid { get; }
    }

    public interface IGridModel
    {
        int MaxCellsPerLine { get; }
        int MaxCellsPerColumn { get; }
        ICellModel[,] Cells { get; }
    }

    public interface ICellModel
    {
        int Height { get; }
    }
}