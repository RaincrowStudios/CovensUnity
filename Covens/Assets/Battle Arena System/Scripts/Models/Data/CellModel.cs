using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class CellModel : ICellModel
    {
        // Properties
        public int Column { get; private set; }

        public int Line { get; private set; }

        public int Height { get; private set; }

        // Constructor
        public CellModel(int column, int line, CellBuilder builder)
        {
            Column = column;
            Line = line;
            Height = builder.Height;
        }    
    }
}