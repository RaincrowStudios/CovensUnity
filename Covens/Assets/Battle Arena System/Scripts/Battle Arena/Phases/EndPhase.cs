using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class EndPhase : IState
    {
        // Variables
        private IBattleResultModel _battleResult;

        // Properties
        public string Name => "End Phase";

        public EndPhase(IBattleResultModel battleResultModel)
        {
            _battleResult = battleResultModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            Debug.Log(_battleResult.Type);
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