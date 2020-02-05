using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Factory
{
    public class MockGridFactory : AbstractGridModelFactory
    {
        public override IGridModel Create(AbstractCharacterModelFactory characterFactory)
        {
            GridBuilder gridBuilder;
            {
                gridBuilder = new GridBuilder()
                {
                    MaxCellsPerLine = 5,
                    MaxCellsPerColumn = 5,
                };

                gridBuilder.CellBuilders = new CellBuilder[gridBuilder.MaxCellsPerColumn, gridBuilder.MaxCellsPerLine];

                // Characters
                ICharacterModel witch1 = characterFactory.Create();
                ICharacterModel witch2 = characterFactory.Create();
                //ICharacterModel witch3 = new CharacterModel(new CharacterBuilder());

                // create first column
                gridBuilder.CellBuilders[0, 0] = null;
                gridBuilder.CellBuilders[0, 1] = new CellBuilder();// { Height = 1 };
                gridBuilder.CellBuilders[0, 2] = new CellBuilder() { CharacterModel = witch1 };
                gridBuilder.CellBuilders[0, 3] = new CellBuilder();
                gridBuilder.CellBuilders[0, 4] = new CellBuilder();

                // create second column
                gridBuilder.CellBuilders[1, 0] = new CellBuilder();
                gridBuilder.CellBuilders[1, 1] = null;
                gridBuilder.CellBuilders[1, 2] = new CellBuilder();
                gridBuilder.CellBuilders[1, 3] = new CellBuilder();
                gridBuilder.CellBuilders[1, 4] = new CellBuilder();

                // create third column
                gridBuilder.CellBuilders[2, 0] = new CellBuilder();
                gridBuilder.CellBuilders[2, 1] = new CellBuilder();
                gridBuilder.CellBuilders[2, 2] = null;
                gridBuilder.CellBuilders[2, 3] = new CellBuilder();
                gridBuilder.CellBuilders[2, 4] = new CellBuilder();// { Height = 2 };

                // create fourth column
                gridBuilder.CellBuilders[3, 0] = new CellBuilder();
                gridBuilder.CellBuilders[3, 1] = new CellBuilder();
                gridBuilder.CellBuilders[3, 2] = new CellBuilder();
                gridBuilder.CellBuilders[3, 3] = null;
                gridBuilder.CellBuilders[3, 4] = new CellBuilder();

                // create fifth column
                gridBuilder.CellBuilders[4, 0] = new CellBuilder();// { Height = 3 };
                gridBuilder.CellBuilders[4, 1] = new CellBuilder() { CharacterModel = witch2 };
                gridBuilder.CellBuilders[4, 2] = new CellBuilder();
                gridBuilder.CellBuilders[4, 3] = new CellBuilder();
                gridBuilder.CellBuilders[4, 4] = null;
            }

            return new GridModel(gridBuilder);
        }
    }
}