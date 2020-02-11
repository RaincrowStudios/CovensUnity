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
            Debug.LogFormat("Enter {0}", Name);
            _gameMaster = context.GameMaster;
            _sendReadyBattleResponse = _dispatcher.Dispatch(_gameMaster.SendReadyBattle(context.Id));
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {            
            if (!_sendReadyBattleResponse.ReturnValue.HasValue)
            {
                Debug.LogFormat("Update {0}", Name);
            }
            else
            {
                yield return stateMachine.ChangeState<PlanningPhase>();
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Exit {0}", Name);
            yield return null;
        }        
    }

    public class PlanningPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;

        public string Name => "Planning Phase";

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Enter {0}", Name);
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<ActionResolutionPhase>();
            }
            else
            {
                Debug.LogFormat("Update {0}", Name);
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Exit {0}", Name);
            yield return null;
        }        
    }

    public class ActionResolutionPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;

        public string Name => "Action Resolution Phase";

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Enter {0}", Name);
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<BanishmentPhase>();
            }
            else
            {
                Debug.LogFormat("Update {0}", Name);
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Exit {0}", Name);
            yield return null;
        }
    }

    public class BanishmentPhase : IState<IBattleModel>
    {
        private float _startTime = 0f;

        public string Name => "Banishment Phase";

        public IEnumerator Enter(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Enter {0}", Name);
            _startTime = Time.time;
            yield return null;
        }

        public IEnumerator Update(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            if (Time.time - _startTime > 3f)
            {
                yield return stateMachine.ChangeState<InitiativePhase>();
            }
            else
            {
                Debug.LogFormat("Update {0}", Name);
            }
        }

        public IEnumerator Exit(IStateMachine<IBattleModel> stateMachine, IBattleModel context)
        {
            Debug.LogFormat("Exit {0}", Name);
            yield return null;
        }        
    }
}
