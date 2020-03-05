using Raincrow.BattleArena.Views;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public class BattleModel : IBattleModel
    {
        public string Id { get; set; }
        public IGridUIModel GridUI { get; set; }
        public string[] PlanningOrder { get; set; }
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }
    }

    [System.Serializable]
    public class GridGameObjectModel : IGridGameObjectModel
    {
        // Serializable variables
        [SerializeField] private CellUIController _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell
        [SerializeField] private Vector2 _cellScale = Vector2.one;

        // Properties
        public CellUIController CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }
        public Vector2 CellScale { get => _cellScale; private set => _cellScale = value; }
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

        public void SetObjectToGrid(IObjectModel objectModel, int row, int col)
        {
            if (objectModel.BattleSlot.HasValue)
            {
                // remove from previous space
                RemoveObjectFromGrid(objectModel);
            }

            ICellModel cell = Cells[row, col];
            cell.ObjectId = objectModel.Id;

            objectModel.BattleSlot = new BattleSlot()
            {
                Row = cell.X,
                Col = cell.Y
            };            
        }

        public void RemoveObjectFromGrid(IObjectModel objectModel)
        {
            int row = objectModel.BattleSlot.Value.Row;
            int col = objectModel.BattleSlot.Value.Col;            

            Cells[row, col].ObjectId = string.Empty;
            objectModel.BattleSlot = null;
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

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(ObjectId);
        }

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
