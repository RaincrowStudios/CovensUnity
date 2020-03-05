using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{    
    public interface IBattleModel
    {
        string Id { get; }
        IGridUIModel GridUI { get; }        
    }

    public interface IGridUIModel
    {
        ICellUIModel[,] Cells { get; }
        int MaxCellsPerRow { get; }
        int MaxCellsPerColumn { get; }
        ICollection<ICharacterController<ISpiritModel, ISpiritUIModel>> SpiritsViews { get; }
        ICollection<ICharacterController<IWitchModel, IWitchUIModel>> WitchesViews { get; }
        IEnumerator SpawnObjectOnGrid(IObjectModel objectModel, int row, int col);
        void SetObjectToGrid(IObjectUIModel objectUIModel, IObjectModel objectModel, int row, int col);
        void RemoveObjectFromGrid(IObjectUIModel objectUIModel, IObjectModel objectModel);
        void RecycleCharacter(GameObject character);
    }

    public interface ICellUIModel
    {
        void Show(ICellModel cellModel, Vector2 cellScale);
        Transform Transform { get; }
        CellClickEvent OnCellClick { get; }
    }

    public interface IGridModel
    {
        int MaxCellsPerRow { get; }
        int MaxCellsPerColumn { get; }
        ICellModel[,] Cells { get; }
        void SetObjectToGrid(IObjectModel objectModel, int row, int col);
        void RemoveObjectFromGrid(IObjectModel objectModel);
    }

    public interface ICellModel
    {
        string ObjectId { get; set; }
        int Height { get; }
        int X { get; }
        int Y { get; }
        bool IsEmpty();
    }
}