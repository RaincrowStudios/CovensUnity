using Raincrow.StateMachines;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
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
}