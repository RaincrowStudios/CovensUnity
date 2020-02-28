using System.Collections;
using System.Collections.Generic;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phase
{
    public class InitiativePhase : IState<ITurnController>
    {
        // Variables
        private ICoroutineHandler _coroutineHandler;
        private IEnumerator<bool?> _sendPlanningPhaseReady;
        private ITurnController _context;
        private bool? _isPlanningPhaseReady;        

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineHandler = coroutineStarter;
            _sendPlanningPhaseReady = null;
            _context = default;
            _isPlanningPhaseReady = null;
        }

        public IEnumerator Enter(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            // Set context
            _context = context;

            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _context.GameMaster.SendPlanningPhaseReady(_context.Battle.Id, OnPlanningPhaseReady);

            // Start the Send Planning Phase Ready Coroutine
            _coroutineHandler.Invoke(_sendPlanningPhaseReady);

            yield return null;
        }        

        public IEnumerator Update(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            if (_isPlanningPhaseReady.GetValueOrDefault())
            {
                yield return stateMachine.ChangeState<PlanningPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<ITurnController> stateMachine, ITurnController context)
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
                _context.PlanningOrder = new string[args.PlanningOrder.Length];
                args.PlanningOrder.CopyTo(_context.PlanningOrder, 0);
            }            
            else
            {
                _context.PlanningOrder = new string[0];
            }

            _context.PlanningMaxTime = args.PlanningMaxTime;
            _context.MaxActionsAllowed = args.MaxActionsAllowed;

            _isPlanningPhaseReady = true;
        }

        #endregion
    }

    public class PlanningPhase : IState<ITurnController>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;
        private ICharactersTurnOrderView _characterTurnOrderView;

        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter, ICharactersTurnOrderView characterOrderView)
        {
            _coroutineStarter = coroutineStarter;
            _characterTurnOrderView = characterOrderView;
        }

        public IEnumerator Enter(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            _startTime = Time.time;

            IEnumerator showCharacterTurnOrderView = _characterTurnOrderView.Show(context.PlanningOrder, context.MaxActionsAllowed, context.Battle.Witches, context.Battle.Spirits);
            yield return _coroutineStarter.Invoke(showCharacterTurnOrderView);
        }

        public IEnumerator Update(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            if (Time.time - _startTime > context.PlanningMaxTime)
            {
                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            _characterTurnOrderView.Hide();
            yield return null;
        }        
    }

    public class ActionResolutionPhase : IState<ITurnController>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<BanishmentPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            yield return null;
        }
    }

    public class BanishmentPhase : IState<ITurnController>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<InitiativePhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<ITurnController> stateMachine, ITurnController context)
        {
            yield return null;
        }        
    }
}
