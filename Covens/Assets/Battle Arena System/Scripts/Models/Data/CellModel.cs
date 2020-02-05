using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class CellModel : ICellModel
    {
        // Properties

        public int Height { get; private set; }

        public ICharacterModel CharacterModel { get; private set; }

        // Constructor
        public CellModel(CellBuilder builder)
        {
            Height = builder.Height;
            CharacterModel = builder.CharacterModel;
        }    
    }
}