using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Factory
{
    public class MockBattleArenaGridFactory : AbstractBattleArenaGridFactory
    {
        public override IBattleArenaGridModel Create()
        {
            BattleArenaGridBuilder gridBuilder;
            {
                gridBuilder = new BattleArenaGridBuilder()
                {
                    MaxCellsPerLine = 5,
                    MaxCellsPerColumn = 5,
                };

                gridBuilder.CellBuilders = new BattleArenaCellBuilder[gridBuilder.MaxCellsPerColumn, gridBuilder.MaxCellsPerLine];

                // create first column
                gridBuilder.CellBuilders[0, 0] = null;
                gridBuilder.CellBuilders[0, 1] = new BattleArenaCellBuilder();// { Height = 1 };
                gridBuilder.CellBuilders[0, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[0, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[0, 4] = new BattleArenaCellBuilder();

                // create second column
                gridBuilder.CellBuilders[1, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 1] = null;
                gridBuilder.CellBuilders[1, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 4] = new BattleArenaCellBuilder();

                // create third column
                gridBuilder.CellBuilders[2, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 2] = null;
                gridBuilder.CellBuilders[2, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 4] = new BattleArenaCellBuilder();// { Height = 2 };

                // create fourth column
                gridBuilder.CellBuilders[3, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 3] = null;
                gridBuilder.CellBuilders[3, 4] = new BattleArenaCellBuilder();

                // create fifth column
                gridBuilder.CellBuilders[4, 0] = new BattleArenaCellBuilder();// { Height = 3 };
                gridBuilder.CellBuilders[4, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 4] = null;
            }

            return new BattleArenaGridModel(gridBuilder);
        }
    }
}