using Raincrow.BattleArena.View;
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

    public class ActionModel
    {        
        public virtual string Type { get; }
    }

    public class MoveActionModel : ActionModel
    {
        public override string Type => ActionType.Move;
        public BattleSlot Position { get; set; }
    }

    public class CastSpellActionModel : ActionModel
    {
        public override string Type => ActionType.Cast;
        public string SpellId { get; set; }
        public string TargetId { get; set; }
        public InventoryItemModel[] Ingredients { get; set; }        
    }

    public class SummonActionModel : ActionModel
    {
        public override string Type => ActionType.Summon;
        public string SpiritId { get; set; }
        public BattleSlot Position { get; set; }
    }

    public class BattleModel : IBattleModel
    {
        public string Id { get; set; }
        public IGridModel Grid { get; set; }
        public IList<ICharacterModel> Characters { get; set; }
        public string[] PlanningOrder { get; set; }
        public float PlanningMaxTime { get; set; }
        public int MaxActionsAllowed { get; set; }        
    }

    [System.Serializable]
    public class GridGameObjectModel
    {
        // Serializable variables
        [SerializeField] private CellView _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell

        // Properties
        public CellView CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }
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
        public GridModel(GridBuilder builder)
        {
            MaxCellsPerColumn = builder.MaxCellsPerColumn;
            MaxCellsPerRow = builder.MaxCellsPerRow;

            Cells = new CellModel[MaxCellsPerColumn, MaxCellsPerRow];

            for (int i = 0; i < MaxCellsPerColumn; i++)
            {
                for (int j = 0; j < MaxCellsPerRow; j++)
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

        public GridModel(int maxCellsPerLine, int maxCellsPerColumn, ICellModel[,] cells)
        {
            MaxCellsPerRow = maxCellsPerColumn;
            MaxCellsPerColumn = maxCellsPerColumn;
            Cells = cells;
        }
    }

    public class GridBuilder
    {        
        public int MaxCellsPerColumn { get; set; }
        public int MaxCellsPerRow { get; set; }
        public CellBuilder[,] CellBuilders { get; set; }
    }

    public class CellModel : ICellModel
    {
        // Properties
        public string ObjectId { get; set; }
        public int Height { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

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
