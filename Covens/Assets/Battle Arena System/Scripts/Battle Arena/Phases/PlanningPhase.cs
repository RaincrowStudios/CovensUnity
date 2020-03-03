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
        private IEnumerator<bool?> _sendFinishPlanningPhase;
        private bool? _isPlanningPhaseFinished;
        private ICoroutineHandler _coroutineStarter;
        private AbstractGameMasterController _gameMaster;
        private ICountdownView _countdownView;
        private IQuickCastView _quickCastView;
        private ISummoningView _summoningView;
        private ICharactersTurnOrderView _charactersTurnOrderView;
        private ITurnModel _turnModel;
        private IBattleModel _battleModel;
        private ICellView[,] _gridView;
        private BattleSlot? _selectedSlot;
        private string _objectId;

        // Properties
        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter,
                             AbstractGameMasterController gameMaster,
                             IQuickCastView quickCastView,
                             ISummoningView summoningView,
                             ICharactersTurnOrderView charactersTurnOrderView,
                             ITurnModel turnModel,
                             IBattleModel battleModel,
                             ICellView[,] gridView,
                             ICountdownView countdownView)
        {
            _coroutineStarter = coroutineStarter;
            _isPlanningPhaseFinished = null;
            _sendFinishPlanningPhase = null;
            _gameMaster = gameMaster;
            _summoningView = summoningView;
            _quickCastView = quickCastView;
            _charactersTurnOrderView = charactersTurnOrderView;
            _turnModel = turnModel;
            _battleModel = battleModel;
            _gridView = gridView;
            _countdownView = countdownView;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _startTime = Time.time;

            _quickCastView.Show(OnClickFly, OnClickSummon, OnClickFlee, OnCastSpell);

            //Start countdown turn
            _countdownView.Show(_turnModel.PlanningMaxTime);

            // Add Click Events
            for (int i = 0; i < _battleModel.Grid.MaxCellsPerRow; i++)
            {
                for (int j = 0; j < _battleModel.Grid.MaxCellsPerColumn; j++)
                {
                    ICellView cellView = _gridView[i, j];
                    cellView.OnCellClick.AddListener(CheckInput);
                }
            }

            _selectedSlot = null;

            // Show Character Turn Order View
            IEnumerator showCharacterTurnOrderView = _charactersTurnOrderView.Show(_turnModel.PlanningOrder, _turnModel.MaxActionsAllowed, _battleModel.Witches, _battleModel.Spirits);
            yield return _coroutineStarter.Invoke(showCharacterTurnOrderView);
        }

        private void CheckInput(ICellModel cellModel)
        {
            _selectedSlot = new BattleSlot()
            {
                Row = cellModel.X,
                Col = cellModel.Y
            };

            _objectId = cellModel.ObjectId;

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
                int numActions = _turnModel.RequestedActions.Count;
                IActionRequestModel[] actions = new IActionRequestModel[numActions];
                _turnModel.RequestedActions.CopyTo(actions, 0);

                _sendFinishPlanningPhase = _gameMaster.SendFinishPlanningPhase(_battleModel.Id, actions, OnPlanningPhaseFinished);
                _coroutineStarter.Invoke(_sendFinishPlanningPhase);

                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
        }        

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            _quickCastView.Hide();
            _countdownView.Hide();
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

            _isPlanningPhaseFinished = null;
            _sendFinishPlanningPhase = null;

            // wait for planning phase finished event
            yield return new WaitUntil(() => _isPlanningPhaseFinished.GetValueOrDefault());
        }

        private bool HasActionsAvailable()
        {
            return _turnModel.MaxActionsAllowed > _turnModel.RequestedActions.Count;
        }

        #region Socket Events

        private void OnPlanningPhaseFinished(PlanningPhaseFinishedEventArgs args)
        {            
            _coroutineStarter.Invoke(OnPlanningPhaseFinishedCoroutine(args));
        }

        private IEnumerator OnPlanningPhaseFinishedCoroutine(PlanningPhaseFinishedEventArgs args)
        {
            foreach (BattleActor actor in args.Actors)
            {
                string characterId = actor.Id;
                foreach (BattleAction battleAction in actor.Actions)
                {
                    List<IActionResponseModel> actionsResults = new List<IActionResponseModel>(battleAction.Results);
                    if (!_turnModel.ResponseActions.ContainsKey(characterId))
                    {
                        _turnModel.ResponseActions.Add(characterId, actionsResults);
                    }
                    else
                    {
                        IList<IActionResponseModel> responseActions = _turnModel.ResponseActions[characterId];
                        actionsResults.AddRange(responseActions);
                        _turnModel.ResponseActions[characterId] = actionsResults;
                    }
                    
                    yield return null;
                }
            }
            _isPlanningPhaseFinished = true;
        }

        #endregion

        #region Actions

        private void OnClickFlee()
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue)
            {
                _turnModel.RequestedActions.Add(new FleeActionRequestModel());
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
            }
        }
        
        private void OnCastSpell(string spell)
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue && Spellcasting.CanCast(spell))
            {
                CastActionRequestModel cast = new CastActionRequestModel()
                {
                    SpellId = spell,
                    TargetId = _objectId,
                    Ingredients = new InventoryItemModel[0]
                };

                _turnModel.RequestedActions.Add(cast);
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
            }
        }

        private void OnClickFly()
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue)
            {
                _turnModel.RequestedActions.Add(new MoveActionRequestModel() { Position = _selectedSlot.Value });
                _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
            }
        }

        private void OnClickSummon()
        {
            if (HasActionsAvailable() && _selectedSlot.HasValue)
            {
                _summoningView.Open(OnSummon);
            }
        }

        private void OnSummon(string spiritID)
        {
            _turnModel.RequestedActions.Add(new SummonActionRequestModel() { Position = _selectedSlot.Value, SpiritId = spiritID });
            _charactersTurnOrderView.UpdateActionsPoints(_turnModel.RequestedActions.Count);
        }

        #endregion
    }
}