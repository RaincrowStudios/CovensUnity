using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controllers
{
    public abstract class AbstractGameMasterController : MonoBehaviour, IGameMasterController
    {


        /// <summary>
        ///  Send to server that the player is ready to the battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendPlanningPhaseReady(
            string battleId, 
            string playerId, 
            UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady, 
            UnityAction<AddParticipantsEventArgs> addParticipants, 
            UnityAction<BattleEndEventArgs> onBattleEnd);

        public abstract IEnumerator<bool?> SendFinishPlanningPhase(string battleId, IActionRequestModel[] actions, UnityAction<PlanningPhaseFinishedEventArgs> onPlanningPhaseFinished);
    }
}