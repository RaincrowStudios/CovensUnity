using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public class BattleModel : IBattleModel
    {
        public IGridModel Grid { get; set; }
    }

    [System.Serializable]
    public class GridGameObjectModel
    {
        // Serializable variables
        [SerializeField] private GameObject _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell

        // Properties
        public GameObject CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }
    }

    public class GridModel : IGridModel
    {
        // Properties

        /// <summary>
        /// Max Number of Cells per Line in the grid
        /// </summary>
        public int MaxCellsPerLine { get; private set; }

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
        public GridModel(GridBuilder builder)
        {
            MaxCellsPerColumn = builder.MaxCellsPerColumn;
            MaxCellsPerLine = builder.MaxCellsPerLine;

            Cells = new CellModel[MaxCellsPerColumn, MaxCellsPerLine];

            for (int i = 0; i < MaxCellsPerColumn; i++)
            {
                for (int j = 0; j < MaxCellsPerLine; j++)
                {
                    CellBuilder cellBuilder = builder.CellBuilders[i, j];
                    if (cellBuilder != null) // if null, cell will be empty
                    {
                        ICellModel battleArenaCell = new CellModel(cellBuilder);
                        Cells[i, j] = battleArenaCell;
                    }
                }
            }
        }
    }

    public class GridBuilder
    {
        public int MaxCellsPerLine { get; set; }
        public int MaxCellsPerColumn { get; set; }
        public CellBuilder[,] CellBuilders { get; set; }
    }

    public class CellModel : ICellModel
    {
        // Properties
        public int Height { get; private set; }

        // Constructor
        public CellModel(CellBuilder builder)
        {
            Height = builder.Height;
        }
    }    

    public class CellBuilder
    {
        public int Height { get; set; }
    }
}
