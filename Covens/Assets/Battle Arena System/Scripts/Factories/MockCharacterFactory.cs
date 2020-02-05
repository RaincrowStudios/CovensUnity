using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Factory
{
    public class MockCharacterFactory : AbstractCharacterModelFactory
    {
        public override ICharacterModel Create()
        {
            CharacterBuilder builder = new CharacterBuilder();
            return new CharacterModel(builder);
        }
    }
}