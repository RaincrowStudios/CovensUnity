using System.Collections;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phase
{
    public class InitiativePhase : IState<IBattleModel>
    {
        // Variables
        private Coroutine<bool?> _sendReadyBattleResponse;
        private IGameMasterController _gameMaster;
        private ICoroutineDispatcher _dispatcher;

        // Properties
        public string Name => "Initiative Phase";

        public InitiativePhase(ICoroutineDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {            
            _gameMaster = context.GameMaster;
            _sendReadyBattleResponse = _dispatcher.Dispatch(_gameMaster.SendReadyBattle(context.Id));
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {            
            if (_sendReadyBattleResponse.ReturnValue.HasValue)
            {
                yield return stateMachine.ChangeState<PlanningPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            yield return null;
        }        
    }

    public class PlanningPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;
        private ICoroutineDispatcher _dispatcher;

        public string Name => "Planning Phase";

        public PlanningPhase(ICoroutineDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
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
        private ICoroutineDispatcher _dispatcher;

        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
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
        private ICoroutineDispatcher _dispatcher;

        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
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
