using System.Collections;
using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;

namespace Raincrow.BattleArena.Phase
{
    public class InitiativePhase : IState<IBattleModel>
    {
        public IEnumerator Enter(IBattleModel context)
        {
            yield return null;
        }

        public IEnumerator Exit(IBattleModel context)
        {
            yield return null;
        }

        public void Update(IBattleModel context, float deltaTime)
        {

        }
    }

    public class PlanningPhase : IState<IBattleModel>
    {
        public IEnumerator Enter(IBattleModel context)
        {
            yield return null;
        }

        public IEnumerator Exit(IBattleModel context)
        {
            yield return null;
        }

        public void Update(IBattleModel context, float deltaTime)
        {

        }
    }

    public class ActionResolutionPhase : IState<IBattleModel>
    {
        public IEnumerator Enter(IBattleModel context)
        {
            yield return null; 
        }

        public IEnumerator Exit(IBattleModel context)
        {
            yield return null;
        }

        public void Update(IBattleModel context, float deltaTime)
        {
            
        }
    }

    public class BanishmentPhase : IState<IBattleModel>
    {
        public IEnumerator Enter(IBattleModel context)
        {
            yield return null;
        }

        public IEnumerator Exit(IBattleModel context)
        {
            yield return null;
        }

        public void Update(IBattleModel context, float deltaTime)
        {

        }
    }
}
