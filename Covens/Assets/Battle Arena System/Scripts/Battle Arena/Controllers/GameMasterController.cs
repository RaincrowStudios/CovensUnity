using Newtonsoft.Json;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controller
{
    public class GameMasterController : AbstractGameMasterController
    {
        // Variables
        private UnityAction<PlanningPhaseReadyEventArgs> _onPlanningPhaseReady;
        private UnityAction<PlanningPhaseFinishedEventArgs> _onPlanningPhaseFinished;

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            PlanningPhaseStartEventHandler.AddListener(OnPlanningPhaseReady);
            //TurnResolutionEventHandler.AddListener(OnTurnResolution);
            //BattleEndEventHandler.AddListener(OnBattleEnd);
        }

        protected virtual void OnDisable()
        {
            PlanningPhaseStartEventHandler.RemoveListener(OnPlanningPhaseReady);
            //TurnResolutionEventHandler.RemoveListener(OnTurnResolution);
            //BattleEndEventHandler.RemoveListener(OnBattleEnd);
        }

        #endregion

        #region IGameMasterController

        public override IEnumerator<bool?> SendPlanningPhaseReady(string battleId, UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady)
        {
            _onPlanningPhaseReady = onPlanningPhaseReady;

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

        public override IEnumerator<bool?> SendFinishPlanningPhase(string battleId, IActionModel[] actions, UnityAction<PlanningPhaseFinishedEventArgs> onPlanningPhaseFinished)
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

        #endregion
    }
}