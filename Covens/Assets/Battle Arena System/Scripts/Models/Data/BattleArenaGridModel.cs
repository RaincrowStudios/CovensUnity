using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class BattleArenaGridModel : IBattleArenaGridModel
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
        public IBattleArenaCellModel[,] Cells { get; private set; }

        /// <summary>
        /// Create a new instance of Battle Arena Grid using a Battle Arena Grid Builder
        /// </summary>
        /// <param name="builder"></param>
        public BattleArenaGridModel(BattleArenaGridBuilder builder)
        {
            MaxCellsPerColumn = builder.MaxCellsPerColumn;
            MaxCellsPerLine = builder.MaxCellsPerLine;

            Cells = new BattleArenaCellModel[MaxCellsPerColumn, MaxCellsPerLine];

            for (int i = 0; i < MaxCellsPerColumn; i++)
            {
                for (int j = 0; j < MaxCellsPerLine; j++)
                {
                    BattleArenaCellBuilder cellBuilder = builder.CellBuilders[i, j];
                    if (cellBuilder != null) // if null, cell will be empty
                    {
                        IBattleArenaCellModel battleArenaCell = new BattleArenaCellModel(i, j, cellBuilder);
                        Cells[i, j] = battleArenaCell;
                    }                                        
                }
            }
        }
    }
}