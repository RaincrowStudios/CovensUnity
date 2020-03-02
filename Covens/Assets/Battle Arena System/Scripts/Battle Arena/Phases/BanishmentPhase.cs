using System.Collections;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
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
