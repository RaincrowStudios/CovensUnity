using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class CharacterModel : ICharacterModel
    {
        // Properties

        /// <summary>
        /// Player or Spirit ID
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Position X on Grid
        /// </summary>
        public int Line { set; get; }

        /// <summary>
        /// Position Y on Grid
        /// </summary>
        public int Column { set; get; }

        public CharacterModel(CharacterBuilder builder)
        {

        }
    }
}