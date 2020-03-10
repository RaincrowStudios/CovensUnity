using Newtonsoft.Json;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controllers
{
    public class GameMasterController : AbstractGameMasterController
    {
        // Variables
        private UnityAction<PlanningPhaseReadyEventArgs> _onPlanningPhaseReady;
        private UnityAction<PlanningPhaseFinishedEventArgs> _onPlanningPhaseFinished;
        private UnityAction<BattleEndEventArgs> _onBattleEnd;

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            PlanningPhaseStartEventHandler.AddListener(OnPlanningPhaseReady);
            PlanningPhaseFinishedEventHandler.AddListener(OnPlanningPhaseFinished);
            BattleEndEventHandler.AddListener(OnBattleEnd);
            //TurnResolutionEventHandler.AddListener(OnTurnResolution);
            //BattleEndEventHandler.AddListener(OnBattleEnd);
        }        

        protected virtual void OnDisable()
        {
            PlanningPhaseStartEventHandler.RemoveListener(OnPlanningPhaseReady);
            PlanningPhaseFinishedEventHandler.RemoveListener(OnPlanningPhaseFinished);
            BattleEndEventHandler.RemoveListener(OnBattleEnd);
            //TurnResolutionEventHandler.RemoveListener(OnTurnResolution);
            //BattleEndEventHandler.RemoveListener(OnBattleEnd);
        }

        #endregion

        #region IGameMasterController

        public override IEnumerator<bool?> SendPlanningPhaseReady(
            string battleId, 
            string playerId,
            UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady,
            UnityAction<BattleEndEventArgs> onBattleEnd)
        {
            _onPlanningPhaseReady = onPlanningPhaseReady;
            _onBattleEnd = onBattleEnd;

            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/ready/" + battleId, "{}",
               (response, result) =>
               {
                   resultCode = result;
                   responded = true;
               });

            while (!responded)
            {
                yield return null;
            }

            // request came back as 200
            yield return resultCode == 200;
        }

        public override IEnumerator<bool?> SendFinishPlanningPhase(string battleId, IActionRequestModel[] actions, UnityAction<PlanningPhaseFinishedEventArgs> onPlanningPhaseFinished)
        {
            _onPlanningPhaseFinished = onPlanningPhaseFinished;

            string actionsJson = JsonConvert.SerializeObject(actions);
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/actions/" + battleId, actionsJson,
               (response, result) =>
               {
                   resultCode = result;
                   responded = true;
               });

            while (!responded)
            {
                yield return null;
            }

            // request came back as 200
            yield return resultCode == 200;
        }

        #endregion

        #region Unity Actions

        private void OnPlanningPhaseReady(PlanningPhaseReadyEventArgs response)
        {
            _onPlanningPhaseReady?.Invoke(response);
        }

        private void OnPlanningPhaseFinished(PlanningPhaseFinishedEventArgs response)
        {
            _onPlanningPhaseFinished?.Invoke(response);
        }

        private void OnBattleEnd(BattleEndEventArgs response)
        {
            _onBattleEnd?.Invoke(response);
        }

        #endregion
    }
}