using Raincrow.BattleArena.Events;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Controller
{
    public interface IGameMasterController
    {
        /// <summary>
        ///  Send to server an action to move the player on the grid
        /// </summary>
        /// <param name="battleId"></param>
        /// <returns></returns>
        IEnumerator<bool?> SendMove();

        /// <summary>
        ///  Send to server an action to flee of battle
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool?> SendFlee();

        /// <summary>
        ///  Send to server that the player is ready for planning
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool?> SendPlanningPhaseReady(string battleId);

        /// <summary>
        /// Socket event
        /// </summary>
        PlanningPhaseStartEvent OnPlanningPhaseReadyEvent { get; }

        /// <summary>
        /// Socket event
        /// </summary>
        TurnResolutionEvent OnTurnResolutionEvent { get; }

        /// <summary>
        /// Socket event
        /// </summary>
        BattleEndEvent OnBattleEndEvent { get; }
    }
}