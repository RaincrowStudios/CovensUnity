using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class PlanningPhase : IState
    {
        // Variables
        private float _startTime = 0f;
        private IEnumerator<bool?> sendFinishPlanningPhase;
        private bool? _isPlanningPhaseFinished;
        private ICoroutineHandler _coroutineStarter;
        private AbstractGameMasterController _gameMaster;
        private IQuickCastView _quickCastView;
        private ISummoningView _summoningView;
        private ICharactersTurnOrderView _charactersTurnOrderView;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;
        private ICellView[,] _gridView;

        // Properties
        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter,
                             AbstractGameMasterController gameMaster,
                             IQuickCastView quickCastView,
                             ISummoningView summoningView,
                             ICharactersTurnOrderView charactersTurnOrderView,
                             ITurnModel turnModel,
                             IBattleModel battleModel,
                             ICellView[,] gridView)
        {
            _coroutineStarter = coroutineStarter;
            _gameMaster = gameMaster;
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
                // copy actions to array
                int numActions = _turnModel.ActionsRequested.Count;
                IActionModel[] actions = new IActionModel[numActions];
                _turnModel.ActionsRequested.CopyTo(actions, 0);

                sendFinishPlanningPhase = _gameMaster.SendFinishPlanningPhase(_battleModel.Id, actions, OnPlanningPhaseFinished);
                _coroutineStarter.Invoke(sendFinishPlanningPhase);

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

            // wait for planning phase finished event
            yield return new WaitUntil(() => _isPlanningPhaseFinished.GetValueOrDefault());
        }

        private bool HasActionsAvailable()
        {
            return _turnModel.MaxActionsAllowed > _turnModel.ActionsRequested.Count;
        }

        #region Socket Events

        private void OnPlanningPhaseFinished(PlanningPhaseFinishedEventArgs args)
        {
            _isPlanningPhaseFinished = true;
        }

        #endregion

        #region Actions

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

        #endregion
    }
}