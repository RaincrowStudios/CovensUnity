using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class BattleArenaCellModel : IBattleArenaCellModel
    {
        public int Column { get; private set; }

        public int Line { get; private set; }

        public int Height { get; private set; }

        public BattleArenaCellModel(int column, int line, BattleArenaCellBuilder builder)
        {
            Column = column;
            Line = line;
            Height = builder.Height;
        }    
    }
}