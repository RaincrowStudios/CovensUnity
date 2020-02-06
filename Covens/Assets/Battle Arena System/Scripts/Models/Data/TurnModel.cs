using Raincrow.BattleArena.Builder;

namespace Raincrow.BattleArena.Model
{
    public class TurnModel : ITurnModel
    {
        // Properties

        /// <summary>
        /// Array of characters within the grid
        /// </summary>
        public ICharacterModel[] Character { set; get; }

        /// <summary>
        /// Number of actions per turn
        /// </summary>
        public int ActionsPerTurnCount { set; get; }

        /// <summary>
        /// Time limit to perform actions in the turn
        /// </summary>
        public float TurnLimit { set; get; }

        public TurnModel()
        {
           
        }
    }
}