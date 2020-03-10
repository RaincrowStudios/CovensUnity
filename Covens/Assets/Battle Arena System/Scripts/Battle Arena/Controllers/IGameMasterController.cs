using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controllers
{
    public interface IGameMasterController
    {
        /// <summary>
        ///  Send to server that the player is ready for planning
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool?> SendPlanningPhaseReady(string battleId, string playerId, UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady, UnityAction<BattleEndEventArgs> onBattleEnd);

        IEnumerator<bool?> SendFinishPlanningPhase(string battleId, IActionRequestModel[] actions, UnityAction<PlanningPhaseFinishedEventArgs> onPlanningPhaseFinished);
    }
}