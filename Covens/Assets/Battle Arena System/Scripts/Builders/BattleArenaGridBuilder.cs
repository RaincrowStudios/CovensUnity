using UnityEngine;

namespace Raincrow.BattleArena.Builder
{
    public class BattleArenaGridBuilder
    {        
        public int MaxCellsPerLine { get; set; }
        public int MaxCellsPerColumn { get; set; }
        public BattleArenaCellBuilder[,] CellBuilders { get; set; }
    }
}