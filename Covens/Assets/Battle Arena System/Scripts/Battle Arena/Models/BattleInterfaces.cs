using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public interface IBattleResultModel
    {
        string Type { get; set; }
        string[] Ranking { get; set; }
        IBattleRewardModel Reward { get; set; }
    }

    public interface IBattleRewardModel
    {
        InventoryItemModel[] Tools { get; set; }
        InventoryItemModel[] Herbs { get; set; }
        InventoryItemModel[] Gems { get; set; }
        InventoryItemModel[] Consumables { get; set; }
        int Experience { get; set; }
        int GoldCurrency { get; set; }
        int SilverCurrency { get; set; }
        int Degree { get; set; }
    }

    public interface IBattleModel
    {
        string Id { get; }
        IGridUIModel GridUI { get; }
    }

    public interface IGridGameObjectModel
    {
        CellUIController CellPrefab { get; }
        Vector2 Spacing { get; }
        Vector2 CellScale { get; }
    }

    public interface IGridUIModel
    {
        ICellUIModel[,] Cells { get; }
        int MaxCellsPerRow { get; }
        int MaxCellsPerColumn { get; }
        ICollection<ICharacterController<ISpiritModel, ISpiritUIModel>> SpiritsViews { get; }
        ICollection<ICharacterController<IWitchModel, IWitchUIModel>> WitchesViews { get; }
        IEnumerator<ICharacterController> SpawnObjectOnGrid(IObjectModel objectModel, int row, int col);
        void SetObjectToGrid(ICharacterController characterController, IObjectModel objectModel, int row, int col);
        void RemoveObjectFromGrid(ICharacterController characterController, IObjectModel objectModel);
        //void RecycleCharacter(GameObject character);       
    }

    public interface ICellUIModel
    {
        ICellModel CellModel { get; }
        bool IsSelected { get; set; }
        void Show(ICellModel cellModel, Vector2 cellScale);
        void SetIsSelected(bool value);
        Transform Transform { get; }
        //CellClickEvent OnCellClick { get; }
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