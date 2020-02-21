using Raincrow.BattleArena.Events;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Controller
{
    public class GameMasterController : AbstractGameMasterController
    {
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

        public override IEnumerator<bool?> SendPlanningPhaseReady(string battleId)
        {
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

        public override IEnumerator<bool?> SendFlee()
        {
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/flee/", "{}",
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

        public override IEnumerator<bool?> SendMove()
        {
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/move/", "{}",
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
            OnPlanningPhaseReadyEvent?.Invoke(response);
        }

        //private void OnTurnResolution(TurnResolutionEventArgs response)
        //{
        //    OnTurnResolutionEvent?.Invoke(response);
        //}

        //private void OnBattleEnd(BattleEndEventArgs response)
        //{
        //    OnBattleEndEvent?.Invoke(response);
        //}

        #endregion
    }
}