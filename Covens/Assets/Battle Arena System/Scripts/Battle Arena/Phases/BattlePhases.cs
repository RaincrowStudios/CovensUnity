using System.Collections;
using System.Collections.Generic;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phase
{
    public class InitiativePhase : IState<IBattleModel>
    {
        // Variables
        private IGameMasterController _gameMaster;
        private ICoroutineHandler _coroutineHandler;
        private IEnumerator<bool?> _sendPlanningPhaseReady;
        private bool? _isPlanningPhaseReady; 

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineHandler = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {   
            // Set Game Master
            _gameMaster = context.GameMaster;

            // Create the Send Planning Phase Ready Coroutine
            _sendPlanningPhaseReady = _gameMaster.SendPlanningPhaseReady(context.Id);

            // Start the Send Planning Phase Ready Coroutine
            _coroutineHandler.Invoke(_sendPlanningPhaseReady);

            // Subscribe to the 'On Planning Phase Ready' event
            _gameMaster.OnPlanningPhaseReadyEvent.AddListener(OnPlanningPhaseReady);

            yield return null;
        }        

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (_isPlanningPhaseReady.GetValueOrDefault())
            {
                yield return stateMachine.ChangeState<PlanningPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            // Stop Send Planning Phase Ready Coroutine
            if (_sendPlanningPhaseReady != null)
            {
                _coroutineHandler.StopInvoke(_sendPlanningPhaseReady);
            }            

            // Unsubscribe to the 'On Planning Phase Ready' event
            _gameMaster.OnPlanningPhaseReadyEvent?.RemoveListener(OnPlanningPhaseReady);

            yield return null;
        }

        private void OnPlanningPhaseReady(PlanningPhaseReadyEventArgs args)
        {
            _isPlanningPhaseReady = true;
        }
    }

    public class PlanningPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            yield return null;
        }        
    }

    public class ActionResolutionPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<BanishmentPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            yield return null;
        }
    }

    public class BanishmentPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;

        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineHandler coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<InitiativePhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            yield return null;
        }        
    }
}
