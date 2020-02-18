using System.Collections.Generic;

namespace Raincrow.BattleArena.Model
{    
    public interface IBattleModel
    {
        string Id { get; }        
        IGridModel Grid { get; }   
        IList<ICharacterModel> Characters { get; set; }
    }

    public interface IGridModel
    {
        int MaxCellsPerRow { get; }
        int MaxCellsPerColumn { get; }
        ICellModel[,] Cells { get; }
    }

    public interface ICellModel
    {
        string ObjectId { get; set; }
        int Height { get; }
        int X { get; }
        int Y { get; }
    }
}