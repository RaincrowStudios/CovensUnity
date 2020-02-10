using Raincrow.BattleArena.Controller;

namespace Raincrow.BattleArena.Model
{
    public interface IBattleModel
    {
        string Id { get; }
        IGridModel Grid { get; }
        AbstractGameMasterController GameMaster { get; }
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