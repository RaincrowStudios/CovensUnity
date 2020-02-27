using Raincrow.BattleArena.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controller
{
    public abstract class AbstractGameMasterController : MonoBehaviour, IGameMasterController
    {
        /// <summary>
        ///  Send to server that the player is ready to the battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendPlanningPhaseReady(string battleId, UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady);        
    }
}