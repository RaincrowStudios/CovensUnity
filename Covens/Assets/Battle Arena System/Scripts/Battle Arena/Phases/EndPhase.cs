using Raincrow.StateMachines;
using System.Collections;

namespace Raincrow.BattleArena.Phases
{
    public class EndPhase : IState
    {
        public string Name => "End Phase";

        public EndPhase()
        {

        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            yield return null;
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            yield return null;
        }        
    }
}