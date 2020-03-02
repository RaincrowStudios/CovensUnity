using System;
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
        private IQuickCastView _quickCastView;
        private ISummoningView _summoningView;
        private ICharactersTurnOrderView _charactersTurnOrderView;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;
        private ICellView[,] _gridView;

        // Properties
        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter, 
                             IQuickCastView quickCastView, 
                             ISummoningView summoningView,
                             ICharactersTurnOrderView charactersTurnOrderView, 
                             ITurnModel turnModel, 
                             IBattleModel battleModel, 
                             ICellView[,] gridView)
        {
            _coroutineStarter = coroutineStarter;
            _summoningView = summoningView;
            _quickCastView = quickCastView;
            _charactersTurnOrderView = charactersTurnOrderView;
            _turnModel = turnModel;
            _battleModel = battleModel;
            _gridView = gridView;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _startTime = Time.time;

            _quickCastView.Show(OnClickFly, OnClickSummon, OnClickFlee);

            // Remove all requested actions
            _turnModel.ActionsRequested.Clear();

            // Add Click Events
            for (int i = 0; i < _battleModel.Grid.MaxCellsPerRow; i++)
            {
                for (int j = 0; j < _battleModel.Grid.MaxCellsPerColumn; j++)
                {
                    ICellView cellView = _gridView[i, j];
                    cellView.OnCellClick.AddListener(CheckInput);
                }
            }

            // Show Character Turn Order View
            IEnumerator showCharacterTurnOrderView = _charactersTurnOrderView.Show(_turnModel.PlanningOrder, _turnModel.MaxActionsAllowed, _battleModel.Witches, _battleModel.Spirits);
            yield return _coroutineStarter.Invoke(showCharacterTurnOrderView);
        }

        private void CheckInput(ICellModel cellModel)
        {
            if (cellModel.IsEmpty())
            {
                _quickCastView.OpenActionsMenu();
            }
            else
            {
                _quickCastView.OpenSpellMenu();
            }
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            if (Time.time - _startTime > _turnModel.PlanningMaxTime || !HasActionsAvailable())
            {
                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            _quickCastView.Hide();
            _charactersTurnOrderView.Hide();

            // Remove Click Events
            for (int i = 0; i < _battleModel.Grid.MaxCellsPerRow; i++)
            {
                for (int j = 0; j < _battleModel.Grid.MaxCellsPerColumn; j++)
                {
                    ICellView cellView = _gridView[i, j];
                    cellView.OnCellClick.RemoveListener(CheckInput);
                }
            }
            yield return null;
        }

        private void OnClickFlee()
        {
            if (HasActionsAvailable())
            {
                _turnModel.AddAction(new FleeActionModel());
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.ActionsRequested.Count);
            }
        }

        private void OnClickFly()
        {
            if (HasActionsAvailable())
            {
                _turnModel.AddAction(new MoveActionModel() { Position = _turnModel.SelectedSlot });
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.ActionsRequested.Count);
            }
        }

        private void OnClickSummon()
        {
            if (HasActionsAvailable())
            {
                _summoningView.Open(OnSummon);
            }
        }

        private void OnSummon(string spiritID)
        {
            _turnModel.AddAction(new SummonActionModel() { Position = _turnModel.SelectedSlot, SpiritId = spiritID });
            _charactersTurnOrderView.UpdateActionsPoints(_turnModel.ActionsRequested.Count);
        }

        private bool HasActionsAvailable()
        {
            return _turnModel.MaxActionsAllowed > _turnModel.ActionsRequested.Count;
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
