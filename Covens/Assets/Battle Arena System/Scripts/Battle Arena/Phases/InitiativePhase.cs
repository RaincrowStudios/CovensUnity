using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private IAnimationController _animController;
        private string _playerId;
        //private IList<ICharacterController> _summonedCharacters = new List<ICharacterController>();
        private ICameraTargetController _cameraTargetController;
        private float _moveSpeed;

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(
            ICoroutineHandler coroutineStarter,
            string playerId,
            AbstractGameMasterController gameMaster,
            ITurnModel turnModel,
            IBattleModel battleModel,
            IBattleResultModel battleResultModel,
            IAnimationController animController, 
            ICameraTargetController cameraTargetController,
            float moveSpeed)
        {
            _coroutineHandler = coroutineStarter;
            _sendPlanningPhaseReady = null;
            _isPlanningPhaseReady = null;
            _gameMaster = gameMaster;
            _turnModel = turnModel;
            _battleModel = battleModel;
            _playerId = playerId;
            _battleResultModel = battleResultModel;
            _animController = animController;
            _cameraTargetController = cameraTargetController;
            _moveSpeed = moveSpeed;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            // Save Summoned Characters
            //_summonedCharacters = GetSummonedCharacters();

            // Move camera to center
            IEnumerator moveTo = _cameraTargetController.MoveTo(Vector3.zero, _moveSpeed);
            yield return _coroutineHandler.Invoke(moveTo);

            // Show Summon Animation
            yield return _coroutineHandler.Invoke(SummonCharacters());

            // Reset Turn Model
            _turnModel.Reset();

            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _gameMaster.SendPlanningPhaseReady(_battleModel.Id, _playerId, OnPlanningPhaseReady, OnBattleEnd);

            // Start the Send Planning Phase Ready Coroutine
            _coroutineHandler.Invoke(_sendPlanningPhaseReady);

            yield return null;
        }

        private IEnumerator SummonCharacters()
        {
            IList<ICharacterController> summonedCharacters = new List<ICharacterController>();
            foreach (IList<IActionResponseModel> responses in _turnModel.ResponseActions.Values)
            {
                foreach (IActionResponseModel response in responses)
                {
                    if (response.Type == ActionResponseType.Join)
                    {
                        JoinActionResponseModel joinAction = response as JoinActionResponseModel;
                        if (joinAction.IsSuccess)
                        {
                            IObjectModel objectModel = joinAction.Object;

                            IEnumerator<ICharacterController> enumerator = _battleModel.GridUI.SpawnObjectOnGrid(objectModel, joinAction.Position.Row, joinAction.Position.Col);
                            Coroutine<ICharacterController> spawnObjectOnGrid = _coroutineHandler.Invoke(enumerator);
                            yield return spawnObjectOnGrid;

                            summonedCharacters.Add(spawnObjectOnGrid.ReturnValue);
                        }

                    }
                }
            }

            // Add Starting characters
            summonedCharacters.AddRange(_turnModel.StartingCharacters);
            _turnModel.StartingCharacters.Clear();

            yield return _animController.Summon(summonedCharacters);
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (_isPlanningPhaseReady.GetValueOrDefault())
            {
                // Change to Planning Phase
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
            _battleResultModel.Ranking = args.Ranking;
            _battleResultModel.Reward = new BattleRewardModel();
            _battleResultModel.Type = args.Type;
            _battleResultModel.Reward = args.Reward;
        }

        #endregion
    }
}