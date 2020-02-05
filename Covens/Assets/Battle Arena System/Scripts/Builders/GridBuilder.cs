namespace Raincrow.BattleArena.Builder
{
    public class GridBuilder
    {        
        public int MaxCellsPerLine { get; set; }
        public int MaxCellsPerColumn { get; set; }
        public CellBuilder[,] CellBuilders { get; set; }
    }
}