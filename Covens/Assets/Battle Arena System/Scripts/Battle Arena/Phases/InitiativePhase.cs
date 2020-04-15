using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using System;
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
        private IList<ICharacterController> _summonedCharacters = new List<ICharacterController>();
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
            // update cooldowns
            _battleModel.UpdateCooldowns();

            // Save Summoned Characters
            _summonedCharacters.Clear();
            _summonedCharacters.AddRange(_turnModel.StartingCharacters);

            // Move camera to center
            IEnumerator moveTo = _cameraTargetController.MoveTo(Vector3.zero, _moveSpeed);
            yield return _coroutineHandler.Invoke(moveTo);

            // Add New Characters
            Coroutine<IList<ICharacterController>> addNewCharacters = _coroutineHandler.Invoke(AddNewCharacters());
            yield return addNewCharacters;

            _summonedCharacters.AddRange(addNewCharacters.ReturnValue);

            // Reset Turn Model
            _turnModel.Reset();

            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _gameMaster.SendPlanningPhaseReady(_battleModel.Id, _playerId, OnPlanningPhaseReady, OnAddParticipants, OnBattleEnd);

            // Start the Send Planning Phase Ready Coroutine
            _coroutineHandler.Invoke(_sendPlanningPhaseReady);

            yield return null;
        }        

        private IEnumerator<IList<ICharacterController>> AddNewCharacters()
        {
            IList<ICharacterController> newCharacters = new List<ICharacterController>();
            foreach (ICharacterModel character in _turnModel.NewCharacters)
            {
                if (character.BattleSlot.HasValue)
                {
                    BattleSlot characterPos = character.BattleSlot.Value;
                    IEnumerator<ICharacterController> enumerator = _battleModel.GridUI.SpawnObjectOnGrid(character, characterPos.Row, characterPos.Col);
                    Coroutine<ICharacterController> spawnObjectOnGrid = _coroutineHandler.Invoke(enumerator);
                    while (spawnObjectOnGrid.keepWaiting)
                    {
                        yield return null;
                    }

                    newCharacters.Add(spawnObjectOnGrid.ReturnValue);
                }
            }
            yield return newCharacters;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (_isPlanningPhaseReady.GetValueOrDefault())
            {
                // Show Summon Animation
                yield return _animController.Summon(_summonedCharacters);
                _turnModel.StartingCharacters.Clear();
                _turnModel.NewCharacters.Clear();

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

        private void OnAddParticipants(AddParticipantsEventArgs args)
        {
            List<GenericCharacterObjectServer> newCharacters = new List<GenericCharacterObjectServer>();
            foreach (var character in args.Participants)
            {
                if (!_battleModel.GridUI.HasCharacter(character.Id))
                {
                    newCharacters.Add(character);
                }
            }
            _turnModel.NewCharacters.AddRange(newCharacters);
        }

        #endregion
    }
}