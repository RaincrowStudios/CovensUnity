using Raincrow.BattleArena.Views;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public class ActionType
    {
        public static readonly string Move = "move";
        public static readonly string Cast = "cast";
        public static readonly string Summon = "summon";
        public static readonly string Flee = "flee";
    }

    public interface IActionModel
    {
        string Type { get; }
    }

    public class FleeActionModel : IActionModel
    {
        public string Type => ActionType.Flee;
    }

    public class MoveActionModel : IActionModel
    {
        public string Type => ActionType.Move;
        public BattleSlot Position { get; set; }
    }

    public class CastSpellActionModel : IActionModel
    {
        public string Type => ActionType.Cast;
        public string SpellId { get; set; }
        public string TargetId { get; set; }
        public InventoryItemModel[] Ingredients { get; set; }
    }

    public class SummonActionModel : IActionModel
    {
        public string Type => ActionType.Summon;
        public string SpiritId { get; set; }
        public BattleSlot Position { get; set; }
    }

    public class BattleModel : IBattleModel
    {
        public string Id { get; set; }
        public IGridModel Grid { get; set; }
        public string[] PlanningOrder { get; set; }
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }
        public IList<ISpiritModel> Spirits { get; set; }
        public IList<IWitchModel> Witches { get; set; }
    }

    [System.Serializable]
    public class GridGameObjectModel
    {
        // Serializable variables
        [SerializeField] private CellView _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell
        [SerializeField] private Vector2 _cellScale = Vector2.one;

        // Properties
        public CellView CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }
        public Vector2 CellScale { get => _cellScale; set => _cellScale = value; }
    }

    public class GridModel : IGridModel
    {
        // Properties

        /// <summary>
        /// Max Number of Cells per Line in the grid
        /// </summary>
        public int MaxCellsPerRow { get; private set; }

        /// <summary>
        /// Max Number of Cells per Column in the grid
        /// </summary>
        public int MaxCellsPerColumn { get; private set; }

        /// <summary>
        /// Bidimensional array containing all the cells
        /// </summary>
        public ICellModel[,] Cells { get; private set; }

        /// <summary>
        /// Create a new instance of Battle Arena Grid using a Battle Arena Grid Builder
        /// </summary>
        /// <param name="builder"></param>
        private GridModel() { }

        public GridModel(int maxCellsPerLine, int maxCellsPerColumn, ICellModel[,] cells)
        {
            MaxCellsPerRow = maxCellsPerColumn;
            MaxCellsPerColumn = maxCellsPerColumn;
            Cells = cells;
        }

        public sealed class Builder
        {
            public int MaxCellsPerColumn { get; set; }
            public int MaxCellsPerRow { get; set; }
            public CellModel.Builder[,] CellBuilders { get; set; }

            public IGridModel Build()
            {
                IGridModel gridModel = new GridModel()
                {
                    MaxCellsPerColumn = this.MaxCellsPerColumn,
                    MaxCellsPerRow = this.MaxCellsPerRow,
                    Cells = new CellModel[MaxCellsPerRow, MaxCellsPerColumn]
                };

                for (int i = 0; i < MaxCellsPerRow; i++)
                {
                    for (int j = 0; j < MaxCellsPerColumn; j++)
                    {
                        CellModel.Builder cellBuilder = CellBuilders[i, j];
                        if (cellBuilder != null) // if null, cell will be empty
                        {
                            ICellModel battleArenaCell = cellBuilder.Build();
                            gridModel.Cells[i, j] = battleArenaCell;
                        }
                    }
                }
                return gridModel;
            }
        }
    }

    public class CellModel : ICellModel
    {
        // Properties
        public string ObjectId { get; set; }
        public int Height { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

        // Constructor
        private CellModel() { }

        public sealed class Builder
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; set; }

            public ICellModel Build()
            {
                ICellModel cellModel = new CellModel()
                {
                    Height = this.Height,
                    X = this.X,
                    Y = this.Y
                };
                return cellModel;
            }
        }
    }
}
