using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public class BattleArenaModel
    {
        public BattleGrid BattleGrid { get; set; }
    }

    public class BattleGrid : IGridModel {
        public int MaxCellsPerLine { get; set; }
        public int MaxCellsPerColumn { get; set; }
        public ICellModel[,] Cells { get; set; }
    }
}
