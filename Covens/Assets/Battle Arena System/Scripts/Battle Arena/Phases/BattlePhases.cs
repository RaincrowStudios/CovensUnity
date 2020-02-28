using System.Collections;
using System.Collections.Generic;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phase
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

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(ICoroutineHandler coroutineStarter, AbstractGameMasterController gameMaster, ITurnModel turnModel, IBattleModel battleModel)
        {
            _coroutineHandler = coroutineStarter;
            _sendPlanningPhaseReady = null;
            _isPlanningPhaseReady = null;
            _gameMaster = gameMaster;
            _turnModel = turnModel;
            _battleModel = battleModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _gameMaster.SendPlanningPhaseReady(_battleModel.Id, OnPlanningPhaseReady);

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
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            // Stop Send Planning Phase Ready Coroutine
            if (_sendPlanningPhaseReady != null)
            {
                _coroutineHandler.StopInvoke(_sendPlanningPhaseReady);
            }

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
            else
            {
                _turnModel.PlanningOrder = new string[0];
            }

            _turnModel.PlanningMaxTime = args.PlanningMaxTime;
            _turnModel.MaxActionsAllowed = args.MaxActionsAllowed;

            _isPlanningPhaseReady = true;
        }

        #endregion
    }

    public class PlanningPhase : IState
    {
        // Variables
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;
        private ICharactersTurnOrderView _characterTurnOrderView;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;

        // Properties
        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter, ICharactersTurnOrderView characterOrderView, ITurnModel turnModel, IBattleModel battleModel)
        {
            _coroutineStarter = coroutineStarter;
            _characterTurnOrderView = characterOrderView;
            _turnModel = turnModel;
            _battleModel = battleModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _startTime = Time.time;

            IEnumerator showCharacterTurnOrderView = _characterTurnOrderView.Show(_turnModel.PlanningOrder, _turnModel.MaxActionsAllowed, _battleModel.Witches, _battleModel.Spirits);
            yield return _coroutineStarter.Invoke(showCharacterTurnOrderView);
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (Time.time - _startTime > _turnModel.PlanningMaxTime)
            {
                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            _characterTurnOrderView.Hide();
            yield return null;
        }        
    }

    public class ActionResolutionPhase : IState
    {
        // Variables
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<BanishmentPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            yield return null;
        }
    }

    public class BanishmentPhase : IState
    {
        // Variables
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        // Properties
        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<InitiativePhase>();
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            yield return null;
        }        
    }
}
