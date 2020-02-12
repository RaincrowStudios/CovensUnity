using Raincrow.BattleArena.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public abstract class AbstractGameMasterController : MonoBehaviour, IGameMasterController
    {
        public PlanningPhaseStartEvent OnPlanningPhaseReadyEvent { get; } = new PlanningPhaseStartEvent();
        public TurnResolutionEvent OnTurnResolutionEvent { get; } = new TurnResolutionEvent();
        public BattleEndEvent OnBattleEndEvent { get; } = new BattleEndEvent();

        /// <summary>
        ///  Send to server an action to move the player on the grid
        /// </summary>
        /// <param name="battleId"></param>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendMove();

        /// <summary>
        ///  Send to server an action to flee of battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendFlee();

        /// <summary>
        ///  Send to server that the player is ready to the battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendPlanningPhaseReady(string battleId);        
    }
}