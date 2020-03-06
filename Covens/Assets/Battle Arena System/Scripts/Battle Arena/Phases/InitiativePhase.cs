using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Phases
{
    public class InitiativePhase : IState
    {
        // Variables
        private ICoroutineHandler _coroutineHandler;
        private IEnumerator<bool?> _sendPlanningPhaseReady;
        private bool? _isPlanningPhaseReady;
        private AbstractGameMasterController _gameMaster;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;
        private IBattleResultModel _battleResultModel;
        private string _playerId;

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(
            ICoroutineHandler coroutineStarter, 
            string playerId, 
            AbstractGameMasterController gameMaster, 
            ITurnModel turnModel, 
            IBattleModel battleModel,
            IBattleResultModel battleResultModel)
        {
            _coroutineHandler = coroutineStarter;
            _sendPlanningPhaseReady = null;
            _isPlanningPhaseReady = null;            
            _gameMaster = gameMaster;
            _turnModel = turnModel;
            _battleModel = battleModel;
            _playerId = playerId;
            _battleResultModel = battleResultModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            // Reset Turn Model
            _turnModel.Reset();

            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _gameMaster.SendPlanningPhaseReady(_battleModel.Id, _playerId, OnPlanningPhaseReady, OnBattleEnd);

            // Start the Send Planning Phase Ready Coroutine
            _coroutineHandler.Invoke(_sendPlanningPhaseReady);            

            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (_isPlanningPhaseReady.GetValueOrDefault())
            {
                yield return stateMachine.ChangeState<PlanningPhase>();
            }
            else if (!string.IsNullOrWhiteSpace(_battleResultModel.Type))
            {
                yield return stateMachine.ChangeState<EndPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            // Stop Send Planning Phase Ready Coroutine
            if (_sendPlanningPhaseReady != null)
            {
                _coroutineHandler.StopInvoke(_sendPlanningPhaseReady);
            }

            _sendPlanningPhaseReady = null;
            _isPlanningPhaseReady = null;
            yield return null;
        }

        #region Socket Events

        private void OnPlanningPhaseReady(PlanningPhaseReadyEventArgs args)
        {
            // Copy Planning order to battle model
            if (args.PlanningOrder != null)
            {
                _turnModel.PlanningOrder = new string[args.PlanningOrder.Length];
                args.PlanningOrder.CopyTo(_turnModel.PlanningOrder, 0);
            }

            _turnModel.PlanningMaxTime = args.PlanningMaxTime;
            _turnModel.MaxActionsAllowed = args.MaxActionsAllowed;
            _isPlanningPhaseReady = true;
        }

        private void OnBattleEnd(BattleEndEventArgs args)
        {
            _battleResultModel.Ranking = new string[0];
            _battleResultModel.Reward = new BattleRewardModel();
            _battleResultModel.Type = args.Type;
        }

        #endregion
    }
}