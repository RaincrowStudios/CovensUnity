using Raincrow.BattleArena.Controller;
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
        private ITurnModel _turnModel;

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter, ITurnModel turnModel)
        {
            _coroutineStarter = coroutineStarter;
            _turnModel = turnModel;
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